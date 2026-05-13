using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Tems.Common.Tenant;
using Tems.Host.Configuration;
using Tems.Host.Services;
using TicketManagement.Application.Helpers;
using UserManagement.Infrastructure.Repositories;

namespace Tems.Host.Endpoints;

public static class AiSupportEndpoints
{
    public static IEndpointRouteBuilder MapAiSupportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/ai-support");
        group.RequireAuthorization();

        group.MapGet("/conversations", GetConversationsAsync);
        group.MapGet("/conversations/{conversationId}", GetConversationByIdAsync);
        group.MapDelete("/conversations/{conversationId}", DeleteConversationAsync);
        group.MapPost("/chat/stream", StreamChatAsync);

        return app;
    }

    private static async Task<IResult> GetConversationsAsync(
        HttpContext context,
        IAiSupportConversationRepository repository,
        IUserRepository userRepository,
        ITenantContext tenantContext,
        CancellationToken cancellationToken)
    {
        var userId = await ResolveCurrentUserIdAsync(context.User, userRepository, cancellationToken);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Results.Unauthorized();
        }

        var conversations = await repository.GetSummariesAsync(tenantContext.TenantId, userId, cancellationToken);
        return Results.Ok(conversations.Select(ToSummaryResponse));
    }

    private static async Task<IResult> GetConversationByIdAsync(
        string conversationId,
        HttpContext context,
        IAiSupportConversationRepository repository,
        IUserRepository userRepository,
        ITenantContext tenantContext,
        CancellationToken cancellationToken)
    {
        var userId = await ResolveCurrentUserIdAsync(context.User, userRepository, cancellationToken);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Results.Unauthorized();
        }

        var conversation = await repository.GetByIdAsync(conversationId, tenantContext.TenantId, userId, cancellationToken);
        if (conversation is null)
        {
            return Results.NotFound(new { message = "Conversation not found." });
        }

        return Results.Ok(ToDetailResponse(conversation));
    }

    private static async Task<IResult> DeleteConversationAsync(
        string conversationId,
        HttpContext context,
        IAiSupportConversationRepository repository,
        IUserRepository userRepository,
        ITenantContext tenantContext,
        CancellationToken cancellationToken)
    {
        var userId = await ResolveCurrentUserIdAsync(context.User, userRepository, cancellationToken);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Results.Unauthorized();
        }

        var deleted = await repository.DeleteAsync(conversationId, tenantContext.TenantId, userId, cancellationToken);
        return deleted
            ? Results.NoContent()
            : Results.NotFound(new { message = "Conversation not found." });
    }

    private static async Task StreamChatAsync(
        HttpContext context,
        AiSupportChatRequest request,
        DeepSeekAiSupportClient aiSupportClient,
        IAiSupportConversationRepository repository,
        IUserRepository userRepository,
        ITenantContext tenantContext,
        IOptions<AiSupportOptions> options,
        CancellationToken cancellationToken)
    {
        var settings = options.Value;
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted, cancellationToken);
        linkedCts.CancelAfter(TimeSpan.FromMinutes(Math.Max(1, settings.TimeoutMinutes)));

        if (string.IsNullOrWhiteSpace(settings.ApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "AiSupport:ApiKey is not configured."
            }, cancellationToken);
            return;
        }

        var message = request.Message?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(message))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Message is required."
            }, cancellationToken);
            return;
        }

        var userId = await ResolveCurrentUserIdAsync(context.User, userRepository, linkedCts.Token);
        if (string.IsNullOrWhiteSpace(userId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var tenantId = tenantContext.TenantId;
        var existingConversation = !string.IsNullOrWhiteSpace(request.ConversationId)
            ? await repository.GetByIdAsync(request.ConversationId, tenantId, userId, linkedCts.Token)
            : null;

        if (!string.IsNullOrWhiteSpace(request.ConversationId) && existingConversation is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Conversation not found."
            }, linkedCts.Token);
            return;
        }

        var userMessage = new AiSupportConversationMessage
        {
            Role = "user",
            Content = message,
            CreatedAt = DateTime.UtcNow
        };

        var conversation = existingConversation;
        if (conversation is null)
        {
            conversation = await repository.CreateAsync(new AiSupportConversation
            {
                TenantId = tenantId,
                UserId = userId,
                Title = BuildConversationTitle(message),
                Messages = [userMessage]
            }, linkedCts.Token);
        }
        else
        {
            await repository.AddMessageAsync(conversation.ConversationId, tenantId, userId, userMessage, linkedCts.Token);
            conversation.Messages.Add(userMessage);
            conversation.UpdatedAt = userMessage.CreatedAt;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.Headers.CacheControl = "no-cache";
        context.Response.Headers.Connection = "keep-alive";
        context.Response.Headers["X-Accel-Buffering"] = "no";
        context.Response.ContentType = "text/event-stream; charset=utf-8";

        await context.Response.StartAsync(linkedCts.Token);
        await WriteEventAsync(context.Response, "conversation", ToSummaryResponse(ToSummary(conversation)), linkedCts.Token);
        await context.Response.Body.FlushAsync(linkedCts.Token);

        var promptMessages = conversation.Messages
            .Select(x => new AiSupportConversationMessage
            {
                MessageId = x.MessageId,
                Role = x.Role,
                Content = x.Content,
                CreatedAt = x.CreatedAt
            })
            .ToList();

        var accumulatedResponse = string.Empty;

        try
        {
            accumulatedResponse = await aiSupportClient.StreamResponseAsync(
                promptMessages,
                async (chunk, ct) =>
                {
                    await WriteEventAsync(context.Response, "delta", new { chunk }, ct);
                    await context.Response.Body.FlushAsync(ct);
                },
                linkedCts.Token);

            if (!string.IsNullOrWhiteSpace(accumulatedResponse))
            {
                await repository.AddMessageAsync(
                    conversation.ConversationId,
                    tenantId,
                    userId,
                    new AiSupportConversationMessage
                    {
                        Role = "assistant",
                        Content = accumulatedResponse,
                        CreatedAt = DateTime.UtcNow
                    },
                    linkedCts.Token);
            }

            var refreshedConversation = await repository.GetByIdAsync(conversation.ConversationId, tenantId, userId, linkedCts.Token)
                ?? conversation;

            await WriteEventAsync(context.Response, "done", new
            {
                content = accumulatedResponse,
                conversation = ToSummaryResponse(ToSummary(refreshedConversation))
            }, linkedCts.Token);
            await context.Response.Body.FlushAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            if (!string.IsNullOrWhiteSpace(accumulatedResponse))
            {
                await TryPersistAssistantMessageAsync(repository, conversation.ConversationId, tenantId, userId, accumulatedResponse, CancellationToken.None);
            }
        }
        catch (OperationCanceledException)
        {
            const string timeoutMessage = "AI support timed out before completing the response.";
            await TryPersistAssistantMessageAsync(repository, conversation.ConversationId, tenantId, userId, timeoutMessage, CancellationToken.None);

            await WriteEventAsync(context.Response, "error", new
            {
                message = timeoutMessage
            }, CancellationToken.None);
            await context.Response.Body.FlushAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            const string failureMessage = "AI support is temporarily unavailable.";
            await TryPersistAssistantMessageAsync(repository, conversation.ConversationId, tenantId, userId, failureMessage, CancellationToken.None);

            await WriteEventAsync(context.Response, "error", new
            {
                message = failureMessage,
                detail = ex.Message
            }, CancellationToken.None);
            await context.Response.Body.FlushAsync(CancellationToken.None);
        }
    }

    private static AiSupportConversationSummary ToSummary(AiSupportConversation conversation)
    {
        return new AiSupportConversationSummary(
            conversation.ConversationId,
            conversation.Title,
            conversation.CreatedAt,
            conversation.UpdatedAt,
            conversation.Messages.Count);
    }

    private static AiSupportConversationSummaryResponse ToSummaryResponse(AiSupportConversationSummary conversation)
    {
        return new AiSupportConversationSummaryResponse(
            conversation.ConversationId,
            conversation.Title,
            conversation.CreatedAt,
            conversation.UpdatedAt,
            conversation.MessageCount);
    }

    private static AiSupportConversationDetailResponse ToDetailResponse(AiSupportConversation conversation)
    {
        return new AiSupportConversationDetailResponse(
            conversation.ConversationId,
            conversation.Title,
            conversation.CreatedAt,
            conversation.UpdatedAt,
            conversation.Messages.Count,
            conversation.Messages.Select(message => new AiSupportConversationMessageResponse(
                message.MessageId,
                message.Role,
                message.Content,
                message.CreatedAt)).ToList());
    }

    private static async Task<string?> ResolveCurrentUserIdAsync(
        ClaimsPrincipal principal,
        IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        return await ApprovalGateHelper.ResolveCurrentUserIdAsync(principal, userRepository, cancellationToken);
    }

    private static async Task TryPersistAssistantMessageAsync(
        IAiSupportConversationRepository repository,
        string conversationId,
        string tenantId,
        string userId,
        string content,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        await repository.AddMessageAsync(
            conversationId,
            tenantId,
            userId,
            new AiSupportConversationMessage
            {
                Role = "assistant",
                Content = content,
                CreatedAt = DateTime.UtcNow
            },
            cancellationToken);
    }

    private static string BuildConversationTitle(string firstMessage)
    {
        var normalized = string.Join(" ", firstMessage
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return "New conversation";
        }

        const int maxLength = 96;
        if (normalized.Length <= maxLength)
        {
            return normalized;
        }

        var cut = normalized.LastIndexOf(' ', maxLength);
        if (cut < maxLength / 2)
        {
            cut = maxLength;
        }

        return normalized[..cut].TrimEnd() + "...";
    }

    private static async Task WriteEventAsync(HttpResponse response, string eventName, object payload, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(payload);
        await response.WriteAsync($"event: {eventName}\n", cancellationToken);
        await response.WriteAsync($"data: {json}\n\n", cancellationToken);
    }

    private sealed record AiSupportChatRequest(string Message, string? ConversationId);

    private sealed record AiSupportConversationSummaryResponse(
        string ConversationId,
        string Title,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        int MessageCount);

    private sealed record AiSupportConversationDetailResponse(
        string ConversationId,
        string Title,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        int MessageCount,
        IReadOnlyList<AiSupportConversationMessageResponse> Messages);

    private sealed record AiSupportConversationMessageResponse(
        string MessageId,
        string Role,
        string Content,
        DateTime CreatedAt);
}
