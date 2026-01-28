# API Endpoint Implementation Plan: POST /functions/v1/generate-flashcards

## 1. Przegl¹d punktu koñcowego
Endpoint orkiestruje generowanie fiszek przez us³ugê AI (OpenRouter) na podstawie tekstu wejœciowego u¿ytkownika. Waliduje dane wejœciowe, wysy³a ¿¹danie do modelu, parsuje odpowiedŸ, zapisuje zdarzenie w `public.generation_events` i zwraca listê propozycji fiszek.

## 2. Szczegó³y ¿¹dania
- Metoda HTTP: `POST`
- Struktura URL: `/functions/v1/generate-flashcards`
- Parametry:
  - Wymagane: brak (brak query string)
  - Opcjonalne: brak
- Nag³ówki:
  - `Authorization: Bearer <user_token>`
  - `Content-Type: application/json`
- Request Body:
  - `text` (string, wymagany) — d³ugoœæ od 1000 do 10000 znaków.
  - `amount` (number, wymagany) — liczba fiszek do wygenerowania.

## 3. Wykorzystywane typy
- `GenerateFlashcardsRequest` (C#) jako model wejœciowy.
- `GenerateFlashcardsResponse` (C#) jako model odpowiedzi.
- `GeneratedFlashcardDto` (C#) jako element listy `flashcards`.
- `GenerationEventDto` (C#) jako reprezentacja wpisu w `public.generation_events`.

## 4. Szczegó³y odpowiedzi
- `201 Created`: zwraca obiekt z `success`, `flashcards` i `generationId`.
- `400 Bad Request`: tekst za krótki lub za d³ugi, brak wymaganych pól.
- `401 Unauthorized`: brak lub nieprawid³owy token.
- `500 Internal Server Error`: b³¹d komunikacji z OpenRouter lub b³¹d parsowania.

## 5. Przep³yw danych
1. Klient Blazor wysy³a ¿¹danie do Edge Function z tokenem u¿ytkownika.
2. Funkcja weryfikuje token, waliduje `text` i `amount`.
3. Funkcja wywo³uje OpenRouter z tekstem i iloœci¹ fiszek.
4. OdpowiedŸ modelu jest parsowana do listy `GeneratedFlashcardDto`.
5. Funkcja zapisuje rekord w `public.generation_events` (kolumny: `user_id`, `input_length`, `total_generated_count`, opcjonalnie `target_deck_id`).
6. Funkcja zwraca `GenerateFlashcardsResponse` z `generationId`.

## 6. Wzglêdy bezpieczeñstwa
- Wymagane uwierzytelnienie JWT z Supabase Auth (header `Authorization`).
- Autoryzacja oparta o RLS w `public.generation_events` (`auth.uid() = user_id`).
- Walidacja d³ugoœci `text` i zakresu `amount` po stronie funkcji.
- Ochrona przed nadu¿yciem: limit liczby ¿¹dañ na u¿ytkownika (rate limiting w Edge Function) i limity kosztów OpenRouter.
- Nie zapisywaæ treœci fiszek w `generation_events` (zgodnie ze specyfikacj¹ logów).

## 7. Obs³uga b³êdów
- `400`: niespe³nione warunki d³ugoœci tekstu lub brak pól ? komunikat walidacyjny i wczesny zwrot.
- `401`: brak tokena lub token niewa¿ny ? komunikat o koniecznoœci logowania.
- `500`: b³¹d OpenRouter, timeout, b³¹d parsowania odpowiedzi ? logowanie szczegó³ów w logach funkcji.
- Rejestrowanie b³êdów w tabeli: brak dedykowanej tabeli b³êdów; zapisywaæ szczegó³y w logach funkcji i monitoringu Supabase.

## 8. Rozwa¿ania dotycz¹ce wydajnoœci
- Utrzymywaæ limit `amount` (np. górny limit po stronie funkcji) w celu ograniczenia kosztów i czasu odpowiedzi.
- Ustawiæ timeouty dla po³¹czenia z OpenRouter i mechanizm retry z ograniczeniem liczby prób.
- Rozwa¿yæ cache'owanie wyników dla identycznych zapytañ w przysz³oœci (opcjonalnie).

## 9. Etapy wdro¿enia
1. Zdefiniowaæ kontrakt wejœcia/wyjœcia dla Edge Function zgodny z `GenerateFlashcardsRequest` i `GenerateFlashcardsResponse`.
2. Zaimplementowaæ walidacjê `text` (1000-10000 znaków) oraz `amount` z guard clauses i wczesnym zwrotem `400`.
3. Dodaæ klienta OpenRouter oraz logikê parsowania odpowiedzi do listy `GeneratedFlashcardDto`.
4. Zaimplementowaæ zapis do `public.generation_events` z `user_id`, `input_length`, `total_generated_count` i `target_deck_id` (jeœli dostêpny).
5. Obs³u¿yæ b³êdy komunikacji z OpenRouter i b³êdy parsowania, mapuj¹c je na `500`.
6. Zwróciæ odpowiedŸ `201` z `generationId` i list¹ fiszek.
7. Dodaæ wywo³anie w serwisie Blazor odpowiedzialnym za AI oraz mapowanie na `GenerateFlashcardsResponse`.
8. Zweryfikowaæ dzia³anie end-to-end z poprawnym tokenem u¿ytkownika i walidacj¹ RLS.
