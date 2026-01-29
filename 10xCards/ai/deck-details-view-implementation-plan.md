# Plan implementacji widoku Szczegó³y Zestawu

## 1. Przegl¹d
Widok `Szczegó³y Zestawu` umo¿liwia przegl¹d i zarz¹dzanie fiszkami w ramach wybranego zestawu. U¿ytkownik mo¿e zobaczyæ nazwê zestawu, listê fiszek, edytowaæ ich treœæ, usuwaæ je oraz rêcznie dodawaæ nowe pozycje.

## 2. Routing widoku
- `/decks/{deckId}` (parametr `deckId` jako `Guid`).

## 3. Struktura komponentów
- `Pages/DeckDetailsPage.razor`
  - `Components/DeckDetails/DeckHeader.razor`
    - `ManualAddButton` (button)
  - `Components/DeckDetails/FlashcardTable.razor`
    - `Components/DeckDetails/FlashcardRow.razor`
    - `Components/Shared/PaginationControls.razor`
  - `Components/DeckDetails/FlashcardEditorModal.razor`
  - `Components/DeckDetails/FlashcardCreateModal.razor`
  - `Components/Dashboard/DeleteConfirmationModal.razor`
  - `Components/Shared/LoadingSpinner.razor` (jeœli istnieje, w przeciwnym razie prosty blok ³adowania w `DeckDetailsPage`)

## 4. Szczegó³y komponentów
### `DeckDetailsPage`
- Opis: Kontener widoku. Pobiera dane zestawu i fiszki, zarz¹dza stanem, obs³uguje b³êdy oraz steruje modalami.
- G³ówne elementy: nag³ówek z nazw¹ zestawu, sekcja akcji (dodaj fiszkê), tabela/lista fiszek, sekcje stanu (loading/empty/error), modale.
- Obs³ugiwane interakcje: inicjalne ³adowanie, paginacja, otwarcie modali dodawania/edycji/usuwania, odœwie¿enie listy po operacjach.
- Obs³ugiwana walidacja:
  - `deckId` musi byæ poprawnym `Guid`.
  - brak dostêpu/niepoprawny token -> komunikat i opcjonalne przekierowanie.
- Typy: `DeckViewModel`, `FlashcardViewModel`, `FlashcardFormModel`, `PaginationState` (custom).
- Propsy: `DeckId` (z routingu).

### `DeckHeader`
- Opis: Prezentuje nazwê zestawu i akcje.
- G³ówne elementy: nag³ówek `h1`, przycisk `Dodaj fiszkê`.
- Obs³ugiwane interakcje: klikniêcie `Dodaj fiszkê`.
- Obs³ugiwana walidacja: brak.
- Typy: brak dodatkowych.
- Propsy: `DeckName`, `OnCreateRequested`.

### `FlashcardTable`
- Opis: Tabela lub lista fiszek z paginacj¹.
- G³ówne elementy: tabela (`table`/`tbody`) lub lista (`ul`), nag³ówki kolumn (`Front`, `Back`, `Status`, `Actions`), kontrolki paginacji.
- Obs³ugiwane interakcje: zmiana strony, klikniêcie edycji/usuniêcia.
- Obs³ugiwana walidacja: brak.
- Typy: `FlashcardViewModel`, `PaginationState`.
- Propsy: `Items`, `Pagination`, `OnEditRequested`, `OnDeleteRequested`, `OnPageChanged`.

### `FlashcardRow`
- Opis: Pojedynczy wiersz fiszki z akcjami.
- G³ówne elementy: komórki z treœci¹ i statusem, przyciski `Edytuj` i `Usuñ`.
- Obs³ugiwane interakcje: edycja, usuwanie.
- Obs³ugiwana walidacja: brak.
- Typy: `FlashcardViewModel`.
- Propsy: `Item`, `OnEditRequested`, `OnDeleteRequested`.

### `FlashcardEditorModal`
- Opis: Modal edycji fiszki (front/back/status).
- G³ówne elementy: `EditForm`, pola `Front`, `Back`, `Status`, przyciski `Zapisz`/`Anuluj`.
- Obs³ugiwane interakcje: zapis, anulowanie.
- Obs³ugiwana walidacja:
  - `Front` i `Back` wymagane (niepuste po trim).
  - `Status` opcjonalny, ale jeœli ustawiony, musi byæ poprawny (`active`/`draft`).
- Typy: `FlashcardFormModel`, `UpdateFlashcardContentCommand`.
- Propsy: `IsOpen`, `Model`, `OnSubmit`, `OnCancel`, `IsBusy`.

### `FlashcardCreateModal`
- Opis: Modal rêcznego dodawania fiszki.
- G³ówne elementy: `EditForm`, pola `Front`, `Back`, `Status`, przyciski `Dodaj`/`Anuluj`.
- Obs³ugiwane interakcje: dodanie fiszki, anulowanie.
- Obs³ugiwana walidacja:
  - `Front` i `Back` wymagane (niepuste po trim).
  - `deckId` przekazywany z rodzica (wymagany).
- Typy: `FlashcardFormModel`, `CreateFlashcardCommand`/`FlashcardCreateRequest`.
- Propsy: `IsOpen`, `DeckId`, `Model`, `OnSubmit`, `OnCancel`, `IsBusy`.

