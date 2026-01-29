using System.ComponentModel.DataAnnotations;

namespace _10xCards.Models.Auth;

public sealed class LoginRequest
{
    [Required(ErrorMessage = "E-mail jest wymagany")]
    [EmailAddress(ErrorMessage = "Niepoprawny format e-maila")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Has³o jest wymagane")]
    [MinLength(8, ErrorMessage = "Has³o musi mieæ co najmniej 8 znaków")]
    public string Password { get; set; } = string.Empty;
}
