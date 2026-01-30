using System.Net;
using System.Text.Json;
using _10xCards.Models;
using Supabase;
using Supabase.Functions;

namespace _10xCards.Services;

public sealed class FlashcardGenerationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly Supabase.Client supabase;
    private readonly ILogger<FlashcardGenerationService> logger;

    public FlashcardGenerationService(
        Supabase.Client supabase,
        ILogger<FlashcardGenerationService> logger)
    {
        this.supabase = supabase;
        this.logger = logger;
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

        var body = new Dictionary<string, object>
        {
            { "text", request.Text },
            { "amount", request.Amount }
        };
        var jsonBody = JsonSerializer.Serialize(body);
        try
        {
            var response = await supabase.Functions.Invoke<GenerateFlashcardsResponse>("generate-flashcards", jsonBody);
            if (response == null)
            {
                throw new FlashcardGenerationException("Empty response from generation endpoint.", HttpStatusCode.NoContent);
            }
            return response;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Error during function invoke: {Message}", ex.Message);
            throw new FlashcardGenerationException($"Error invoking Supabase function: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    private static string? TryExtractErrorMessage(string? errorContent)
    {
        if (string.IsNullOrWhiteSpace(errorContent))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(errorContent);
            if (document.RootElement.TryGetProperty("message", out var message) &&
                message.ValueKind == JsonValueKind.String)
            {
                return message.GetString();
            }
        }
        catch (JsonException)
        {
        }

        return null;
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
