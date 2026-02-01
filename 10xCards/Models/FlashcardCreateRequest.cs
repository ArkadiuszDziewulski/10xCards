using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record FlashcardCreateRequest
{
    [JsonPropertyName("deck_id")]
    public Guid DeckId { get; init; }

    [JsonPropertyName("user_id")]
    public Guid UserId { get; init; }

    [JsonPropertyName("front")]
    public required string Front { get; init; }

    [JsonPropertyName("back")]
    public required string Back { get; init; }

    [JsonPropertyName("status")]
    public FlashcardStatus? Status { get; init; }
}
