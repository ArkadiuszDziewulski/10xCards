# API Endpoint Implementation Plan: GET /rest/v1/flashcards?next_review_at=lte.now()&status=eq.active

## 1. Przegl¹d punktu koñcowego
Endpoint pobiera aktywne fiszki u¿ytkownika, których termin powtórki min¹³ (due cards) dla Sesji Nauki.

## 2. Szczegó³y ¿¹dania
- Metoda HTTP: GET
- Struktura URL: `/rest/v1/flashcards?next_review_at=lte.now()&status=eq.active`
- Parametry:
  - Wymagane:
    - `next_review_at=lte.now()` (filtr na fiszki z terminem powtórki <= teraz)
    - `status=eq.active` (tylko aktywne fiszki)
  - Opcjonalne: brak
- Request Body: brak

## 3. Wykorzystywane typy
- DTO:
  - `FlashcardDto` (mapowanie na strukturê `flashcards.Row` z `Model.types.ts`)
- Command modele: brak (endpoint tylko do odczytu)

## 3. Szczegó³y odpowiedzi
- 200 OK: tablica obiektów `FlashcardDto`
- 401 Unauthorized: brak autoryzacji u¿ytkownika
- 500 Internal Server Error: nieoczekiwany b³¹d po stronie serwera

## 4. Przep³yw danych
1. Warstwa UI (Blazor WebAssembly) wywo³uje klienta API `FlashcardsApiClient`.
2. Serwis API buduje zapytanie do Supabase REST z filtrami `next_review_at=lte.now()` i `status=eq.active`.
3. Supabase zwraca listê fiszek nale¿¹cych do zalogowanego u¿ytkownika (RLS).
4. Serwis mapuje odpowiedŸ na `FlashcardDto` i zwraca do UI.

## 5. Wzglêdy bezpieczeñstwa
- Autoryzacja przez token Supabase (JWT) przekazywany w nag³ówku `Authorization`.
- RLS w Supabase ogranicza dostêp do fiszek u¿ytkownika (`flashcards.user_id = auth.uid()`).
- Walidacja, ¿e wymagane parametry filtrów s¹ obecne i poprawne.

## 6. Obs³uga b³êdów
- 400 Bad Request: brak lub niepoprawna sk³adnia filtrów w zapytaniu.
- 401 Unauthorized: brak tokena lub token niewa¿ny.
- 500 Internal Server Error: b³¹d sieci, wyj¹tek klienta Supabase lub b³¹d serializacji.
- Logowanie b³êdów: rejestrowaæ b³êdy w logach aplikacji (brak osobnej tabeli b³êdów w zasobach).

## 7. Rozwa¿ania dotycz¹ce wydajnoœci
- Ograniczyæ liczbê zwracanych rekordów poprzez paginacjê (np. `limit`, `offset`) jeœli przewidziane w kliencie.
- U¿ywaæ selekcji pól tylko potrzebnych w UI (np. `select=...`).
- Unikaæ dodatkowych zapytañ po stronie klienta.

## 8. Etapy wdro¿enia
1. Zidentyfikowaæ istniej¹cy klient API dla fiszek lub utworzyæ `FlashcardsApiClient` w `Services`.
2. Dodaæ metodê `GetDueFlashcardsAsync()` buduj¹c¹ zapytanie z filtrami `next_review_at=lte.now()` i `status=eq.active`.
3. Zdefiniowaæ `FlashcardDto` w `Models` zgodnie z `flashcards.Row`.
4. Zaimplementowaæ obs³ugê b³êdów i logowanie w serwisie.
5. Uzupe³niæ integracjê w UI (np. `Pages`/`Components`) wykorzystuj¹c now¹ metodê klienta.
6. Dodaæ testy integracyjne (jeœli stosowane) dla poprawnego filtrowania i autoryzacji.
7. Zaktualizowaæ dokumentacjê w `docs` jeœli wymagane przez zasady projektu.
