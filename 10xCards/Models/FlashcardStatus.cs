using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace _10xCards.Models;

// String values align with the PostgREST "status" column values in the flashcards table.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FlashcardStatus
{
    [EnumMember(Value = "active")]
    Active,
    [EnumMember(Value = "draft")]
    Draft
}
