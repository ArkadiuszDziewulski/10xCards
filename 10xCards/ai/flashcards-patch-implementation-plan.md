# API Endpoint Implementation Plan: `PATCH /rest/v1/flashcards?id=eq.{uuid}`

## 1. Przegl¹d punktu koñcowego
Endpoint aktualizuje jedn¹ fiszkê. W scenariuszu SRS klient wysy³a zaktualizowane wartoœci `next_review_at`, `interval`, `ease_factor`, `repetition_count`. OdpowiedŸ sukcesu: `204 No Content`.

## 2. Szczegó³y ¿¹dania
- Metoda HTTP: `PATCH`
- Struktura URL: `/rest/v1/flashcards?id=eq.{uuid}`
- Parametry:
  - Wymagane:
    - `id` (UUID) w query string (`id=eq.{uuid}`)
  - Opcjonalne:
    - Brak w query; zakres pól w body zale¿ny od scenariusza
- Request Body (SRS):
  - `next_review_at` (string, RFC3339, nullable w DB, ale w SRS wymagane)
  - `interval` (int, >= 0)
  - `ease_factor` (float, > 0)
  - `repetition_count` (int, >= 0)

## 3. Wykorzystywane typy
- DTO:
  - `FlashcardSrsUpdateRequest` (payload PATCH dla SRS)
  - `FlashcardUpdateResponse` (opcjonalnie, jeœli kiedykolwiek wymagane bêdzie `200`, obecnie brak treœci)
- Command modele:
  - `UpdateFlashcardSrsCommand` (id + parametry SRS)
- Modele odpowiedzi b³êdu:
  - `ApiErrorResponse` (zgodny z istniej¹cym standardem projektu)

## 4. Szczegó³y odpowiedzi
- `204 No Content` — udana aktualizacja.
- `400 Bad Request` — nieprawid³owe dane wejœciowe (np. niepoprawny UUID, wartoœci spoza zakresu).
- `401 Unauthorized` — brak/nieprawid³owy token.
- `404 Not Found` — fiszka o danym `id` nie istnieje lub nie nale¿y do u¿ytkownika.
- `500 Internal Server Error` — b³¹d po stronie serwera/Supabase.

## 5. Przep³yw danych
1. Warstwa UI/klient (Blazor WebAssembly) przygotowuje `UpdateFlashcardSrsCommand`.
2. Serwis `FlashcardsApiClient` (lub analogiczny istniej¹cy) wysy³a `PATCH` do Supabase REST.
3. Supabase RLS weryfikuje dostêp po `user_id`.
4. Baza aktualizuje rekord i zwraca `204`.
5. Klient obs³uguje sukces/brak treœci lub mapuje b³¹d na `ApiErrorResponse`.

## 6. Wzglêdy bezpieczeñstwa
- Wymagany token JWT Supabase w nag³ówku `Authorization: Bearer`.
- RLS musi ograniczaæ aktualizacje do `flashcards.user_id = auth.uid()`.
- Walidacja po stronie klienta i serwisu: UUID, zakresy liczbowe, format daty.
- Brak mo¿liwoœci masowej aktualizacji — tylko pojedynczy `id`.

## 7. Obs³uga b³êdów
- Walidacja wejœcia przed wywo³aniem API (guard clauses):
  - `id` wymagany i poprawny UUID.
  - `interval >= 0`, `repetition_count >= 0`.
  - `ease_factor > 0`.
  - `next_review_at` poprawny format ISO 8601.
- Mapowanie b³êdów Supabase na statusy aplikacyjne:
  - `401` dla braku/wygaœniêcia tokenu.
  - `404` gdy brak rekordu lub brak uprawnieñ.
  - `400` dla b³êdów walidacji payload.
  - `500` dla b³êdów sieciowych/serwera.
- Logowanie b³êdów: u¿yæ istniej¹cej infrastruktury logowania (jeœli brak tabeli b³êdów, logowaæ do standardowego loggera i opcjonalnie Sentry/telemetrii).

## 8. Rozwa¿ania dotycz¹ce wydajnoœci
- Aktualizacja pojedynczego rekordu (`PATCH` na `id`) — minimalny koszt.
- Unikaæ dodatkowych zapytañ, jeœli nie s¹ potrzebne (np. brak `SELECT` po aktualizacji, bo `204`).
- Walidacja lokalna zmniejsza liczbê b³êdnych wywo³añ API.

## 9. Kroki implementacji
1. Zweryfikowaæ istniej¹cy serwis klienta API (`FlashcardsApiClient` lub analogiczny) i zlokalizowaæ miejsce na metodê `UpdateSrsAsync`.
2. Dodaæ modele `FlashcardSrsUpdateRequest` i `UpdateFlashcardSrsCommand` w `./Models`.
3. Zaimplementowaæ metodê serwisu wysy³aj¹c¹ `PATCH /rest/v1/flashcards?id=eq.{uuid}` z odpowiednim body i nag³ówkami.
4. Dodaæ walidacjê wejœciow¹ w serwisie (guard clauses) i mapowanie b³êdów na `ApiErrorResponse`.
5. Zintegrowaæ wywo³anie z UI/komponentem, który obs³uguje wynik nauki (SRS).
6. Uzupe³niæ testy integracyjne/kontraktowe (jeœli istniej¹) dla kodów `204`, `400`, `401`, `404`, `500`.
7. Zaktualizowaæ dokumentacjê w `./docs`, jeœli istnieje sekcja API lub opis endpointów.
