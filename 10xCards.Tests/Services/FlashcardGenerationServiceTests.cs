using System.Net;
using _10xCards.Models;
using _10xCards.Services;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace _10xCards.Tests.Services;

public sealed class FlashcardGenerationServiceTests
{
    private sealed class FakeFunctionsInvoker : IFunctionsInvoker
    {
        private readonly Func<string, Supabase.Functions.Client.InvokeFunctionOptions, CancellationToken, Task<object?>> handler;

        public FakeFunctionsInvoker(Func<string, Supabase.Functions.Client.InvokeFunctionOptions, CancellationToken, Task<object?>> handler)
        {
            this.handler = handler;
        }

        public async Task<T?> InvokeAsync<T>(
            string name,
            Supabase.Functions.Client.InvokeFunctionOptions options,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var result = await handler(name, options, cancellationToken);
            return (T?)result;
        }
    }

    [Test]
    public void GenerateAsync_WhenAccessTokenMissing_ThrowsUnauthorized()
    {
        var service = CreateService((_, _, _) => Task.FromResult<object?>(null));

        var ex = Assert.ThrowsAsync<FlashcardGenerationException>(
            () => service.GenerateAsync(" ", new GenerateFlashcardsRequest { Text = "text", Amount = 1 }));

        Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That(ex.Message, Is.EqualTo("Access token is required."));
    }

    [Test]
    public void GenerateAsync_WhenRequestMissing_ThrowsBadRequest()
    {
        var service = CreateService((_, _, _) => Task.FromResult<object?>(null));

        var ex = Assert.ThrowsAsync<FlashcardGenerationException>(
            () => service.GenerateAsync("token", null!));

        Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(ex.Message, Is.EqualTo("Request payload is required."));
    }

    [Test]
    public void GenerateAsync_WhenTextMissing_ThrowsBadRequest()
    {
        var service = CreateService((_, _, _) => Task.FromResult<object?>(null));

        var ex = Assert.ThrowsAsync<FlashcardGenerationException>(
            () => service.GenerateAsync("token", new GenerateFlashcardsRequest { Text = " ", Amount = 1 }));

        Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(ex.Message, Is.EqualTo("Source text is required."));
    }

    [Test]
    public void GenerateAsync_WhenAmountInvalid_ThrowsBadRequest()
    {
        var service = CreateService((_, _, _) => Task.FromResult<object?>(null));

        var ex = Assert.ThrowsAsync<FlashcardGenerationException>(
            () => service.GenerateAsync("token", new GenerateFlashcardsRequest { Text = "text", Amount = 0 }));

        Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(ex.Message, Is.EqualTo("Amount must be greater than zero."));
    }

    [Test]
    public void GenerateAsync_WhenResponseNull_ThrowsInternalServerError()
    {
        var service = CreateService((_, _, _) => Task.FromResult<object?>(null));

        var ex = Assert.ThrowsAsync<FlashcardGenerationException>(
            () => service.GenerateAsync("token", new GenerateFlashcardsRequest { Text = "text", Amount = 2 }));

        Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(ex.Message, Is.EqualTo("Error invoking Supabase function: Empty response from generation endpoint."));
    }

    [Test]
    public void GenerateAsync_WhenInvokeFails_ThrowsInternalServerError()
    {
        var service = CreateService((_, _, _) => throw new InvalidOperationException("boom"));

        var ex = Assert.ThrowsAsync<FlashcardGenerationException>(
            () => service.GenerateAsync("token", new GenerateFlashcardsRequest { Text = "text", Amount = 2 }));

        Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(ex.Message, Is.EqualTo("Error invoking Supabase function: boom"));
    }

    [Test]
    public async Task GenerateAsync_WhenValidRequest_ReturnsResponseAndPassesPayload()
    {
        var response = new GenerateFlashcardsResponse
        {
            Success = true,
            Flashcards = Array.Empty<GeneratedFlashcardDto>(),
            GenerationId = Guid.NewGuid()
        };

        string? capturedName = null;
        Supabase.Functions.Client.InvokeFunctionOptions? capturedOptions = null;
        CancellationToken capturedToken = default;
        var cts = new CancellationTokenSource();

        var service = CreateService((name, options, token) =>
        {
            capturedName = name;
            capturedOptions = options;
            capturedToken = token;
            return Task.FromResult<object?>(response);
        });

        var result = await service.GenerateAsync(
            "token",
            new GenerateFlashcardsRequest { Text = "source", Amount = 3 },
            cts.Token);

        Assert.That(result, Is.SameAs(response));
        Assert.That(capturedName, Is.EqualTo("generate-flashcards"));
        Assert.That(capturedToken, Is.EqualTo(cts.Token));
        Assert.That(capturedOptions, Is.Not.Null);

        var body = capturedOptions!.Body as Dictionary<string, object>;
        Assert.That(body, Is.Not.Null);
        Assert.That(body!["text"], Is.EqualTo("source"));
        Assert.That(body!["amount"], Is.EqualTo(3));
    }

    private static FlashcardGenerationService CreateService(
        Func<string, Supabase.Functions.Client.InvokeFunctionOptions, CancellationToken, Task<object?>> handler)
    {
        var invoker = new FakeFunctionsInvoker(handler);
        return new FlashcardGenerationService(invoker, NullLogger<FlashcardGenerationService>.Instance);
    }
}
