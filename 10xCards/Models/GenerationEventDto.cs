using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record GenerationEventDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("user_id")]
    public Guid UserId { get; init; }

    [JsonPropertyName("input_length")]
    public int InputLength { get; init; }

    [JsonPropertyName("total_generated_count")]
    public int TotalGeneratedCount { get; init; }

    [JsonPropertyName("accepted_count")]
    public int AcceptedCount { get; init; }

    [JsonPropertyName("target_deck_id")]
    public Guid? TargetDeckId { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}
