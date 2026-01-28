using Postgrest.Attributes;
using Postgrest.Models;

namespace _10xCards.Models;

[Table("flashcards")]
public class Flashcard : BaseModel
{
    [PrimaryKey("id", false)]
    public string Id { get; set; } = null!;

    [Column("front")]
    public string Front { get; set; } = null!;

    [Column("back")]
    public string Back { get; set; } = null!;

    [Column("deck_id")]
    public string DeckId { get; set; } = null!;

    [Column("user_id")]
    public string UserId { get; set; } = null!;

    [Column("status")]
    public string Status { get; set; } = "new";

    [Column("ease_factor")]
    public float EaseFactor { get; set; }

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
