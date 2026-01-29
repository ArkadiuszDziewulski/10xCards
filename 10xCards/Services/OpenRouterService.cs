using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using _10xCards.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace _10xCards.Services;

public sealed class OpenRouterService
{
    private const int MaxAttempts = 3;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient httpClient;
    private readonly ILogger<OpenRouterService> logger;
    private readonly string? referer;
    private readonly string? title;

    public OpenRouterService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OpenRouterService>? logger = null)
    {
        this.httpClient = httpClient;
        this.logger = logger ?? NullLogger<OpenRouterService>.Instance;

        var url = configuration["openrouter:Url"] ?? string.Empty;
        var key = configuration["openrouter:Key"] ?? string.Empty;

        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("OpenRouter configuration is missing or invalid.");
        }

        referer = configuration["openrouter:Referer"];
        title = configuration["openrouter:Title"];

        Config = new OpenRouterConfig
        {
            Url = url.Trim(),
            Key = key.Trim(),
        };
    }

    public OpenRouterConfig Config { get; }

    public OpenRouterRequest BuildRequest(
        string userMessage,
        string? systemMessage = null,
        OpenRouterResponseFormat? responseFormat = null,
        string? model = null,
        decimal? temperature = null,
        int? maxTokens = null,
        decimal? topP = null)
    {
        if (string.IsNullOrWhiteSpace(userMessage))
        {
            throw new ArgumentException("User message is required.", nameof(userMessage));
        }

        var messages = new List<OpenRouterMessage>();
        if (!string.IsNullOrWhiteSpace(systemMessage))
        {
            messages.Add(new OpenRouterMessage
            {
                Role = "system",
                Content = systemMessage.Trim(),
            });
        }

        messages.Add(new OpenRouterMessage
        {
            Role = "user",
            Content = userMessage.Trim(),
        });

        return new OpenRouterRequest
        {
            Model = string.IsNullOrWhiteSpace(model) ? Config.DefaultModel : model.Trim(),
            Messages = messages,
            ResponseFormat = responseFormat,
            Temperature = temperature ?? Config.DefaultTemperature,
            MaxTokens = maxTokens ?? Config.DefaultMaxTokens,
            TopP = topP ?? Config.DefaultTopP,
        };
    }

    public async Task<OpenRouterResponse> SendChatAsync(OpenRouterRequest request, CancellationToken ct = default)
    {
        ValidateRequest(request);

        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            using var requestMessage = CreateHttpRequest(request);
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(Config.Timeout);

            try
            {
                using var response = await httpClient.SendAsync(
                    requestMessage,
                    HttpCompletionOption.ResponseHeadersRead,
                    timeoutCts.Token);

                if (response.IsSuccessStatusCode)
                {
                    return await ParseResponseAsync(response, timeoutCts.Token);
                }

                if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                {
                    throw new InvalidOperationException("OpenRouter authorization failed.");
                }

                if (response.StatusCode == (HttpStatusCode)429)
                {
                    if (attempt < MaxAttempts)
                    {
                        await DelayWithBackoffAsync(attempt, ct);
                        continue;
                    }

                    throw new InvalidOperationException("OpenRouter rate limit exceeded.");
                }

                if ((int)response.StatusCode >= 500)
                {
                    if (attempt < MaxAttempts)
                    {
                        await DelayWithBackoffAsync(attempt, ct);
                        continue;
                    }

                    throw new InvalidOperationException("OpenRouter service error.");
                }

                var errorContent = await response.Content.ReadAsStringAsync(timeoutCts.Token);
                logger.LogWarning(
                    "OpenRouter request failed. Status: {StatusCode}. Response: {Response}",
                    response.StatusCode,
                    errorContent);

                throw new InvalidOperationException("OpenRouter request failed.");
            }
            catch (OperationCanceledException ex) when (!ct.IsCancellationRequested)
            {
                logger.LogWarning(ex, "OpenRouter request timed out.");

                if (attempt < MaxAttempts)
                {
                    await DelayWithBackoffAsync(attempt, ct);
                    continue;
                }

                throw new InvalidOperationException("OpenRouter request timed out.", ex);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "OpenRouter network error.");

                if (attempt < MaxAttempts)
                {
                    await DelayWithBackoffAsync(attempt, ct);
                    continue;
                }

                throw new InvalidOperationException("OpenRouter network error.", ex);
            }
        }

        throw new InvalidOperationException("OpenRouter request failed.");
    }

    private HttpRequestMessage CreateHttpRequest(OpenRouterRequest request)
    {
        var baseUrl = Config.Url.TrimEnd('/');
        var requestUri = $"{baseUrl}/chat/completions";

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Config.Key);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrWhiteSpace(referer))
        {
            requestMessage.Headers.TryAddWithoutValidation("HTTP-Referer", referer);
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            requestMessage.Headers.TryAddWithoutValidation("X-Title", title);
        }

        requestMessage.Content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");

        return requestMessage;
    }

    private async Task<OpenRouterResponse> ParseResponseAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.Content.Headers.ContentLength == 0)
        {
            throw new InvalidOperationException("OpenRouter returned empty response.");
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(ct);

        try
        {
            var parsed = await JsonSerializer.DeserializeAsync<OpenRouterResponse>(contentStream, JsonOptions, ct);
            if (parsed?.Choices is null || parsed.Choices.Count == 0)
            {
                throw new InvalidOperationException("OpenRouter response is missing choices.");
            }

            return parsed;
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Failed to parse OpenRouter response.");
            throw new InvalidOperationException("OpenRouter returned invalid JSON.", ex);
        }
    }

    private static void ValidateRequest(OpenRouterRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Model))
        {
            throw new InvalidOperationException("Model is required.");
        }

        if (request.Messages is null || request.Messages.Count == 0)
        {
            throw new InvalidOperationException("At least one message is required.");
        }

        foreach (var message in request.Messages)
        {
            if (message is null)
            {
                throw new InvalidOperationException("Message cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(message.Role) || string.IsNullOrWhiteSpace(message.Content))
            {
                throw new InvalidOperationException("Message role and content are required.");
            }

            if (message.Role is not ("system" or "user"))
            {
                throw new InvalidOperationException("Message role must be system or user.");
            }
        }

        if (request.ResponseFormat is null)
        {
            return;
        }

        if (!string.Equals(request.ResponseFormat.Type, "json_schema", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("response_format type must be json_schema.");
        }

        if (request.ResponseFormat.JsonSchema is null)
        {
            throw new InvalidOperationException("json_schema is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ResponseFormat.JsonSchema.Name))
        {
            throw new InvalidOperationException("json_schema name is required.");
        }

        var schema = request.ResponseFormat.JsonSchema.Schema;
        if (schema.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            throw new InvalidOperationException("json_schema schema is required.");
        }

        if (schema.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("json_schema schema must be a JSON object.");
        }
    }

    private static Task DelayWithBackoffAsync(int attempt, CancellationToken ct)
    {
        // Exponential backoff to avoid hammering the API during transient failures.
        var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
        return Task.Delay(delay, ct);
    }
}
