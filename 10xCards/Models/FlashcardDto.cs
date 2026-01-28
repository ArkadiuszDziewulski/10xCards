using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record FlashcardDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("deck_id")]
    public Guid DeckId { get; init; }

    [JsonPropertyName("user_id")]
    public Guid UserId { get; init; }

    [JsonPropertyName("front")]
    public required string Front { get; init; }

    [JsonPropertyName("back")]
    public required string Back { get; init; }

    [JsonPropertyName("status")]
    public FlashcardStatus Status { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; init; }

    [JsonPropertyName("next_review_at")]
    public DateTimeOffset? NextReviewAt { get; init; }

    [JsonPropertyName("interval")]
    public int Interval { get; init; }

    [JsonPropertyName("ease_factor")]
    public double EaseFactor { get; init; }

    [JsonPropertyName("repetition_count")]
    public int RepetitionCount { get; init; }
}
