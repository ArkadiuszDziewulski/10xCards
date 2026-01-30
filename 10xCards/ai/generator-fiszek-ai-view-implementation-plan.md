# Plan implementacji widoku Generator Fiszek AI

## 1. Przegląd
Widok umożliwia wklejenie tekstu źródłowego (1 000–10 000 znaków), wygenerowanie propozycji fiszek przez LLM, edycję/akceptację wyników oraz zapis do wybranego zestawu (decku).

## 2. Routing widoku
`/generate`

## 3. Struktura komponentów
- `GenerateFlashcardsPage` (strona w `Pages`)
  - `SourceInput`
  - `GenerationLoader`
  - `FlashcardReviewList`
    - `FlashcardReviewItem` (dla każdej fiszki)
  - `DeckSelector`
  - `GenerationActions` (przyciski Generuj / Zapisz / Wyczyść)
  - `InlineAlert` (komunikaty błędów/sukcesu)

## 4. Szczegóły komponentów
### `GenerateFlashcardsPage`
- Opis: Strona orkiestrująca cały proces (kroki: wklej → generuj → edytuj → zapisz).
- Główne elementy: układ kolumnowy Bootstrap, `EditForm`, sekcje kroków.
- Obsługiwane interakcje: kliknięcie „Generuj”, „Zapisz”, „Wyczyść”, wybór decku.
- Walidacja: długość tekstu 1000–10000; obecność wybranego decku lub poprawnej nowej nazwy; co najmniej jedna zaakceptowana fiszka.
- Typy: `GenerateFlashcardsRequestDto`, `GenerateFlashcardsResponseDto`, `FlashcardProposalViewModel`, `DeckSelectionViewModel`, `GenerateStateViewModel`.
- Propsy: brak (komponent strony).

### `SourceInput`
- Opis: Duże pole tekstowe z licznikiem znaków i walidacją długości.
- Główne elementy: `textarea`, licznik znaków, pomocniczy opis.
- Interakcje: `oninput` aktualizuje licznik, `onblur` waliduje.
- Walidacja: min 1000, max 10000.
- Typy: `SourceInputViewModel` (Text, CharCount, ValidationMessage).
- Propsy: `Text`, `MinLength`, `MaxLength`, `OnTextChanged`.

### `GenerationLoader`
- Opis: Wskaźnik postępu podczas wywołania API.
- Główne elementy: spinner Bootstrap + tekst stanu.
- Interakcje: brak (sterowany stanem rodzica).
- Walidacja: brak.
- Typy: brak.
- Propsy: `IsLoading`, `Message`.

### `FlashcardReviewList`
- Opis: Lista wygenerowanych fiszek z edycją i akceptacją.
- Główne elementy: lista kart, przyciski edycji, checkbox akceptacji.
- Interakcje: edycja front/back, zaznaczenie akceptacji.
- Walidacja: front/back niepuste, max długości pól (np. 1–500 znaków).
- Typy: `FlashcardProposalViewModel`.
- Propsy: `Items`, `OnItemChanged`.

### `FlashcardReviewItem`
- Opis: Pojedyncza fiszka z trybem edycji.
- Główne elementy: inputy front/back, checkbox „Zatwierdź”, przyciski „Edytuj/Zapisz/Odrzuć”.
- Interakcje: edycja, zapisywanie zmian, odrzucenie (ustawia `IsAccepted=false`).
- Walidacja: front/back wymagane.
- Typy: `FlashcardProposalViewModel`.
- Propsy: `Item`, `OnChange`.

### `DeckSelector`
- Opis: Wybór istniejącego decku lub wpisanie nowej nazwy.
- Główne elementy: dropdown, pole tekstowe nowej nazwy, przełącznik trybu.
- Interakcje: wybór decku, włączenie trybu „nowy deck”.
- Walidacja: istniejący deck wybrany lub nowa nazwa 3–100 znaków.
- Typy: `DeckSelectionViewModel`, `DeckOptionDto`.
- Propsy: `Selection`, `Decks`, `OnSelectionChanged`.

### `GenerationActions`
- Opis: Pasek akcji dla procesu generowania i zapisu.
- Główne elementy: przyciski `Generuj`, `Zapisz`, `Wyczyść`.
- Interakcje: kliknięcia wywołujące akcje rodzica.
- Walidacja: przyciski aktywne wg stanu (np. `Zapisz` tylko gdy są zaakceptowane fiszki).
- Typy: brak.
- Propsy: `CanGenerate`, `CanSave`, `IsBusy`, `OnGenerate`, `OnSave`, `OnReset`.

