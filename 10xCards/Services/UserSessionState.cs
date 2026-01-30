namespace _10xCards.Services;

public sealed class UserSessionState
{
    public bool IsAuthenticated { get; private set; }
    public string? UserEmail { get; private set; }
    public event Action? OnChange;

    public void SetAuthenticated(string? email)
    {
        UserEmail = email;
        IsAuthenticated = !string.IsNullOrWhiteSpace(email);
        NotifyStateChanged();
    }

    public void ClearSession()
    {
        UserEmail = null;
        IsAuthenticated = false;
        NotifyStateChanged();
    }

    public void UpdateFromSupabase(Supabase.Client supabaseClient)
    {
        if (supabaseClient is null)
        {
            ClearSession();
            return;
        }

        var session = supabaseClient.Auth.CurrentSession;
        if (session is null || string.IsNullOrWhiteSpace(session.AccessToken))
        {
            ClearSession();
            return;
        }

        var email = session.User?.Email ?? supabaseClient.Auth.CurrentUser?.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            SetAuthenticatedWithoutEmail();
            return;
        }

        SetAuthenticated(email);
    }

    private void SetAuthenticatedWithoutEmail()
    {
        UserEmail = null;
        IsAuthenticated = true;
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}
