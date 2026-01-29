namespace _10xCards.Services;

public sealed record OpenRouterConfig
{
    public required string Url { get; init; }

    public required string Key { get; init; }

    public string DefaultModel { get; init; } = "openai/gpt-4.1";

    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);

    public decimal DefaultTemperature { get; init; } = 0.2m;

    public int DefaultMaxTokens { get; init; } = 400;

    public decimal DefaultTopP { get; init; } = 0.9m;
}
