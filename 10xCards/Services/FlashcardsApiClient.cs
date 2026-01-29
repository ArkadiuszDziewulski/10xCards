using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using _10xCards.Models;

namespace _10xCards.Services;

public sealed class FlashcardsApiClient
{
    private static readonly HashSet<string> AllowedSelectFields =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "*",
            "id",
            "deck_id",
            "user_id",
            "front",
            "back",
            "status",
            "next_review_at",
            "interval",
            "ease_factor",
            "repetition_count",
            "created_at",
            "updated_at",
        };

    private readonly HttpClient httpClient;
    private readonly ILogger<FlashcardsApiClient> logger;
    private readonly string supabaseUrl;
    private readonly JsonSerializerOptions jsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
        };

    public FlashcardsApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<FlashcardsApiClient> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        supabaseUrl = configuration["Supabase:Url"] ?? string.Empty;
    }

    public async Task<IReadOnlyList<FlashcardDto>> GetByDeckIdAsync(
        string accessToken,
        Guid deckId,
        string? select = "*",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new FlashcardsApiException("Access token is required.", HttpStatusCode.Unauthorized);
        }

        if (deckId == Guid.Empty)
        {
            throw new FlashcardsApiException("Deck id is required.", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(supabaseUrl))
        {
            throw new InvalidOperationException("Supabase Url is not configured.");
        }

        var normalizedSelect = NormalizeSelect(select);
        var requestUri = BuildRequestUri(deckId, normalizedSelect);

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new FlashcardsApiException("Unauthorized request.", response.StatusCode);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => "Invalid request parameters.",
                HttpStatusCode.Forbidden => "Access to flashcards is forbidden.",
                HttpStatusCode.NotFound => "Flashcards not found.",
                HttpStatusCode.InternalServerError => "Server error while loading flashcards.",
                _ => "Failed to load flashcards.",
            };

            logger.LogWarning(
                "Failed to load flashcards. Status: {StatusCode}. Response: {Response}",
                response.StatusCode,
                errorContent);

            throw new FlashcardsApiException(
                string.IsNullOrWhiteSpace(errorContent) ? message : $"{message} {errorContent}",
                response.StatusCode);
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var flashcards = await JsonSerializer.DeserializeAsync<List<FlashcardDto>>(contentStream, jsonOptions, cancellationToken)
            ?? new List<FlashcardDto>();

        return flashcards;
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

    private string BuildRequestUri(Guid deckId, string select)
    {
        var baseUrl = supabaseUrl.TrimEnd('/');
        var query =
            $"deck_id=eq.{Uri.EscapeDataString(deckId.ToString())}&select={Uri.EscapeDataString(select)}";
        return $"{baseUrl}/rest/v1/flashcards?{query}";
    }
}

public sealed class FlashcardsApiException : Exception
{
    public FlashcardsApiException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}
