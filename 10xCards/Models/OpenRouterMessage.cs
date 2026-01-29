using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record OpenRouterMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    public required string Content { get; init; }
}
