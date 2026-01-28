using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record UpdateFlashcardContentCommand
{
    [JsonPropertyName("front")]
    public string? Front { get; init; }

    [JsonPropertyName("back")]
    public string? Back { get; init; }

    [JsonPropertyName("status")]
    public FlashcardStatus? Status { get; init; }
}
