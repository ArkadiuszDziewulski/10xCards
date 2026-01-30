namespace _10xCards.Models;

public sealed class SourceInputViewModel
{
    public string Text { get; set; } = string.Empty;
    public int CharCount { get; set; }
    public string? ValidationMessage { get; set; }
}
