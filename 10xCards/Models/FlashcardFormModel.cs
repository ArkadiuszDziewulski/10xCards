using System.ComponentModel.DataAnnotations;

namespace _10xCards.Models;

public sealed class FlashcardFormModel
{
    [Required(ErrorMessage = "Przód fiszki jest wymagany")]
    public string Front { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ty³ fiszki jest wymagany")]
    public string Back { get; set; } = string.Empty;

    public FlashcardStatus? Status { get; set; }
}
