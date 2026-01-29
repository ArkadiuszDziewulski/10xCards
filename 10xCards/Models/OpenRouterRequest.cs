using System.Text.Json;
using System.Text.Json.Serialization;

namespace _10xCards.Models;

public sealed record OpenRouterRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("messages")]
    public required IReadOnlyList<OpenRouterMessage> Messages { get; init; }

    [JsonPropertyName("response_format")]
    public OpenRouterResponseFormat? ResponseFormat { get; init; }

    [JsonPropertyName("temperature")]
    public decimal? Temperature { get; init; }

    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; init; }

    [JsonPropertyName("top_p")]
    public decimal? TopP { get; init; }
}

public sealed record OpenRouterResponseFormat
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("json_schema")]
    public required OpenRouterJsonSchema JsonSchema { get; init; }
}

public sealed record OpenRouterJsonSchema
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("strict")]
    public bool Strict { get; init; }

    [JsonPropertyName("schema")]
    public required JsonElement Schema { get; init; }
}
