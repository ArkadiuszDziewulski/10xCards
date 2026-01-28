using Postgrest.Attributes;
using Postgrest.Models;

namespace _10xCards.Models;

[Table("generation_events")]
public class GenerationEvent : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("target_deck_id")]
    public Guid? TargetDeckId { get; set; }

    [Column("input_length")]
    public int InputLength { get; set; }

    [Column("total_generated_count")]
    public int TotalGeneratedCount { get; set; }

    [Column("accepted_count")]
    public int AcceptedCount { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
