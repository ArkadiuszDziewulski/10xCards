namespace _10xCards.Models;

public sealed record DeckOptionDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
}
