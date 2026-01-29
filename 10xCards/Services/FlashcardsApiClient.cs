using System.Net;
using System.Net.Http.Headers;
using System.Text;
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

    public async Task<IReadOnlyList<FlashcardDto>> CreateAsync(
        string accessToken,
        CreateFlashcardsCommand command,
        bool returnRepresentation = true,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new FlashcardsApiException("Access token is required.", HttpStatusCode.Unauthorized);
        }

        if (command is null)
        {
            throw new FlashcardsApiException("Request body is required.", HttpStatusCode.BadRequest);
        }

        if (command.Flashcards is null || command.Flashcards.Count == 0)
        {
            throw new FlashcardsApiException("At least one flashcard is required.", HttpStatusCode.BadRequest);
        }

        var sanitizedFlashcards = new List<FlashcardCreateRequest>(command.Flashcards.Count);
        foreach (var flashcard in command.Flashcards)
        {
            if (flashcard is null)
            {
                throw new FlashcardsApiException("Flashcard payload cannot be null.", HttpStatusCode.BadRequest);
            }

            if (flashcard.DeckId == Guid.Empty)
            {
                throw new FlashcardsApiException("Deck id is required.", HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(flashcard.Front))
            {
                throw new FlashcardsApiException("Front content is required.", HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(flashcard.Back))
            {
                throw new FlashcardsApiException("Back content is required.", HttpStatusCode.BadRequest);
            }

            if (flashcard.Status is not null && !Enum.IsDefined(flashcard.Status.Value))
            {
                throw new FlashcardsApiException("Status must be either active or draft.", HttpStatusCode.BadRequest);
            }

            sanitizedFlashcards.Add(new FlashcardCreateRequest
            {
                DeckId = flashcard.DeckId,
                Front = flashcard.Front.Trim(),
                Back = flashcard.Back.Trim(),
                Status = flashcard.Status,
            });
        }

        if (string.IsNullOrWhiteSpace(supabaseUrl))
        {
            throw new InvalidOperationException("Supabase Url is not configured.");
        }

        var baseUrl = supabaseUrl.TrimEnd('/');
        var requestUri = $"{baseUrl}/rest/v1/flashcards";

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        requestMessage.Headers.TryAddWithoutValidation(
            "Prefer",
            returnRepresentation ? "return=representation" : "return=minimal");
        requestMessage.Content = new StringContent(
            JsonSerializer.Serialize(sanitizedFlashcards, jsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await httpClient.SendAsync(requestMessage, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new FlashcardsApiException("Unauthorized request.", response.StatusCode);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => "Invalid flashcard payload.",
                HttpStatusCode.Forbidden => "Access to flashcards is forbidden.",
                HttpStatusCode.NotFound => "Deck not found.",
                HttpStatusCode.InternalServerError => "Server error while creating flashcards.",
                _ => "Failed to create flashcards.",
            };

            logger.LogWarning(
                "Failed to create flashcards. Status: {StatusCode}. Response: {Response}",
                response.StatusCode,
                errorContent);

            throw new FlashcardsApiException(
                string.IsNullOrWhiteSpace(errorContent) ? message : $"{message} {errorContent}",
                response.StatusCode);
        }

        if (!returnRepresentation || response.Content.Headers.ContentLength == 0)
        {
            return Array.Empty<FlashcardDto>();
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var createdFlashcards = await JsonSerializer.DeserializeAsync<List<FlashcardDto>>(contentStream, jsonOptions, cancellationToken)
            ?? new List<FlashcardDto>();

        if (createdFlashcards.Count == 0)
        {
            logger.LogWarning("Supabase returned empty flashcard response for create request.");
            throw new FlashcardsApiException("Server returned empty flashcard response.", HttpStatusCode.InternalServerError);
        }

        return createdFlashcards;
    }

    public async Task<IReadOnlyList<FlashcardDto>> GetDueFlashcardsAsync(
        string accessToken,
        string? select = "*",
        int? limit = null,
        int? offset = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new FlashcardsApiException("Access token is required.", HttpStatusCode.Unauthorized);
        }

        if (string.IsNullOrWhiteSpace(supabaseUrl))
        {
            throw new InvalidOperationException("Supabase Url is not configured.");
        }

        if (limit is < 0)
        {
            throw new FlashcardsApiException("Limit must be non-negative.", HttpStatusCode.BadRequest);
        }

        if (offset is < 0)
        {
            throw new FlashcardsApiException("Offset must be non-negative.", HttpStatusCode.BadRequest);
        }

        var normalizedSelect = NormalizeSelect(select);
        var requestUri = BuildDueRequestUri(normalizedSelect, limit, offset);

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
                "Failed to load due flashcards. Status: {StatusCode}. Response: {Response}",
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

    private string BuildDueRequestUri(string select, int? limit, int? offset)
    {
        var baseUrl = supabaseUrl.TrimEnd('/');
        var queryParts = new List<string>
        {
            "next_review_at=lte.now()",
            "status=eq.active",
            $"select={Uri.EscapeDataString(select)}",
        };

        if (limit is not null)
        {
            queryParts.Add($"limit={limit.Value}");
        }

        if (offset is not null)
        {
            queryParts.Add($"offset={offset.Value}");
        }

        return $"{baseUrl}/rest/v1/flashcards?{string.Join('&', queryParts)}";
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