### `DeleteConfirmationModal`
- Opis: Potwierdzenie usuniêcia fiszki.
- G³ówne elementy: treœæ ostrze¿enia, przyciski `Usuñ`/`Anuluj`.
- Obs³ugiwane interakcje: potwierdzenie usuniêcia.
- Obs³ugiwana walidacja: brak.
- Typy: brak.
- Propsy: `IsOpen`, `Title`, `Message`, `OnConfirm`, `OnCancel`, `IsBusy`.

## 5. Typy
### Istniej¹ce DTO
- `FlashcardDto`: `Id`, `DeckId`, `UserId`, `Front`, `Back`, `Status`, `CreatedAt`, `UpdatedAt`, `NextReviewAt`, `Interval`, `EaseFactor`, `RepetitionCount`.
- `CreateFlashcardCommand`: `DeckId`, `Front`, `Back`, `Status`.
- `CreateFlashcardsCommand`: `Flashcards` (lista `FlashcardCreateRequest`).
- `UpdateFlashcardContentCommand`: `Front?`, `Back?`, `Status?`.
- `DeckViewModel`: `Id`, `Name`, `CreatedAt`, `EditUrl`, `StudyUrl`.

### Nowe modele widoku
- `FlashcardViewModel`
  - `Id: Guid`
  - `DeckId: Guid`
  - `Front: string`
  - `Back: string`
  - `Status: FlashcardStatus`
  - `UpdatedAt: DateTimeOffset`
- `FlashcardFormModel`
  - `Front: string` (Required)
  - `Back: string` (Required)
  - `Status: FlashcardStatus?`
- `PaginationState`
  - `PageSize: int`
  - `CurrentPage: int`
  - `TotalItems: int`
  - `TotalPages` (computed)

## 6. Zarz¹dzanie stanem
- Stan lokalny w `DeckDetailsPage`:
  - `DeckId`, `DeckName`, `Flashcards`, `PagedFlashcards`
  - `PaginationState`
  - `IsLoading`, `IsSaving`, `ErrorMessage`
  - `IsCreateModalOpen`, `IsEditModalOpen`, `IsDeleteModalOpen`
  - `SelectedFlashcard`
- Brak potrzeby globalnego stanu; dane s¹ specyficzne dla widoku.

## 7. Integracja API
- Pobranie danych zestawu:
  - `GET /rest/v1/decks?id=eq.{deckId}&select=*` (wymagany nowy klient lub rozszerzenie `DecksApiClient`).
- Pobranie fiszek:
  - `FlashcardsApiClient.GetByDeckIdAsync(accessToken, deckId, "*")`.
- Dodanie fiszki:
  - `FlashcardsApiClient.CreateAsync(accessToken, new CreateFlashcardsCommand([...]), returnRepresentation: true)`.
- Edycja fiszki:
  - `PATCH /rest/v1/flashcards?id=eq.{flashcardId}` z `UpdateFlashcardContentCommand` (wymaga nowej metody w `FlashcardsApiClient`).
- Usuniêcie fiszki:
  - `DELETE /rest/v1/flashcards?id=eq.{flashcardId}` (wymaga nowej metody w `FlashcardsApiClient`).

## 8. Interakcje u¿ytkownika
- Wejœcie na `/decks/{deckId}` -> ³adowanie nazwy zestawu i listy fiszek.
- Klikniêcie `Dodaj fiszkê` -> otwarcie modala tworzenia.
- Zapis w modalu tworzenia -> walidacja, wys³anie `POST`, odœwie¿enie listy.
- Klikniêcie `Edytuj` -> otwarcie modala edycji z wype³nionymi polami.
- Zapis w modalu edycji -> `PATCH`, odœwie¿enie listy.
- Klikniêcie `Usuñ` -> modal potwierdzenia -> `DELETE`, odœwie¿enie listy.
- Zmiana strony -> aktualizacja listy widocznej w tabeli.

## 9. Warunki i walidacja
- `deckId` nie mo¿e byæ pusty (`Guid.Empty`).
- `Front` i `Back` wymagane, po `Trim()` nie mog¹ byæ puste.
- `Status` jeœli ustawiony, musi byæ `active` lub `draft`.
- Brak wyników -> stan pusty z CTA.
- Token autoryzacyjny wymagany dla wszystkich operacji.

## 10. Obs³uga b³êdów
- `401 Unauthorized`: komunikat o braku autoryzacji, opcjonalne przekierowanie do logowania.
- `404 Not Found`: komunikat o braku zestawu lub fiszek.
- `400 Bad Request`: walidacja formularzy i komunikaty b³êdów.
- `409 Conflict`: w przypadku konfliktów backendu (np. ograniczenia) pokazanie czytelnego alertu.
- B³êdy sieci: ogólny alert i mo¿liwoœæ ponowienia.

## 11. Kroki implementacji
1. Utworzyæ stronê `Pages/DeckDetailsPage.razor` z routingiem `/decks/{deckId:guid}` i podstawowym layoutem.
2. Dodaæ komponent `DeckHeader` z przyciskiem rêcznego dodawania.
3. Zaimplementowaæ `FlashcardTable` i `FlashcardRow` z paginacj¹ po stronie klienta.
4. Utworzyæ `FlashcardCreateModal` oraz `FlashcardEditorModal` z `EditForm` i walidacj¹.
5. Dodaæ logikê ³adowania danych: pobranie nazwy zestawu i listy fiszek.
6. Dodaæ obs³ugê akcji CRUD: dodawanie, edycja i usuwanie fiszek.
7. Uzupe³niæ obs³ugê b³êdów i stany UI (loading, empty, error).
8. Zaktualizowaæ dokumentacjê w `docs` po wdro¿eniu widoku.
