# API Endpoint Implementation Plan: POST /functions/v1/generate-flashcards

## 1. Przegl¹d punktu koñcowego
Endpoint orkiestruje generowanie fiszek przez LLM (OpenRouter). Przyjmuje tekst wejœciowy, generuje propozycje fiszek, zapisuje zdarzenie w `generation_events` i zwraca listê fiszek wraz z `generationId`.

## 2. Szczegó³y ¿¹dania
- Metoda HTTP: `POST`
- Struktura URL: `/functions/v1/generate-flashcards`
- Nag³ówki:
  - Wymagane: `Authorization: Bearer <user_token>`
  - Opcjonalne: `Content-Type: application/json`
- Parametry:
  - Wymagane (body):
    - `text` (string) — tekst Ÿród³owy do przetworzenia
    - `amount` (number) — liczba fiszek do wygenerowania
  - Opcjonalne: brak
- Request Body (JSON):
  ```json
  {
    "text": "Tekst Ÿród³owy do przetworzenia...",
    "amount": 10
  }
  ```

## 3. Wykorzystywane typy
- DTO:
  - `GenerateFlashcardsRequest`:
    - `text: string`
    - `amount: number`
  - `FlashcardSuggestionDto`:
    - `front: string`
    - `back: string`
  - `GenerateFlashcardsResponse`:
    - `success: boolean`
    - `flashcards: FlashcardSuggestionDto[]`
    - `generationId: string`
- Modele bazy danych (z `Model.types.ts`):
  - `TablesInsert<'generation_events'>`
  - `Tables<'generation_events'>`

## 4. Szczegó³y odpowiedzi
- Sukces: `201 Created`
  ```json
  {
    "success": true,
    "flashcards": [
      { "front": "Pytanie 1", "back": "OdpowiedŸ 1" }
    ],
    "generationId": "uuid-z-tabeli-generation-events"
  }
  ```
- B³êdy:
  - `400 Bad Request`: `text` < 1000 lub > 10000, brak `text`/`amount`, `amount` <= 0
  - `401 Unauthorized`: brak lub niepoprawny token
  - `500 Internal Server Error`: b³¹d komunikacji z OpenRouter lub nieoczekiwany b³¹d serwera

## 5. Przep³yw danych
1. Autoryzacja u¿ytkownika na podstawie nag³ówka `Authorization`.
2. Walidacja `text` i `amount`.
3. Wywo³anie OpenRouter (modele LLM) z promptem zgodnym z wymaganiami generowania.
4. Parsowanie odpowiedzi LLM do listy `FlashcardSuggestionDto`.
5. Zapis zdarzenia do `public.generation_events`:
   - `user_id` z tokenu
   - `input_length = text.length`
   - `total_generated_count = flashcards.length`
   - `accepted_count = 0`
6. Zwrócenie odpowiedzi z list¹ fiszek oraz `generationId`.

## 6. Wzglêdy bezpieczeñstwa
- Wymuszenie `Authorization: Bearer` i weryfikacja u¿ytkownika (`auth.uid()`).
- Zapisy w `generation_events` tylko dla zalogowanego u¿ytkownika (RLS).
- Walidacja d³ugoœci tekstu i typu danych po stronie endpointu.
- Ograniczenie logowania danych wra¿liwych (nie logowaæ pe³nego `text`).
- Obs³uga limitów i czasu odpowiedzi OpenRouter (timeout, retry z backoff).

## 7. Obs³uga b³êdów
- `400`: walidacja danych wejœciowych (krótki/za d³ugi tekst, niepoprawny `amount`).
- `401`: brak/nieprawid³owy token JWT.
- `500`: b³¹d OpenRouter, nieoczekiwane wyj¹tki, b³¹d zapisu do `generation_events`.
- Logowanie b³êdów w logach funkcji Supabase (brak osobnej tabeli b³êdów w specyfikacji).

## 8. Rozwa¿ania dotycz¹ce wydajnoœci
- Minimalizowanie payloadu odpowiedzi i promptu.
- Ograniczenie liczby fiszek generowanych w jednym wywo³aniu (np. walidacja `amount` <= 50) jeœli zespó³ uzna to za konieczne.
- Ustawienie timeoutów dla wywo³añ OpenRouter.
- U¿ycie asynchronicznych wywo³añ I/O.

## 9. Kroki implementacji
1. Utworzyæ DTO (`GenerateFlashcardsRequest`, `FlashcardSuggestionDto`, `GenerateFlashcardsResponse`) w `./Models`.
2. Dodaæ serwis orkiestracji (np. `FlashcardGenerationService`) w `./Services`:
   - walidacja wejœcia
   - wywo³anie OpenRouter
   - mapowanie odpowiedzi
3. Zaimplementowaæ funkcjê Supabase Edge Function `generate-flashcards`:
   - autoryzacja u¿ytkownika
   - walidacja payloadu
   - wywo³anie serwisu generuj¹cego
   - zapis do `generation_events`
   - zwrócenie `201` z `generationId`
4. Dodaæ obs³ugê b³êdów i mapowanie na kody `400/401/500`.
5. Uzupe³niæ konfiguracjê (np. klucz OpenRouter w `appsettings.json`/sekretach funkcji).
6. Dodaæ krótk¹ dokumentacjê w `./docs` opisuj¹c¹ endpoint i wymagania (zgodnie z `DOC_UPDATES`).
7. Dodaæ testy integracyjne (jeœli istnieje infrastruktura): walidacja d³ugoœci tekstu, brak tokenu, b³¹d OpenRouter.
