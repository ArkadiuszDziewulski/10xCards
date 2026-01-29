namespace _10xCards.Models;

public sealed class DeckViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string EditUrl => $"/decks/{Id}";
    public string StudyUrl => $"/study/{Id}";
}
