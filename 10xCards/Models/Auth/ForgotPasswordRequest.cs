using System.ComponentModel.DataAnnotations;

namespace _10xCards.Models.Auth;

public sealed class ForgotPasswordRequest
{
    [Required(ErrorMessage = "E-mail jest wymagany")]
    [EmailAddress(ErrorMessage = "Niepoprawny format e-maila")]
    public string Email { get; set; } = string.Empty;
}
