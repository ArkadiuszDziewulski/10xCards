using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using _10xCards.Models;

namespace _10xCards.Services;

public sealed class DecksApiClient
{
    private const int MaxDeckNameLength = 200;
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
    private readonly ILogger<DecksApiClient> logger;
    private readonly string supabaseUrl;
    private readonly JsonSerializerOptions jsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
        };

    public DecksApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<DecksApiClient> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
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

    public async Task<DeckDto> CreateDeckAsync(
        string accessToken,
        CreateDeckRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new DecksApiException("Access token is required.", HttpStatusCode.Unauthorized);
        }

        if (request is null)
        {
            throw new DecksApiException("Request body is required.", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new DecksApiException("Deck name is required.", HttpStatusCode.BadRequest);
        }

        var trimmedName = request.Name.Trim();
        if (trimmedName.Length > MaxDeckNameLength)
        {
            throw new DecksApiException(
                $"Deck name must be at most {MaxDeckNameLength} characters long.",
                HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(supabaseUrl))
        {
            throw new InvalidOperationException("Supabase Url is not configured.");
        }

        if (!TryGetUserIdFromAccessToken(accessToken, out var userId))
        {
            throw new DecksApiException("Invalid access token.", HttpStatusCode.Unauthorized);
        }

        var command = new CreateDeckCommand
        {
            UserId = userId,
            Name = trimmedName,
        };

        var baseUrl = supabaseUrl.TrimEnd('/');
        var requestUri = $"{baseUrl}/rest/v1/decks";

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        requestMessage.Headers.TryAddWithoutValidation("Prefer", "return=representation");
        requestMessage.Content = new StringContent(
            JsonSerializer.Serialize(command, jsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await httpClient.SendAsync(requestMessage, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new DecksApiException("Unauthorized request.", response.StatusCode);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            if (IsUniqueConstraintViolation(response.StatusCode, errorContent))
            {
                throw new DecksApiException("Deck name already exists.", HttpStatusCode.Conflict);
            }

            var message = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => "Invalid request payload.",
                HttpStatusCode.Forbidden => "Access to decks is forbidden.",
                HttpStatusCode.InternalServerError => "Server error while creating deck.",
                _ => "Failed to create deck.",
            };

            logger.LogWarning(
                "Failed to create deck. Status: {StatusCode}. Response: {Response}",
                response.StatusCode,
                errorContent);

            throw new DecksApiException(
                string.IsNullOrWhiteSpace(errorContent) ? message : $"{message} {errorContent}",
                response.StatusCode);
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var createdDecks = await JsonSerializer.DeserializeAsync<List<DeckDto>>(contentStream, jsonOptions, cancellationToken)
            ?? new List<DeckDto>();

        if (createdDecks.Count == 0)
        {
            logger.LogWarning("Supabase returned empty deck response for create request.");
            throw new DecksApiException("Server returned empty deck response.", HttpStatusCode.InternalServerError);
        }

        return createdDecks[0];
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

    private string BuildRequestUri(string select, string order)
    {
        var baseUrl = supabaseUrl.TrimEnd('/');
        var query = $"select={Uri.EscapeDataString(select)}&order={Uri.EscapeDataString(order)}";
        return $"{baseUrl}/rest/v1/decks?{query}";
    }

    private static bool IsUniqueConstraintViolation(HttpStatusCode statusCode, string? content)
    {
        if (statusCode == HttpStatusCode.Conflict)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        return content.Contains("23505", StringComparison.OrdinalIgnoreCase);
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
