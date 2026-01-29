using System.ComponentModel.DataAnnotations;

namespace _10xCards.Models.Auth;

public sealed class ResetPasswordRequest
{
    [Required(ErrorMessage = "Has³o jest wymagane")]
    [MinLength(8, ErrorMessage = "Has³o musi mieæ co najmniej 8 znaków")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Powtórzenie has³a jest wymagane")]
    [Compare(nameof(Password), ErrorMessage = "Has³a musz¹ byæ identyczne")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Token jest wymagany")]
    public string AccessToken { get; set; } = string.Empty;
}
