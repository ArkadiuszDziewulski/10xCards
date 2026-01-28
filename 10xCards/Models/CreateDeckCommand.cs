using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record CreateDeckCommand
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}
