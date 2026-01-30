namespace _10xCards.Models;

public sealed class DeckSelectionViewModel
{
    public DeckSelectionMode Mode { get; set; } = DeckSelectionMode.Existing;
    public Guid? SelectedDeckId { get; set; }
    public string? NewDeckName { get; set; }
}
