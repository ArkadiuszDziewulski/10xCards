# Plan implementacji widoku Dashboard (Moje Zestawy)

## 1. Przegl¹d
Widok "Dashboard" (Moje Zestawy) stanowi centralny punkt aplikacji dla zalogowanego u¿ytkownika. Jego g³ównym celem jest wyœwietlenie listy dostêpnych zestawów fiszek (Decks), umo¿liwienie stworzenia nowego zestawu, usuniêcia istniej¹cego oraz przejœcia do trybu nauki lub edycji konkretnego zestawu.

## 2. Routing widoku
Widok powinien byæ dostêpny pod nastêpuj¹cymi œcie¿kami:
- `/` (G³ówna strona dla zalogowanych u¿ytkowników)
- `/decks`

**Uwaga:** Widok wymaga autoryzacji (`[Authorize]`).

## 3. Struktura komponentów
Hierarchia komponentów dla tego widoku (katalog `Pages/Dashboard` oraz `Components/Dashboard`):

- `DashboardPage.razor` (G³ówna strona - Container)
  - `DeckList.razor` (Komponent prezentacyjny - Grid/Lista)
    - `DeckCard.razor` (Pojedynczy kafelek zestawu)
  - `CreateDeckModal.razor` (Modal z formularzem tworzenia)
  - `DeleteConfirmationModal.razor` (Modal potwierdzaj¹cy usuniêcie - generyczny lub dedykowany)

## 4. Szczegó³y komponentów

### `DashboardPage.razor`
- **Opis:** G³ówny kontener logiki. Odpowiada za pobranie danych z API, zarz¹dzanie stanem ³adowania (loading state), oraz koordynacjê akcji (otwieranie modali, odœwie¿anie listy po dodaniu/usuniêciu).
- **G³ówne elementy:**
  - `AuthorizeView` do zabezpieczenia widoku.
  - Nag³ówek "Moje Zestawy".
  - Przycisk "Dodaj nowy zestaw" (otwieraj¹cy `CreateDeckModal`).
  - Warunkowe renderowanie: Spinner ³adowania / Komponent b³êdu / `DeckList` / Pusty stan (empty state).
- **Obs³ugiwane interakcje:**
  - `OnInitializedAsync`: Pobranie listy zestawów.
  - `HandleCreateDeck`: Callback po pomyœlnym utworzeniu zestawu (odœwie¿enie listy).
  - `HandleDeleteRestricted`: Callback potwierdzaj¹cy usuniêcie.
- **Wymagane serwisy:** `DecksApiClient`, `NavigationManager`, `AuthenticationStateProvider`.
- **Walidacja/Warunki:**
  - Sprawdzenie czy u¿ytkownik zalogowany (przez `[Authorize]`).

### `DeckList.razor`
- **Opis:** Komponent prezentacyjny (dumb component) wyœwietlaj¹cy siatkê zestawów (np. row/col w Bootstrap).
- **G³ówne elementy:**
  - Pêtla `foreach` iteruj¹ca po zestawach.
  - Renderowanie komponentów `DeckCard`.
- **Propsy:**
  - `Decks`: `List<DeckViewModel>`
  - `OnDeleteClick`: `EventCallback<Guid>` - zdarzenie klikniêcia usuwania przekazuj¹ce ID zestawu.
  - `OnStudyClick`: `EventCallback<Guid>` - opcjonalnie, jeœli przycisk nauki jest handled przez rodzica, ale zazwyczaj link wystarczy.

### `DeckCard.razor`
- **Opis:** Kafelek reprezentuj¹cy pojedynczy zestaw.
- **G³ówne elementy:**
  - Nazwa zestawu (`card-title`).
  - Liczniki (opcjonalnie, jeœli dostêpne w modelu): Iloœæ fiszek, Do powtórki.
  - Przyciski akcji: "Ucz siê" (Primary Anchor Tag), Ikona "Edytuj" (Anchor Tag), Ikona "Usuñ" (Button).
- **Propsy:**
  - `Deck`: `DeckViewModel`
  - `OnDelete`: `EventCallback<Guid>`

### `CreateDeckModal.razor`
- **Opis:** Modal oparty na Bootstrap 5, zawieraj¹cy formularz tworzenia zestawu.
- **G³ówne elementy:**
  - `EditForm` z modelem `CreateDeckFormModel`.
  - `InputText` dla nazwy zestawu.
  - `ValidationMessage` dla pola nazwy.
  - Przyciski "Anuluj" (zamkniêcie) i "Utwórz" (submit).
- **Obs³ugiwana walidacja:**
  - `Required`: Nazwa jest wymagana.
  - `MaxLength`: Nazwa max 200 znaków.
- **Propsy:**
  - `IsVisible`: `bool` - sterowanie wyœwietlaniem (klasa `show`, styl `display: block`).
  - `OnClose`: `EventCallback`
  - `OnSubmit`: `EventCallback<CreateDeckFormModel>`
- **Typy:** `CreateDeckFormModel`.

## 5. Typy

Wymagane ViewModele i DTO (powinny znaleŸæ siê w katalogu `Models` lub zagnie¿d¿one w komponentach, jeœli s¹ specyficzne tylko dla nich).

### `DeckViewModel`
Reprezentacja zestawu na widoku.
```csharp
public class DeckViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    // Opcjonalnie w przysz³oœci: FlashcardCount, DueCount
    public string EditUrl => $"/decks/{Id}";
    public string StudyUrl => $"/study/{Id}";
}
```

