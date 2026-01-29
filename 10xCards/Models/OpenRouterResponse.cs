using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record OpenRouterResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("choices")]
    public IReadOnlyList<OpenRouterChoice>? Choices { get; init; }

    [JsonPropertyName("usage")]
    public OpenRouterUsage? Usage { get; init; }
}

public sealed record OpenRouterChoice
{
    [JsonPropertyName("index")]
    public int? Index { get; init; }

    [JsonPropertyName("message")]
    public OpenRouterMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

public sealed record OpenRouterUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int? PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public int? CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public int? TotalTokens { get; init; }
}
