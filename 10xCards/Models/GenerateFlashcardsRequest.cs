using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record GenerateFlashcardsRequest
{
    [JsonPropertyName("text")]
    public required string Text { get; init; }

    [JsonPropertyName("amount")]
    public int Amount { get; init; }
}
