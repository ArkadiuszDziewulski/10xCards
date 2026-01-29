namespace _10xCards.Models.Auth;

public sealed class AuthResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
    public string? UserEmail { get; init; }

    public static AuthResult Success(string? userEmail)
    {
        return new AuthResult
        {
            IsSuccess = true,
            UserEmail = userEmail,
        };
    }

    public static AuthResult Failure(string errorCode, string errorMessage)
    {
        return new AuthResult
        {
            IsSuccess = false,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage,
        };
    }
}
