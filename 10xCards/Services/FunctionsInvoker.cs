using Supabase;
using Supabase.Functions;

namespace _10xCards.Services;

public interface IFunctionsInvoker
{
    Task<T?> InvokeAsync<T>(
        string name,
        Supabase.Functions.Client.InvokeFunctionOptions options,
        CancellationToken cancellationToken = default)
        where T : class;
}

public sealed class SupabaseFunctionsInvoker : IFunctionsInvoker
{
    private readonly Supabase.Client supabase;

    public SupabaseFunctionsInvoker(Supabase.Client supabase)
    {
        this.supabase = supabase;
    }

    public Task<T?> InvokeAsync<T>(
        string name,
        Supabase.Functions.Client.InvokeFunctionOptions options,
        CancellationToken cancellationToken = default)
        where T : class
    {
        return supabase.Functions.Invoke<T>(name, options: options);
    }
}
