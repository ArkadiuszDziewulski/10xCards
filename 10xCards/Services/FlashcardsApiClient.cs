using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
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

        if (!TryGetUserIdFromAccessToken(accessToken, out var userId))
        {
            throw new FlashcardsApiException("Invalid access token.", HttpStatusCode.Unauthorized);
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
                UserId = userId,
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

    public async Task UpdateContentAsync(
        string accessToken,
        Guid flashcardId,
        UpdateFlashcardContentCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new FlashcardsApiException("Access token is required.", HttpStatusCode.Unauthorized);
        }

        if (flashcardId == Guid.Empty)
        {
            throw new FlashcardsApiException("Flashcard id is required.", HttpStatusCode.BadRequest);
        }

        if (command is null)
        {
            throw new FlashcardsApiException("Request body is required.", HttpStatusCode.BadRequest);
        }

        string? trimmedFront = null;
        string? trimmedBack = null;
        FlashcardStatus? status = null;
        var hasChanges = false;

        if (command.Front is not null)
        {
            trimmedFront = command.Front.Trim();
            if (string.IsNullOrWhiteSpace(trimmedFront))
            {
                throw new FlashcardsApiException("Front content is required.", HttpStatusCode.BadRequest);
            }

            hasChanges = true;
        }

        if (command.Back is not null)
        {
            trimmedBack = command.Back.Trim();
            if (string.IsNullOrWhiteSpace(trimmedBack))
            {
                throw new FlashcardsApiException("Back content is required.", HttpStatusCode.BadRequest);
            }

            hasChanges = true;
        }

        if (command.Status is not null)
        {
            if (!Enum.IsDefined(command.Status.Value))
            {
                throw new FlashcardsApiException("Status must be either active or draft.", HttpStatusCode.BadRequest);
            }

            status = command.Status;
            hasChanges = true;
        }

        if (!hasChanges)
        {
            throw new FlashcardsApiException("At least one field must be provided.", HttpStatusCode.BadRequest);
        }

        var sanitizedCommand = new UpdateFlashcardContentCommand
        {
            Front = trimmedFront,
            Back = trimmedBack,
            Status = status,
        };

        if (string.IsNullOrWhiteSpace(supabaseUrl))
        {
            throw new InvalidOperationException("Supabase Url is not configured.");
        }

        var baseUrl = supabaseUrl.TrimEnd('/');
        var requestUri =
            $"{baseUrl}/rest/v1/flashcards?id=eq.{Uri.EscapeDataString(flashcardId.ToString())}";

        using var requestMessage = new HttpRequestMessage(HttpMethod.Patch, requestUri);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        requestMessage.Headers.TryAddWithoutValidation("Prefer", "return=minimal");
        requestMessage.Content = new StringContent(
            JsonSerializer.Serialize(sanitizedCommand, jsonOptions),
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
                HttpStatusCode.BadRequest => "Invalid flashcard update payload.",
                HttpStatusCode.Forbidden => "Access to flashcards is forbidden.",
                HttpStatusCode.NotFound => "Flashcard not found.",
                HttpStatusCode.InternalServerError => "Server error while updating flashcard.",
                _ => "Failed to update flashcard.",
            };

            logger.LogWarning(
                "Failed to update flashcard content. Status: {StatusCode}. Response: {Response}",
                response.StatusCode,
                errorContent);

            throw new FlashcardsApiException(
                string.IsNullOrWhiteSpace(errorContent) ? message : $"{message} {errorContent}",
                response.StatusCode);
        }
    }

    public async Task DeleteAsync(
        string accessToken,
        Guid flashcardId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new FlashcardsApiException("Access token is required.", HttpStatusCode.Unauthorized);
        }

        if (flashcardId == Guid.Empty)
        {
            throw new FlashcardsApiException("Flashcard id is required.", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(supabaseUrl))
        {
            throw new InvalidOperationException("Supabase Url is not configured.");
        }

        var baseUrl = supabaseUrl.TrimEnd('/');
        var requestUri =
            $"{baseUrl}/rest/v1/flashcards?id=eq.{Uri.EscapeDataString(flashcardId.ToString())}";

        using var requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        requestMessage.Headers.TryAddWithoutValidation("Prefer", "return=minimal");

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
                HttpStatusCode.BadRequest => "Invalid flashcard identifier.",
                HttpStatusCode.Forbidden => "Access to flashcards is forbidden.",
                HttpStatusCode.NotFound => "Flashcard not found.",
                HttpStatusCode.InternalServerError => "Server error while deleting flashcard.",
                _ => "Failed to delete flashcard.",
            };

            logger.LogWarning(
                "Failed to delete flashcard. Status: {StatusCode}. Response: {Response}",
                response.StatusCode,
                errorContent);

            throw new FlashcardsApiException(
                string.IsNullOrWhiteSpace(errorContent) ? message : $"{message} {errorContent}",
                response.StatusCode);
        }
    }

    public async Task UpdateSrsAsync(
        string accessToken,
        Guid flashcardId,
        UpdateFlashcardSrsCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new FlashcardsApiException("Access token is required.", HttpStatusCode.Unauthorized);
        }

        if (flashcardId == Guid.Empty)
        {
            throw new FlashcardsApiException("Flashcard id is required.", HttpStatusCode.BadRequest);
        }

        if (command is null)
        {
            throw new FlashcardsApiException("Request body is required.", HttpStatusCode.BadRequest);
        }

        if (command.NextReviewAt is null)
        {
            throw new FlashcardsApiException("Next review date is required.", HttpStatusCode.BadRequest);
        }

        if (command.Interval is null || command.Interval < 0)
        {
            throw new FlashcardsApiException("Interval must be non-negative.", HttpStatusCode.BadRequest);
        }

        if (command.EaseFactor is null || command.EaseFactor <= 0)
        {
            throw new FlashcardsApiException("Ease factor must be greater than zero.", HttpStatusCode.BadRequest);
        }

        if (command.RepetitionCount is null || command.RepetitionCount < 0)
        {
            throw new FlashcardsApiException("Repetition count must be non-negative.", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(supabaseUrl))
        {
            throw new InvalidOperationException("Supabase Url is not configured.");
        }

        var requestPayload = new FlashcardSrsUpdateRequest
        {
            NextReviewAt = command.NextReviewAt.Value,
            Interval = command.Interval.Value,
            EaseFactor = command.EaseFactor.Value,
            RepetitionCount = command.RepetitionCount.Value,
        };

        var baseUrl = supabaseUrl.TrimEnd('/');
        var requestUri =
            $"{baseUrl}/rest/v1/flashcards?id=eq.{Uri.EscapeDataString(flashcardId.ToString())}";

        using var requestMessage = new HttpRequestMessage(HttpMethod.Patch, requestUri);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        requestMessage.Headers.TryAddWithoutValidation("Prefer", "return=minimal");
        requestMessage.Content = new StringContent(
            JsonSerializer.Serialize(requestPayload, jsonOptions),
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
                HttpStatusCode.BadRequest => "Invalid flashcard update payload.",
                HttpStatusCode.Forbidden => "Access to flashcards is forbidden.",
                HttpStatusCode.NotFound => "Flashcard not found.",
                HttpStatusCode.InternalServerError => "Server error while updating flashcard.",
                _ => "Failed to update flashcard.",
            };

            logger.LogWarning(
                "Failed to update flashcard SRS. Status: {StatusCode}. Response: {Response}",
                response.StatusCode,
                errorContent);

            throw new FlashcardsApiException(
                string.IsNullOrWhiteSpace(errorContent) ? message : $"{message} {errorContent}",
                response.StatusCode);
        }
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

    private static bool TryGetUserIdFromAccessToken(string accessToken, out Guid userId)
    {
        userId = Guid.Empty;

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return false;
        }

        var parts = accessToken.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length < 2)
        {
            return false;
        }

        try
        {
            var payloadBytes = DecodeBase64Url(parts[1]);
            using var payloadJson = JsonDocument.Parse(payloadBytes);
            if (TryGetGuidClaim(payloadJson.RootElement, "sub", out userId))
            {
                return true;
            }

            return TryGetGuidClaim(payloadJson.RootElement, "user_id", out userId);
        }
        catch (FormatException)
        {
            return false;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryGetGuidClaim(JsonElement payload, string claimName, out Guid userId)
    {
        userId = Guid.Empty;

        if (!payload.TryGetProperty(claimName, out var claimValue))
        {
            return false;
        }

        if (claimValue.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        var claimText = claimValue.GetString();
        return Guid.TryParse(claimText, out userId);
    }

    private static byte[] DecodeBase64Url(string value)
    {
        var normalized = value.Replace('-', '+').Replace('_', '/');
        var paddingNeeded = normalized.Length % 4;
        if (paddingNeeded > 0)
        {
            normalized = normalized.PadRight(normalized.Length + (4 - paddingNeeded), '=');
        }

        return Convert.FromBase64String(normalized);
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
