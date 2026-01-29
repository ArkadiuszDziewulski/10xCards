using System.ComponentModel.DataAnnotations;

namespace _10xCards.Models;

public sealed class CreateDeckFormModel
{
    [Required(ErrorMessage = "Nazwa zestawu jest wymagana")]
    [StringLength(200, ErrorMessage = "Nazwa nie mo¿e przekraczaæ 200 znaków")]
    public string Name { get; set; } = string.Empty;
}
