using System.Text.Json;
using Tems.Host.Configuration;
using Tems.Host.Services;
using Microsoft.Extensions.Options;

namespace Tems.Host.Endpoints;

public static class AiSupportEndpoints
{
    public static IEndpointRouteBuilder MapAiSupportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/ai-support");
        group.RequireAuthorization();

        group.MapPost("/chat/stream", StreamChatAsync);

        return app;
    }

    private static async Task StreamChatAsync(
        HttpContext context,
        AiSupportChatRequest request,
        DeepSeekAiSupportClient aiSupportClient,
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

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.Headers.CacheControl = "no-cache";
        context.Response.Headers.Connection = "keep-alive";
        context.Response.Headers["X-Accel-Buffering"] = "no";
        context.Response.ContentType = "text/event-stream; charset=utf-8";

        await context.Response.StartAsync(linkedCts.Token);

        try
        {
            await aiSupportClient.StreamResponseAsync(
                request.Message,
                context,
                async (chunk, ct) =>
                {
                    await WriteEventAsync(context.Response, "delta", new { chunk }, ct);
                    await context.Response.Body.FlushAsync(ct);
                },
                linkedCts.Token);

            var finalResponse = context.Items.TryGetValue("ai-support-final-response", out var rawFinal)
                ? rawFinal?.ToString() ?? string.Empty
                : string.Empty;

            await WriteEventAsync(context.Response, "done", new { content = finalResponse }, linkedCts.Token);
            await context.Response.Body.FlushAsync(linkedCts.Token);
        }
        catch (OperationCanceledException)
        {
            // Request timed out or client disconnected.
        }
        catch (Exception ex)
        {
            await WriteEventAsync(context.Response, "error", new
            {
                message = "AI support is temporarily unavailable.",
                detail = ex.Message
            }, linkedCts.Token);
            await context.Response.Body.FlushAsync(linkedCts.Token);
        }
    }

    private static async Task WriteEventAsync(HttpResponse response, string eventName, object payload, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(payload);
        await response.WriteAsync($"event: {eventName}\n", cancellationToken);
        await response.WriteAsync($"data: {json}\n\n", cancellationToken);
    }

    private sealed record AiSupportChatRequest(string Message);
}
