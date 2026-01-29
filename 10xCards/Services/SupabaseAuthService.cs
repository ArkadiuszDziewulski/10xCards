using _10xCards.Models.Auth;

namespace _10xCards.Services;

public sealed class SupabaseAuthService
{
    private const string InvalidLoginCode = "invalid_login_credentials";
    private const string EmailNotConfirmedCode = "email_not_confirmed";
    private const string NetworkErrorCode = "network_error";
    private const string LoginFailedCode = "login_failed";
    private const string SignOutFailedCode = "signout_failed";
    private const string InvalidRequestCode = "invalid_request";

    private readonly Supabase.Client supabaseClient;
    private readonly UserSessionState userSessionState;
    private readonly ILogger<SupabaseAuthService> logger;
    private bool isInitialized;

    public SupabaseAuthService(
        Supabase.Client supabaseClient,
        UserSessionState userSessionState,
        ILogger<SupabaseAuthService> logger)
    {
        this.supabaseClient = supabaseClient;
        this.userSessionState = userSessionState;
        this.logger = logger;
    }

    public async Task<AuthResult> SignInAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return AuthResult.Failure(InvalidRequestCode, "Nieprawid³owe dane logowania.");
        }

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return AuthResult.Failure(InvalidRequestCode, "Podaj poprawny e-mail i has³o.");
        }

        await EnsureInitializedAsync(cancellationToken);

        try
        {
            var email = request.Email.Trim();
            var session = await supabaseClient.Auth.SignIn(email, request.Password);
            var userEmail = session?.User?.Email ?? supabaseClient.Auth.CurrentUser?.Email ?? email;

            userSessionState.SetAuthenticated(userEmail);
            return AuthResult.Success(userEmail);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Nie uda³o siê zalogowaæ u¿ytkownika.");
            var mappedError = MapError(ex);
            return AuthResult.Failure(mappedError.ErrorCode, mappedError.ErrorMessage);
        }
    }

    public async Task<AuthResult> SignOutAsync(CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);

        try
        {
            await supabaseClient.Auth.SignOut();
            userSessionState.ClearSession();
            return AuthResult.Success(null);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Nie uda³o siê wylogowaæ u¿ytkownika.");
            return AuthResult.Failure(SignOutFailedCode, "Nie uda³o siê wylogowaæ. Spróbuj ponownie.");
        }
    }

    public async Task EnsureInitializedAsync(CancellationToken cancellationToken = default)
    {
        if (isInitialized)
        {
            return;
        }

        await supabaseClient.InitializeAsync();
        userSessionState.UpdateFromSupabase(supabaseClient);
        isInitialized = true;
    }

    private static (string ErrorCode, string ErrorMessage) MapError(Exception exception)
    {
        var message = exception.Message ?? string.Empty;
        var normalized = message.ToLowerInvariant();

        if (normalized.Contains(InvalidLoginCode, StringComparison.OrdinalIgnoreCase))
        {
            return (InvalidLoginCode, "Nieprawid³owy e-mail lub has³o.");
        }

        if (normalized.Contains(EmailNotConfirmedCode, StringComparison.OrdinalIgnoreCase))
        {
            return (EmailNotConfirmedCode, "Konto wymaga potwierdzenia adresu e-mail.");
        }

        if (normalized.Contains("timeout", StringComparison.OrdinalIgnoreCase)
            || normalized.Contains("timed out", StringComparison.OrdinalIgnoreCase)
            || normalized.Contains("network", StringComparison.OrdinalIgnoreCase))
        {
            return (NetworkErrorCode, "Nie uda³o siê po³¹czyæ z us³ug¹. Spróbuj ponownie.");
        }

        return (LoginFailedCode, "Nie uda³o siê zalogowaæ. Spróbuj ponownie.");
    }
}
