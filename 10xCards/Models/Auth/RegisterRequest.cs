using System.ComponentModel.DataAnnotations;

namespace _10xCards.Models.Auth;

public sealed class RegisterRequest
{
    [Required(ErrorMessage = "E-mail jest wymagany")]
    [EmailAddress(ErrorMessage = "Niepoprawny format e-maila")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Has³o jest wymagane")]
    [MinLength(8, ErrorMessage = "Has³o musi mieæ co najmniej 8 znaków")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Powtórzenie has³a jest wymagane")]
    [Compare(nameof(Password), ErrorMessage = "Has³a musz¹ byæ identyczne")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
