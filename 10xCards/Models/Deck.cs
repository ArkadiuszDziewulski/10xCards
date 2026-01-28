using Postgrest.Attributes;
using Postgrest.Models;

namespace _10xCards.Models;

[Table("decks")]
public class Deck : BaseModel
{
    [PrimaryKey("id", false)]
    public string Id { get; set; } = null!;

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("user_id")]
    public string UserId { get; set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
