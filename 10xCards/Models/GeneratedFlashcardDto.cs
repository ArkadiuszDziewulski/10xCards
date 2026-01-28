using System.Text.Json.Serialization;

namespace _10xCards.Models;

// Subset of flashcards table fields returned by the AI generation endpoint.
public sealed record GeneratedFlashcardDto
{
    [JsonPropertyName("front")]
    public required string Front { get; init; }

    [JsonPropertyName("back")]
    public required string Back { get; init; }
}
