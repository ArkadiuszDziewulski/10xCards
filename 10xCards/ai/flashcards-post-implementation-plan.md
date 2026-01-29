# API Endpoint Implementation Plan: POST /rest/v1/flashcards

## 1. Przegl¹d punktu koñcowego
Endpoint s³u¿y do dodawania nowych fiszek do tabeli `public.flashcards` pojedynczo lub w batchu. ¯¹danie trafia do Supabase REST i tworzy rekordy powi¹zane z aktualnym u¿ytkownikiem (RLS po `user_id`).

## 2. Szczegó³y ¿¹dania
- Metoda HTTP: `POST`
- Struktura URL: `/rest/v1/flashcards`
- Parametry:
  - Wymagane: brak w query string
  - Opcjonalne: brak w query string
- Request Body: JSON Array obiektów fiszek
  - Wymagane pola: `deck_id`, `front`, `back`
  - Opcjonalne pola: `status` (gdy brak, stosuje siê domyœlne `active` z DB)

## 3. Wykorzystywane typy
- `FlashcardCreateRequest` (DTO wejœciowy): `DeckId` (`Guid`), `Front` (`string`), `Back` (`string`), `Status` (`string?`).
- `CreateFlashcardsCommand` (model komendy): `IReadOnlyList<FlashcardCreateRequest>`.
- `FlashcardDto` (DTO odpowiedzi): pola z `public.flashcards` (`id`, `deck_id`, `user_id`, `front`, `back`, `status`, `next_review_at`, `interval`, `ease_factor`, `repetition_count`, `created_at`, `updated_at`).

## 4. Szczegó³y odpowiedzi
- `201 Created`: lista `FlashcardDto` (gdy u¿yty `Prefer: return=representation`) lub puste body (gdy `return=minimal`).
- `400 Bad Request`: nieprawid³owe dane wejœciowe (puste pola, niepoprawny UUID, b³êdny status).
- `401 Unauthorized`: brak lub niepoprawny token.
- `404 Not Found`: brak zestawu `deck_id` dla u¿ytkownika (jeœli walidacja sprawdza istnienie).
- `500 Internal Server Error`: b³¹d Supabase lub nieobs³u¿ony wyj¹tek klienta.

## 5. Przep³yw danych
1. UI wysy³a `POST /rest/v1/flashcards` z list¹ fiszek.
2. Klient API w `Services` mapuje `FlashcardCreateRequest` do payloadu Supabase.
3. Do ¿¹dania dodawany jest token u¿ytkownika oraz nag³ówek `Prefer: return=representation` (jeœli UI wymaga zwrotu danych).
4. Supabase REST zapisuje rekordy w `public.flashcards` i stosuje RLS na `user_id`.
5. OdpowiedŸ jest mapowana do `FlashcardDto` i zwracana do UI.

## 6. Wzglêdy bezpieczeñstwa
- Uwierzytelnianie Bearer JWT zgodne z Supabase Auth.
- RLS zapewnia zapis wy³¹cznie dla w³aœciciela danych.
- Walidacja `deck_id` po stronie klienta oraz opcjonalne sprawdzenie, czy `deck_id` nale¿y do u¿ytkownika.
- Ochrona przed mass assignment: ignorowanie `user_id` w wejœciu (nie wysy³aæ z klienta).

## 7. Obs³uga b³êdów
- `400`: brak wymaganych pól, puste `front/back`, niepoprawny format `deck_id`, `status` spoza `active|draft`.
- `401`: brak tokenu lub token wygas³.
- `404`: `deck_id` nie istnieje lub nie nale¿y do u¿ytkownika (jeœli weryfikacja w serwisie).
- `500`: b³êdy sieci, time-out, Supabase REST error.
- Logowanie b³êdów: u¿yæ istniej¹cego mechanizmu logowania aplikacji; brak dedykowanej tabeli b³êdów w specyfikacji.

## 8. Rozwa¿ania dotycz¹ce wydajnoœci
- Wspieraæ batch insert, aby ograniczyæ liczbê requestów.
- Ustaliæ limit rozmiaru batcha po stronie klienta, aby unikn¹æ du¿ych payloadów.
- U¿ywaæ asynchronicznych wywo³añ w `Services` i unikaæ dodatkowych zapytañ.

## 9. Etapy wdro¿enia
1. Zdefiniowaæ `FlashcardCreateRequest` oraz `CreateFlashcardsCommand` w `Models` zgodnie z kontraktem.
2. Zidentyfikowaæ lub dodaæ `FlashcardDto` zgodny z tabel¹ `public.flashcards`.
3. Dodaæ metodê w `Services` (np. `FlashcardsApiClient.CreateAsync`) buduj¹c¹ request `POST /rest/v1/flashcards` z JSON Array.
4. Zaimplementowaæ walidacjê wejœcia (puste pola, UUID, status) z wczesnym zwrotem `400`.
5. Dodaæ obs³ugê b³êdów HTTP i mapowanie odpowiedzi `201` na `FlashcardDto`.
6. Zintegrowaæ wywo³anie z UI (np. formularz dodawania fiszek).
7. Dodaæ testy jednostkowe klienta API (walidacja payloadu, obs³uga kodów 400/401/500).
8. Zaktualizowaæ dokumentacjê w `/docs`, `README.md` i `CHANGELOG.md` zgodnie z zasadami projektu.
