namespace _10xCards.Models;

public sealed class GenerateStateViewModel
{
    public string SourceText { get; set; } = string.Empty;
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? GenerationId { get; set; }
    public List<FlashcardProposalViewModel> Items { get; } = new();
    public int AcceptedCount { get; set; }
}