### `CreateDeckFormModel`
Model formularza do walidacji.
```csharp
using System.ComponentModel.DataAnnotations;

public class CreateDeckFormModel
{
    [Required(ErrorMessage = "Nazwa zestawu jest wymagana")]
    [StringLength(200, ErrorMessage = "Nazwa nie mo¿e przekraczaæ 200 znaków")]
    public string Name { get; set; } = string.Empty;
}
```

## 6. Zarz¹dzanie stanem

Stan bêdzie zarz¹dzany lokalnie w komponencie `DashboardPage.razor`.

Zmienne stanu:
- `List<DeckViewModel> _decks`: Lista wyœwietlanych zestawów.
- `bool _isLoading`: Flaga steruj¹ca wyœwietlaniem spinnera.
- `string? _errorMessage`: Przechowywanie komunikatów b³êdów API (np. b³¹d pobierania).
- `bool _showCreateModal`: Widocznoœæ modala tworzenia.
- `DeckViewModel? _deckToDelete`: Zestaw wybrany do usuniêcia (dla modala potwierdzenia).
- `bool _showDeleteModal`: Widocznoœæ modala usuwania.

## 7. Integracja API

Wykorzystanie `DecksApiClient`.

1.  **Pobieranie listy (`GET /rest/v1/decks`):**
    - Metoda: `GetDecksAsync(accessToken)`
    - OdpowiedŸ: `List<DeckDto>` -> Mapowanie na `DeckViewModel`.
2.  **Tworzenie zestawu (`POST /rest/v1/decks`):**
    - Metoda: `CreateDeckAsync(accessToken, CreateDeckRequest)`
    - Request: `CreateDeckRequest { Name = ... }`
    - OdpowiedŸ: `DeckDto` -> Dodanie do lokalnej listy `_decks`.
3.  **Usuwanie zestawu (`DELETE /rest/v1/decks?id=eq.{uuid}`):**
    - Metoda: `DeleteDeckAsync(accessToken, deckId)`
    - OdpowiedŸ: `DeckDeleteResponse` -> Usuniêcie z lokalnej listy `_decks`.

**Token:** Token JWT pobierany z `IAccessTokenProvider` lub `AuthenticationStateProvider`.

## 8. Interakcje u¿ytkownika

1.  **£adowanie strony:**
    - Wywo³anie `OnInitializedAsync`.
    - Ustawienie `_isLoading = true`.
    - Pobranie tokena i danych.
    - Ustawienie `_isLoading = false`.
2.  **Otwarcie tworzenia zestawu:**
    - Klikniêcie przycisku "Dodaj zestaw".
    - `_showCreateModal = true`.
3.  **Zatwierdzenie tworzenia:**
    - U¿ytkownik wpisuje nazwê i klika "Utwórz".
    - Wywo³anie API.
    - Sukces: Dodanie do listy, `_showCreateModal = false`.
    - B³¹d (np. duplikat): Wyœwietlenie b³êdu w modalu (wymaga, aby callback `OnSubmit` pozwala³ na obs³ugê b³êdu lub przekazywanie b³êdu z powrotem do modala).
4.  **Anulowanie tworzenia:**
    - Klikniêcie "Anuluj" lub t³a.
    - `_showCreateModal = false`, reset formularza.
5.  **Usuwanie:**
    - Klikniêcie ikony kosza na karcie.
    - `_deckToDelete = deck`.
    - `_showDeleteModal = true`.
    - Potwierdzenie w modalu -> API Delete -> Usuniêcie z listy `_decks`.

## 9. Warunki i walidacja

- Walidacja formularza: `DataAnnotations` (wymagane, d³ugoœæ stringa).
- Walidacja API (Konflikt 409): Jeœli nazwa jest zajêta, API rzuci wyj¹tek (lub zwróci status). Frontend powinien to przechwyciæ i poinformowaæ u¿ytkownika.
- Stan pusty: Jeœli lista `_decks` jest pusta po za³adowaniu, wyœwietl komponent EmptyState.

## 10. Obs³uga b³êdów

- `try-catch` wokó³ wywo³añ API.
- B³êdy krytyczne (³adowanie listy): Ustawienie `_errorMessage` i wyœwietlenie alertu zamiast listy.
- B³êdy akcji (tworzenie/usuwanie): Wyœwietlenie Toast/Snackbar lub b³êdu w modalu.
- Unauthorized: Przekierowanie do logowania (jeœli token wygas³).

## 11. Kroki implementacji

1.  Dodaæ definicje modeli `DeckViewModel` i `CreateDeckFormModel`.
2.  Stworzyæ komponenty `DeckCard.razor` i `DeckList.razor`.
3.  Stworzyæ komponent `CreateDeckModal.razor` z `EditForm`.
4.  Stworzyæ stronê `DashboardPage.razor` w `Pages`.
5.  Zaimplementowaæ pobieranie danych w `DashboardPage` (mock lub real API).
6.  Zaimplementowaæ logikê tworzenia nowego zestawu (otwieranie modala, call API).
7.  Zaimplementowaæ logikê usuwania (modal potwierdzenia, call API).
8.  Dodaæ obs³ugê pustego stanu i ³adowania (spinner).
9.  Zarejestrowaæ stronê w routingu i przetestowaæ nawigacjê.
