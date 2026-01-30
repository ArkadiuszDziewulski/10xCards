using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using _10xCards.Models;

namespace _10xCards.Services;

public sealed class FlashcardGenerationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient httpClient;
    private readonly ILogger<FlashcardGenerationService> logger;
    private readonly string supabaseUrl;

    public FlashcardGenerationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<FlashcardGenerationService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        supabaseUrl = configuration["Supabase:Url"] ?? string.Empty;
    }

    public async Task<GenerateFlashcardsResponse> GenerateAsync(
        string accessToken,
        GenerateFlashcardsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new FlashcardGenerationException("Access token is required.", HttpStatusCode.Unauthorized);
        }

        if (request is null)
        {
            throw new FlashcardGenerationException("Request payload is required.", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(request.Text))
        {
            throw new FlashcardGenerationException("Source text is required.", HttpStatusCode.BadRequest);
        }

        if (request.Amount <= 0)
        {
            throw new FlashcardGenerationException("Amount must be greater than zero.", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(supabaseUrl))
        {
            throw new InvalidOperationException("Supabase Url is not configured.");
        }

        var baseUrl = supabaseUrl.TrimEnd('/');
        var requestUri = $"{baseUrl}/functions/v1/generate-flashcards";

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        requestMessage.Content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await httpClient.SendAsync(requestMessage, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new FlashcardGenerationException("Unauthorized request.", response.StatusCode);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = response.StatusCode switch
            {
                (HttpStatusCode)429 => "Rate limit exceeded.",
                HttpStatusCode.BadRequest => "Invalid generation request.",
                HttpStatusCode.Forbidden => "Access to generation endpoint is forbidden.",
                HttpStatusCode.InternalServerError => "Server error while generating flashcards.",
                _ => "Failed to generate flashcards.",
            };

            logger.LogWarning(
                "Failed to generate flashcards. Status: {StatusCode}. Response: {Response}",
                response.StatusCode,
                errorContent);

            throw new FlashcardGenerationException(
                string.IsNullOrWhiteSpace(errorContent) ? message : $"{message} {errorContent}",
                response.StatusCode);
        }

        if (response.Content.Headers.ContentLength == 0)
        {
            throw new FlashcardGenerationException("Empty response from generation endpoint.", HttpStatusCode.NoContent);
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var parsed = await JsonSerializer.DeserializeAsync<GenerateFlashcardsResponse>(contentStream, JsonOptions, cancellationToken);
        if (parsed is null)
        {
            throw new FlashcardGenerationException("Invalid response from generation endpoint.", HttpStatusCode.InternalServerError);
        }

        return parsed;
    }
}

public sealed class FlashcardGenerationException : Exception
{
    public FlashcardGenerationException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}
