namespace _10xCards.Models;

public sealed class FlashcardProposalViewModel
{
    public Guid Id { get; set; }
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
    public bool IsAccepted { get; set; }
    public bool IsEditing { get; set; }
    public string? ValidationMessage { get; set; }
}
