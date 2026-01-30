using System.Net;
using _10xCards.Models;
using _10xCards.Services;
using Microsoft.AspNetCore.Components;

namespace _10xCards.Pages;

public partial class GenerateFlashcards
{
    private const int MinSourceLength = 1000;
    private const int MaxSourceLength = 10000;
    private const int DefaultAmount = 10;
    private const int MaxFlashcardLength = 500;

    [Inject] public FlashcardGenerationService FlashcardGenerationService { get; set; } = default!;
    [Inject] public DecksApiClient DecksApiClient { get; set; } = default!;
    [Inject] public FlashcardsApiClient FlashcardsApiClient { get; set; } = default!;
    [Inject] public SupabaseAuthService SupabaseAuthService { get; set; } = default!;
    [Inject] public Supabase.Client SupabaseClient { get; set; } = default!;
    [Inject] public UserSessionState UserSessionState { get; set; } = default!;
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    [SupplyParameterFromQuery(Name = "deckId")]
    public Guid? DeckId { get; set; }

    private readonly GenerateStateViewModel state = new();
    private readonly List<DeckOptionDto> decks = new();
    private DeckSelectionViewModel deckSelection = new();
    private string? successMessage;
    private string? decksErrorMessage;
    private bool isDecksLoading;

    private bool CanGenerate => IsSourceTextValid && !state.IsLoading;
    private bool CanSave => state.AcceptedCount > 0 && IsDeckSelectionValid && !state.IsLoading;

    private bool IsSourceTextValid =>
        state.SourceText.Length >= MinSourceLength && state.SourceText.Length <= MaxSourceLength;

    private bool IsDeckSelectionValid
    {
        get
        {
            if (deckSelection.Mode == DeckSelectionMode.Existing)
            {
                return deckSelection.SelectedDeckId.HasValue;
            }
            if (string.IsNullOrWhiteSpace(deckSelection.NewDeckName))
            {
                return false;
            }

            var trimmed = deckSelection.NewDeckName.Trim();
            return trimmed.Length >= 3 && trimmed.Length <= 100;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await SupabaseAuthService.EnsureInitializedAsync();

        if (!UserSessionState.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/auth/login");
            return;
        }

        await LoadDecksAsync();
        ApplyDeckSelectionFromQuery();
    }

    private void ApplyDeckSelectionFromQuery()
    {
        if (!DeckId.HasValue || DeckId.Value == Guid.Empty)
        {
            return;
        }

        if (decks.Count == 0)
        {
            decksErrorMessage = "Wybrany zestaw nie istnieje lub nie jest dostêpny.";
            return;
        }

        var matchingDeck = decks.FirstOrDefault(deck => deck.Id == DeckId.Value);
        if (matchingDeck is null)
        {
            decksErrorMessage = "Wybrany zestaw nie istnieje lub nie jest dostêpny.";
            return;
        }

        deckSelection = new DeckSelectionViewModel
        {
            Mode = DeckSelectionMode.Existing,
            SelectedDeckId = matchingDeck.Id,
            NewDeckName = null,
        };
    }

    private async Task LoadDecksAsync()
    {
        decksErrorMessage = null;
        isDecksLoading = true;
        decks.Clear();

        var accessToken = SupabaseClient.Auth.CurrentSession?.AccessToken;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            decksErrorMessage = "Brak aktywnej sesji. Zaloguj siê, aby wybraæ zestaw.";
            isDecksLoading = false;
            return;
        }

        try
        {
            var deckDtos = await DecksApiClient.GetDecksAsync(accessToken);
            decks.AddRange(deckDtos.Select(deck => new DeckOptionDto
            {
                Id = deck.Id,
                Name = deck.Name,
            }));
        }
        catch (DecksApiException exception)
        {
            decksErrorMessage = exception.StatusCode switch
            {
                HttpStatusCode.Unauthorized => "Brak dostêpu do zestawów.",
                HttpStatusCode.Forbidden => "Nie masz uprawnieñ do zestawów.",
                _ => exception.Message,
            };
        }
        catch (Exception exception)
        {
            decksErrorMessage = $"Nieoczekiwany b³¹d: {exception.Message}";
        }
        finally
        {
            isDecksLoading = false;
        }
    }

