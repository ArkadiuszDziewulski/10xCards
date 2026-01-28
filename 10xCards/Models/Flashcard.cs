using Postgrest.Attributes;
using Postgrest.Models;

namespace _10xCards.Models;

[Table("flashcards")]
public class Flashcard : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("front")]
    public string Front { get; set; } = string.Empty;

    [Column("back")]
    public string Back { get; set; } = string.Empty;

    [Column("deck_id")]
    public Guid DeckId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("status")]
    public string Status { get; set; } = "new";

    [Column("ease_factor")]
    public double EaseFactor { get; set; }

    [Column("interval")]
    public int Interval { get; set; }

    [Column("repetition_count")]
    public int RepetitionCount { get; set; }

    [Column("next_review_at")]
    public DateTime? NextReviewAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
