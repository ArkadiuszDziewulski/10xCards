namespace _10xCards.Models;

public sealed record GetFlashcardsQuery
{
    public Guid DeckId { get; init; }
}