## 5. Typy
- `GenerateFlashcardsRequestDto`
  - `Text: string`
  - `Amount: int`
- `GenerateFlashcardsResponseDto`
  - `Success: bool`
  - `Flashcards: List<FlashcardDto>`
  - `GenerationId: string`
- `FlashcardDto`
  - `Front: string`
  - `Back: string`
- `FlashcardProposalViewModel`
  - `Id: Guid`
  - `Front: string`
  - `Back: string`
  - `IsAccepted: bool`
  - `IsEditing: bool`
  - `ValidationMessage: string?`
- `DeckOptionDto`
  - `Id: string`
  - `Name: string`
- `DeckSelectionViewModel`
  - `Mode: DeckSelectionMode` (`Existing` | `New`)
  - `SelectedDeckId: string?`
  - `NewDeckName: string?`
- `GenerateStateViewModel`
  - `SourceText: string`
  - `IsLoading: bool`
  - `ErrorMessage: string?`
  - `GenerationId: string?`
  - `Items: List<FlashcardProposalViewModel>`
  - `AcceptedCount: int`

## 6. Zarządzanie stanem
- Stan lokalny w `GenerateFlashcardsPage`.
- `GenerateStateViewModel` przechowuje dane wejściowe, wyniki i flagi ładowania.
- Brak potrzeby customowego hooka; logika w kodzie-behind `GenerateFlashcardsPage.razor.cs`.

## 7. Integracja API
- Endpoint: `POST /functions/v1/generate-flashcards`
- Nagłówki: `Authorization: Bearer <user_token>`.
- Request: `GenerateFlashcardsRequestDto` (`text`, `amount`).
- Response: `GenerateFlashcardsResponseDto` (`success`, `flashcards`, `generationId`).
- Akcje frontendowe:
  - `Generate`: wysyła request, mapuje wynik na `FlashcardProposalViewModel`.
  - `Save`: zapis zaakceptowanych fiszek i ewentualnie nowego decku (przez istniejące serwisy Supabase/HTTP).

## 8. Interakcje użytkownika
- Wklejenie tekstu → licznik znaków aktualizuje się na bieżąco.
- Kliknięcie „Generuj” → pokazanie loadera, blokada edycji i przycisków.
- Otrzymanie wyników → wyświetlenie listy fiszek do edycji/akceptacji.
- Zaznaczenie „Zatwierdź” → fiszka wchodzi do zestawu zapisu.
- Wybór decku lub wpisanie nowej nazwy → zapis do wybranego miejsca.
- Kliknięcie „Zapisz” → utrwalenie danych i informacja o sukcesie.

## 9. Warunki i walidacja
- `SourceInput`: 1000–10000 znaków; puste pole blokuje generowanie.
- `Amount`: wartość domyślna (np. 10) i >=1.
- `FlashcardReviewItem`: front i back wymagane; długość 1–500.
- `DeckSelector`: wybrany deck lub poprawna nowa nazwa.
- `Save`: co najmniej jedna zaakceptowana fiszka.
- `Generate`: tylko gdy walidacja wejścia pozytywna.

## 10. Obsługa błędów
- Timeout/429/5xx: komunikat błędu z możliwością ponowienia.
- Brak odpowiedzi lub pusta lista: komunikat „Brak fiszek do wyświetlenia”.
- Błąd autoryzacji: przekierowanie do logowania lub komunikat o sesji.
- Błędy walidacji: komunikaty inline przy polach.

## 11. Kroki implementacji
1. Dodać stronę `Pages/GenerateFlashcards.razor` z routingiem `/generate`.
2. Utworzyć komponenty `SourceInput`, `GenerationLoader`, `FlashcardReviewList`, `FlashcardReviewItem`, `DeckSelector`, `GenerationActions` w `Components`.
3. Zdefiniować DTO i ViewModel w `Models` (request/response, VM dla listy).
4. Dodać serwis klienta API (np. `Services/FlashcardGenerationService`) do wywołania endpointu.
5. Zaimplementować mapowanie odpowiedzi na VM oraz walidację (DataAnnotations lub ręcznie).
6. Dodać logikę zapisu zaakceptowanych fiszek do decku (istniejące serwisy Supabase).
7. Dodać obsługę błędów i komunikaty UI.
8. Przetestować scenariusze: poprawne generowanie, błędy API, brak akceptacji, nowy deck.