    private Task HandleSourceTextChanged(string text)
    {
        state.SourceText = text;
        state.ErrorMessage = null;
        successMessage = null;
        return Task.CompletedTask;
    }

    private Task HandleFlashcardChanged(FlashcardProposalViewModel item)
    {
        UpdateAcceptedCount();
        return Task.CompletedTask;
    }

    private Task HandleDeckSelectionChanged(DeckSelectionViewModel selection)
    {
        deckSelection = selection;
        state.ErrorMessage = null;
        successMessage = null;
        return Task.CompletedTask;
    }

    private async Task HandleGenerateAsync()
    {
        state.ErrorMessage = null;
        successMessage = null;

        if (!IsSourceTextValid)
        {
            state.ErrorMessage = $"Tekst musi mieæ d³ugoœæ od {MinSourceLength} do {MaxSourceLength} znaków.";
            return;
        }

        state.IsLoading = true;
        state.Items.Clear();
        state.AcceptedCount = 0;
        state.GenerationId = null;

        try
        {
            await SupabaseAuthService.EnsureInitializedAsync();
            var accessToken = SupabaseClient.Auth.CurrentSession?.AccessToken;
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                state.ErrorMessage = "Brak aktywnej sesji. Zaloguj siê, aby generowaæ fiszki.";
                return;
            }

            var response = await FlashcardGenerationService.GenerateAsync(
                accessToken,
                new GenerateFlashcardsRequest
                {
                    Text = state.SourceText,
                    Amount = DefaultAmount,
                });

            if (!response.Success)
            {
                state.ErrorMessage = "Generowanie fiszek nie powiod³o siê.";
                return;
            }

            if (response.Flashcards.Count == 0)
            {
                state.ErrorMessage = "Brak fiszek do wyœwietlenia.";
                return;
            }

            state.GenerationId = response.GenerationId;
            state.Items.AddRange(response.Flashcards.Select(flashcard => new FlashcardProposalViewModel
            {
                Id = Guid.NewGuid(),
                Front = flashcard.Front,
                Back = flashcard.Back,
                IsAccepted = false,
                IsEditing = false,
            }));
            UpdateAcceptedCount();
        }
        catch (FlashcardGenerationException exception)
        {
            state.ErrorMessage = exception.StatusCode switch
            {
                HttpStatusCode.Unauthorized => "Sesja wygas³a. Zaloguj siê ponownie.",
                HttpStatusCode.Forbidden => "Brak dostêpu do generatora fiszek.",
                (HttpStatusCode)429 => "Limit zapytañ zosta³ przekroczony. Spróbuj ponownie póŸniej.",
                HttpStatusCode.InternalServerError => "Wyst¹pi³ b³¹d serwera podczas generowania fiszek.",
                HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout
                    => "Us³uga generowania jest chwilowo niedostêpna. Spróbuj ponownie póŸniej.",
                _ => exception.Message,
            };
        }
        catch (Exception exception)
        {
            state.ErrorMessage = $"Nieoczekiwany b³¹d: {exception.Message}";
        }
        finally
        {
            state.IsLoading = false;
        }
    }

    private async Task HandleSaveAsync()
    {
        state.ErrorMessage = null;
        successMessage = null;

        var acceptedItems = GetAcceptedItems();
        if (acceptedItems.Count == 0)
        {
            state.ErrorMessage = "Zaakceptuj co najmniej jedn¹ fiszkê przed zapisem.";
            return;
        }

        if (!ValidateAcceptedItems(acceptedItems))
        {
            state.ErrorMessage = "Uzupe³nij poprawnie wszystkie zaakceptowane fiszki.";
            return;
        }

        if (!IsDeckSelectionValid)
        {
            state.ErrorMessage = "Wybierz istniej¹cy zestaw lub podaj nazwê nowego.";
            return;
        }

        state.IsLoading = true;

        try
        {
            await SupabaseAuthService.EnsureInitializedAsync();
            var accessToken = SupabaseClient.Auth.CurrentSession?.AccessToken;
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                state.ErrorMessage = "Brak aktywnej sesji. Zaloguj siê, aby zapisaæ fiszki.";
                return;
            }

            var deckId = await ResolveDeckIdAsync(accessToken);
            if (deckId == Guid.Empty)
            {
                return;
            }

            var createRequests = acceptedItems.Select(item => new FlashcardCreateRequest
            {
                DeckId = deckId,
                Front = item.Front.Trim(),
                Back = item.Back.Trim(),
                Status = FlashcardStatus.Active,
            }).ToList();

            await FlashcardsApiClient.CreateAsync(
                accessToken,
                new CreateFlashcardsCommand(createRequests),
                returnRepresentation: false);

            successMessage = $"Zapisano {createRequests.Count} fiszek.";
            NavigationManager.NavigateTo($"/decks/{deckId}");
        }
        catch (DecksApiException exception)
        {
            state.ErrorMessage = exception.StatusCode switch
            {
                HttpStatusCode.Conflict => "Zestaw o tej nazwie ju¿ istnieje.",
                HttpStatusCode.BadRequest => "Nieprawid³owa nazwa zestawu.",
                _ => exception.Message,
            };
        }
        catch (FlashcardsApiException exception)
        {
            state.ErrorMessage = exception.StatusCode switch
            {
                HttpStatusCode.Forbidden => "Nie masz uprawnieñ do zapisu fiszek.",
                HttpStatusCode.NotFound => "Wybrany zestaw nie istnieje.",
                _ => exception.Message,
            };
        }
        catch (Exception exception)
        {
            state.ErrorMessage = $"Nieoczekiwany b³¹d: {exception.Message}";
        }
        finally
        {
            state.IsLoading = false;
        }
    }

    private async Task<Guid> ResolveDeckIdAsync(string accessToken)
    {
        if (deckSelection.Mode == DeckSelectionMode.Existing)
        {
            if (deckSelection.SelectedDeckId.HasValue)
            {
                return deckSelection.SelectedDeckId.Value;
            }

            state.ErrorMessage = "Wybierz zestaw do zapisu.";
            return Guid.Empty;
        }

        var deckName = deckSelection.NewDeckName?.Trim();
        if (string.IsNullOrWhiteSpace(deckName))
        {
            state.ErrorMessage = "Podaj nazwê nowego zestawu.";
            return Guid.Empty;
        }

        var createdDeck = await DecksApiClient.CreateDeckAsync(
            accessToken,
            new CreateDeckRequest { Name = deckName });

        decks.Insert(0, new DeckOptionDto
        {
            Id = createdDeck.Id,
            Name = createdDeck.Name,
        });

        deckSelection = new DeckSelectionViewModel
        {
            Mode = DeckSelectionMode.Existing,
            SelectedDeckId = createdDeck.Id,
            NewDeckName = null,
        };

        return createdDeck.Id;
    }

    private List<FlashcardProposalViewModel> GetAcceptedItems()
    {
        return state.Items.Where(item => item.IsAccepted).ToList();
    }

    private bool ValidateAcceptedItems(IReadOnlyList<FlashcardProposalViewModel> items)
    {
        var isValid = true;

        foreach (var item in items)
        {
            var front = item.Front?.Trim() ?? string.Empty;
            var back = item.Back?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(front) || string.IsNullOrWhiteSpace(back))
            {
                item.ValidationMessage = "Przód i ty³ fiszki s¹ wymagane.";
                isValid = false;
                continue;
            }

            if (front.Length > MaxFlashcardLength || back.Length > MaxFlashcardLength)
            {
                item.ValidationMessage = $"Treœæ fiszki mo¿e mieæ maksymalnie {MaxFlashcardLength} znaków.";
                isValid = false;
                continue;
            }

            item.ValidationMessage = null;
        }

        return isValid;
    }

    private Task HandleResetAsync()
    {
        state.SourceText = string.Empty;
        state.Items.Clear();
        state.AcceptedCount = 0;
        state.GenerationId = null;
        state.ErrorMessage = null;
        successMessage = null;
        deckSelection = new DeckSelectionViewModel();
        return Task.CompletedTask;
    }

    private void UpdateAcceptedCount()
    {
        state.AcceptedCount = state.Items.Count(item => item.IsAccepted);
    }
}
