using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Tems.Host.Configuration;

namespace Tems.Host.Services;

public sealed class DeepSeekAiSupportClient(IHttpClientFactory httpClientFactory, IOptions<AiSupportOptions> options, ILogger<DeepSeekAiSupportClient> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<string> StreamResponseAsync(
        IReadOnlyCollection<AiSupportConversationMessage> messages,
        Func<string, CancellationToken, Task> onDelta,
        CancellationToken cancellationToken)
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
        {
            throw new InvalidOperationException("DeepSeek API key is not configured.");
        }

        var client = httpClientFactory.CreateClient("DeepSeekAiSupport");
        client.BaseAddress = new Uri(EnsureTrailingSlash(settings.BaseUrl));

        using var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        request.Content = new StringContent(
            JsonSerializer.Serialize(new
            {
                model = settings.Model,
                messages = BuildRequestMessages(settings.SystemPrompt, messages),
                stream = true,
                temperature = settings.Temperature,
                max_tokens = settings.MaxTokens
            }, JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("DeepSeek request failed with status {StatusCode}: {ErrorBody}", response.StatusCode, errorBody);
            throw new HttpRequestException($"DeepSeek request failed with status {(int)response.StatusCode} ({response.ReasonPhrase})");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        var accumulatedContent = new StringBuilder();

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync(cancellationToken);

            if (line is null)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (!line.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var data = line["data:".Length..].Trim();
            if (string.Equals(data, "[DONE]", StringComparison.Ordinal))
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(data))
            {
                continue;
            }

            using var document = JsonDocument.Parse(data);
            var root = document.RootElement;
            if (!root.TryGetProperty("choices", out var choices) || choices.ValueKind != JsonValueKind.Array || choices.GetArrayLength() == 0)
            {
                continue;
            }

            var choice = choices[0];
            if (!choice.TryGetProperty("delta", out var delta))
            {
                continue;
            }

            var content = delta.TryGetProperty("content", out var contentElement)
                ? contentElement.GetString()
                : null;

            if (!string.IsNullOrEmpty(content))
            {
                accumulatedContent.Append(content);
                await onDelta(content, cancellationToken);
            }
        }

        return accumulatedContent.ToString();
    }

    public async Task<string> GenerateCompletionAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken)
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
        {
            throw new InvalidOperationException("DeepSeek API key is not configured.");
        }

        var client = httpClientFactory.CreateClient("DeepSeekAiSupport");
        client.BaseAddress = new Uri(EnsureTrailingSlash(settings.BaseUrl));

        using var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);

        request.Content = new StringContent(
            JsonSerializer.Serialize(new
            {
                model = settings.Model,
                messages = new object[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                stream = false,
                temperature = settings.TicketSummaryTemperature,
                max_tokens = settings.TicketSummaryMaxTokens
            }, JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("DeepSeek request failed with status {StatusCode}: {ErrorBody}", response.StatusCode, errorBody);
            throw new HttpRequestException($"DeepSeek request failed with status {(int)response.StatusCode} ({response.ReasonPhrase})");
        }

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(payload))
        {
            return string.Empty;
        }

        using var document = JsonDocument.Parse(payload);
        var root = document.RootElement;
        if (!root.TryGetProperty("choices", out var choices) || choices.ValueKind != JsonValueKind.Array || choices.GetArrayLength() == 0)
        {
            return string.Empty;
        }

        var choice = choices[0];
        if (choice.TryGetProperty("message", out var message) &&
            message.TryGetProperty("content", out var contentElement))
        {
            return contentElement.GetString() ?? string.Empty;
        }

        return string.Empty;
    }

    private static string EnsureTrailingSlash(string value)
        => value.EndsWith('/') ? value : value + "/";

    private static object[] BuildRequestMessages(
        string systemPrompt,
        IReadOnlyCollection<AiSupportConversationMessage> messages)
    {
        var requestMessages = new List<object>(messages.Count + 1)
        {
            new { role = "system", content = systemPrompt }
        };

        requestMessages.AddRange(messages.Select(message => new
        {
            role = message.Role,
            content = message.Content
        }));

        return requestMessages.ToArray();
    }
}
