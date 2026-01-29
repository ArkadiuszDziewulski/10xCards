using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record FlashcardSrsUpdateRequest
{
    [JsonPropertyName("next_review_at")]
    public DateTimeOffset NextReviewAt { get; init; }

    [JsonPropertyName("interval")]
    public int Interval { get; init; }

    [JsonPropertyName("ease_factor")]
    public double EaseFactor { get; init; }

    [JsonPropertyName("repetition_count")]
    public int RepetitionCount { get; init; }
}
