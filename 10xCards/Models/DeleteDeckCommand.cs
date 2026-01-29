using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record DeleteDeckCommand
{
    [JsonPropertyName("id")]
    public Guid DeckId { get; init; }

    [JsonPropertyName("user_id")]
    public Guid UserId { get; init; }
}
