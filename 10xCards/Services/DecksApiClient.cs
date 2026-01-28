using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using _10xCards.Models;

namespace _10xCards.Services;

public sealed class DecksApiClient
{
    private static readonly HashSet<string> AllowedSelectFields =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "*",
            "id",
            "user_id",
            "name",
            "created_at",
            "updated_at",
        };

    private static readonly HashSet<string> AllowedOrderFields =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "created_at",
        };

    private readonly HttpClient httpClient;
    private readonly string supabaseUrl;
    private readonly JsonSerializerOptions jsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
        };

    public DecksApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        this.httpClient = httpClient;
        supabaseUrl = configuration["Supabase:Url"] ?? string.Empty;
    }

    public async Task<IReadOnlyList<DeckDto>> GetDecksAsync(
        string accessToken,
        string? select = "*",
        string? order = "created_at.desc",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new ArgumentException("Access token is required.", nameof(accessToken));
        }

        if (string.IsNullOrWhiteSpace(supabaseUrl))
        {
            throw new InvalidOperationException("Supabase Url is not configured.");
        }

        var normalizedSelect = NormalizeSelect(select);
        var normalizedOrder = NormalizeOrder(order);
        var requestUri = BuildRequestUri(normalizedSelect, normalizedOrder);

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new DecksApiException("Unauthorized request.", response.StatusCode);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => "Invalid request parameters.",
                HttpStatusCode.Forbidden => "Access to decks is forbidden.",
                HttpStatusCode.InternalServerError => "Server error while loading decks.",
                _ => "Failed to load decks.",
            };
            throw new DecksApiException(
                string.IsNullOrWhiteSpace(errorContent) ? message : $"{message} {errorContent}",
                response.StatusCode);
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var decks = await JsonSerializer.DeserializeAsync<List<DeckDto>>(contentStream, jsonOptions, cancellationToken)
            ?? new List<DeckDto>();

        return decks;
    }

    private static string NormalizeSelect(string? select)
    {
        if (string.IsNullOrWhiteSpace(select))
        {
            return "*";
        }

        var fields = select.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (fields.Length == 1 && fields[0] == "*")
        {
            return "*";
        }

        foreach (var field in fields)
        {
            if (!AllowedSelectFields.Contains(field))
            {
                throw new ArgumentException($"Select field '{field}' is not allowed.", nameof(select));
            }
        }

        return string.Join(',', fields);
    }

    private static string NormalizeOrder(string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
        {
            return "created_at.desc";
        }

        var parts = order.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            throw new ArgumentException("Order must be in format 'created_at.asc' or 'created_at.desc'.", nameof(order));
        }

        if (!AllowedOrderFields.Contains(parts[0]) || (parts[1] != "asc" && parts[1] != "desc"))
        {
            throw new ArgumentException("Order must be in format 'created_at.asc' or 'created_at.desc'.", nameof(order));
        }

        return $"{parts[0]}.{parts[1]}";
    }

    private string BuildRequestUri(string select, string order)
    {
        var baseUrl = supabaseUrl.TrimEnd('/');
        var query = $"select={Uri.EscapeDataString(select)}&order={Uri.EscapeDataString(order)}";
        return $"{baseUrl}/rest/v1/decks?{query}";
    }
}

public sealed class DecksApiException : Exception
{
    public DecksApiException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}
