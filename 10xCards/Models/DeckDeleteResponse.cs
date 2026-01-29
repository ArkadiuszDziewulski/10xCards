using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record DeckDeleteResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; init; }

    [JsonPropertyName("deleted_at")]
    public DateTimeOffset DeletedAt { get; init; }
}
