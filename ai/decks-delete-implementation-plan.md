# API Endpoint Implementation Plan: DELETE /rest/v1/decks?id=eq.{uuid}

## 1. Przegl¹d punktu koñcowego
Usuwa zestaw fiszek u¿ytkownika na podstawie identyfikatora `deck_id`. Usuniêcie jest kaskadowe i usuwa powi¹zane fiszki (`flashcards`) zgodnie z relacj¹ `ON DELETE CASCADE` w bazie danych.

## 2. Szczegó³y ¿¹dania
- Metoda HTTP: `DELETE`
- Struktura URL: `/rest/v1/decks?id=eq.{uuid}`
- Parametry:
  - Wymagane: `id` (UUID zestawu)
  - Opcjonalne: brak
- Request Body: brak

## 3. Wykorzystywane typy
- DTO:
  - `DeckDeleteResponse` (np. `id`, `deleted`, `deletedAt`)
- Command modele:
  - `DeleteDeckCommand` (np. `DeckId`, `UserId`)

## 4. Szczegó³y odpowiedzi
- `200 OK`: potwierdzenie usuniêcia (np. identyfikator i status)
- `400 Bad Request`: nieprawid³owy format UUID
- `401 Unauthorized`: brak/nieprawid³owe uwierzytelnienie
- `404 Not Found`: brak zestawu o podanym `id` lub brak dostêpu
- `500 Internal Server Error`: b³¹d serwera lub Supabase

## 5. Przep³yw danych
1. Warstwa API przyjmuje ¿¹danie `DELETE` z parametrem `id`.
2. Walidacja `id` (UUID, niepuste).
3. Pobranie `UserId` z kontekstu autoryzacji.
4. Serwis domenowy wykonuje usuniêcie zestawu w Supabase:
   - Zapytanie do tabeli `decks` z filtrem `id` i `user_id`.
   - Baza usuwa rekord i kaskadowo powi¹zane `flashcards`.
5. Zwrócenie odpowiedzi `200` z informacj¹ o usuniêciu.

## 6. Wzglêdy bezpieczeñstwa
- Wymagane uwierzytelnienie (token Supabase).
- Autoryzacja: sprawdzenie, czy `decks.user_id` = `UserId`.
- Ochrona przed masowym usuwaniem: brak wsparcia dla wielu `id` w jednym ¿¹daniu.
- Walidacja parametru `id` przed wykonaniem zapytania do bazy.

## 7. Obs³uga b³êdów
- `400`: nieprawid³owy `id` (nie-UUID, pusty).
- `401`: brak/nieprawid³owy token.
- `404`: brak rekordu lub brak dostêpu u¿ytkownika.
- `500`: wyj¹tki po stronie serwera, b³êdy SDK Supabase.
- Rejestrowanie b³êdów: u¿yæ istniej¹cego mechanizmu logowania aplikacji; brak osobnej tabeli b³êdów w specyfikacji.

## 8. Rozwa¿ania dotycz¹ce wydajnoœci
- Operacja usuniêcia pojedynczego rekordu; kaskadowe usuwanie zale¿ne od liczby fiszek.
- Monitorowaæ czas odpowiedzi dla du¿ych zestawów.
- Unikaæ dodatkowych zapytañ – polegaæ na kaskadzie w DB.

## 9. Etapy wdro¿enia
1. Zdefiniowaæ `DeleteDeckCommand` i `DeckDeleteResponse` w `Models`.
2. Dodaæ metodê w serwisie (`DecksService` lub istniej¹cy klient API) do usuwania zestawu po `id` i `user_id`.
3. Zaimplementowaæ endpoint w warstwie API wywo³uj¹cy serwis.
4. Dodaæ walidacjê wejœcia (UUID) i mapowanie b³êdów na statusy HTTP.
5. Dodaæ logowanie b³êdów w przypadku wyj¹tków SDK lub baz danych.
6. Uzupe³niæ testy integracyjne dla scenariuszy: sukces, brak zasobu, brak autoryzacji, nieprawid³owe `id`.
