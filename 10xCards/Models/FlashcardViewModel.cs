namespace _10xCards.Models;

public sealed class FlashcardViewModel
{
    public Guid Id { get; set; }
    public Guid DeckId { get; set; }
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
    public FlashcardStatus Status { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
