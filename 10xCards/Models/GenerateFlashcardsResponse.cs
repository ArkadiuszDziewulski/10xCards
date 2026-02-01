using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record GenerateFlashcardsResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("flashcards")]
    public required IReadOnlyList<GeneratedFlashcardDto> Flashcards { get; init; }

    [JsonPropertyName("generationId")]
    public Guid? GenerationId { get; init; }
}
