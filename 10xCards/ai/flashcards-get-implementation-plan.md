# API Endpoint Implementation Plan: GET /rest/v1/flashcards

## 1. Przegl¹d punktu koñcowego
Endpoint zwraca listê fiszek z tabeli `public.flashcards` dla wskazanego zestawu. Wykorzystuje Supabase REST i filtr `deck_id=eq.{uuid}`.

## 2. Szczegó³y ¿¹dania
- Metoda HTTP: `GET`
- Struktura URL: `/rest/v1/flashcards`
- Parametry:
  - Wymagane: `deck_id=eq.{uuid}`
  - Wymagane: `select=*`
  - Opcjonalne: brak (w specyfikacji)
- Request Body: brak

## 3. Wykorzystywane typy
- `FlashcardDto` (odpowiedŸ): pola z `public.flashcards` (`id`, `deck_id`, `user_id`, `front`, `back`, `status`, `next_review_at`, `interval`, `ease_factor`, `repetition_count`, `created_at`, `updated_at`).
- `GetFlashcardsQuery` (model wejœciowy): `DeckId` jako `Guid`.

## 4. Szczegó³y odpowiedzi
- `200 OK`: lista `FlashcardDto`.
- `400 Bad Request`: nieprawid³owy `deck_id` lub brak wymaganych parametrów.
- `401 Unauthorized`: brak lub niepoprawny token uwierzytelniaj¹cy.
- `404 Not Found`: brak fiszek dla `deck_id` (opcjonalnie, jeœli ustalone w UI/UX; alternatywnie `200` z pust¹ list¹).
- `500 Internal Server Error`: b³¹d po stronie serwera lub Supabase.

## 5. Przep³yw danych
1. UI/klient wywo³uje `GET /rest/v1/flashcards?deck_id=eq.{uuid}&select=*`.
2. Klient API w `Services` buduje zapytanie i przekazuje token u¿ytkownika.
3. Supabase REST filtruje `flashcards` po `deck_id` i RLS po `user_id`.
4. OdpowiedŸ mapowana do `FlashcardDto` i zwracana do UI.

## 6. Wzglêdy bezpieczeñstwa
- Wymagane uwierzytelnienie Bearer JWT zgodne z Supabase Auth.
- RLS w Supabase zapewnia dostêp tylko do fiszek u¿ytkownika.
- Walidacja formatu `deck_id` po stronie klienta przed wys³aniem.

## 7. Obs³uga b³êdów
- `400`: gdy `deck_id` nie jest UUID lub brak `select`.
- `401`: brak tokenu lub wygas³y token.
- `404`: brak zasobów (jeœli projekt przyjmuje takie zachowanie).
- `500`: nieobs³u¿one wyj¹tki klienta API, time-outy, b³êdy sieci.
- Logowanie b³êdów: wykorzystaæ istniej¹cy mechanizm logowania aplikacji; brak dedykowanej tabeli b³êdów w specyfikacji.

## 8. Rozwa¿ania dotycz¹ce wydajnoœci
- Zapewniæ mo¿liwoœæ ograniczenia pola `select` w przysz³oœci.
- Rozwa¿yæ paginacjê przy du¿ych zbiorach (nag³ówki `Range` Supabase), jeœli UI bêdzie tego wymagaæ.
- Unikaæ dodatkowych zapytañ po stronie klienta.

## 9. Etapy wdro¿enia
1. Zidentyfikowaæ lub dodaæ model `FlashcardDto` w `Models` zgodny z kolumnami `public.flashcards`.
2. Zdefiniowaæ `GetFlashcardsQuery` w `Models` lub `Services` dla parametru `deck_id`.
3. Dodaæ metodê w `Services` (np. `FlashcardsApiClient.GetByDeckIdAsync`) buduj¹c¹ URL z `deck_id=eq.{uuid}&select=*`.
4. Zaimplementowaæ walidacjê wejœcia (sprawdzenie `Guid`, brak pustych wartoœci) z wczesnym zwrotem b³êdu.
5. Obs³u¿yæ mapowanie odpowiedzi i b³êdów HTTP (400/401/404/500) z czytelnymi komunikatami.
6. Dodaæ integracjê w UI (np. strona zestawu) wywo³uj¹c¹ serwis.
7. Dodaæ testy jednostkowe klienta API (walidacja URL, obs³uga b³êdów).
8. Zaktualizowaæ dokumentacjê w `/docs` jeœli wymagane przez proces projektu.
