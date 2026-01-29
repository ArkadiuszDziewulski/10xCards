# API Endpoint Implementation Plan: GET /rest/v1/decks

## 1. Przegl¹d punktu koñcowego
Endpoint zwraca listê zestawów (decks) nale¿¹cych do zalogowanego u¿ytkownika. Dane s¹ pobierane z tabeli `public.decks` przez Supabase REST, z sortowaniem po `created_at` malej¹co.

## 2. Szczegó³y ¿¹dania
- Metoda HTTP: `GET`
- Struktura URL: `/rest/v1/decks`
- Parametry:
  - Wymagane: brak (autoryzacja wynika z tokenu u¿ytkownika)
  - Opcjonalne:
    - `select` (np. `*`)
    - `order` (np. `created_at.desc`)
- Request Body: brak

## 3. Wykorzystywane typy
- DTO:
  - `DeckDto` (odpowiednik wiersza `public.decks` dla odpowiedzi API)
    - `id: string`
    - `user_id: string`
    - `name: string`
    - `created_at: string`
    - `updated_at: string`
- Command modele: brak (GET bez body)

## 3. Szczegó³y odpowiedzi
- Sukces:
  - `200 OK`
  - Body: `DeckDto[]`
- B³êdy:
  - `401 Unauthorized` – brak lub nieprawid³owy token
  - `500 Internal Server Error` – b³¹d po stronie serwera lub Supabase

## 4. Przep³yw danych
1. Klient Blazor WebAssembly wywo³uje `/rest/v1/decks?select=*&order=created_at.desc`.
2. Warstwa HTTP dodaje token u¿ytkownika (Supabase Auth) do nag³ówka `Authorization`.
3. Supabase RLS filtruje rekordy `public.decks` po `user_id` zgodnym z tokenem.
4. Wynik zwracany jest jako tablica obiektów `DeckDto`.

## 5. Wzglêdy bezpieczeñstwa
- Wymagane uwierzytelnianie przez Supabase Auth (JWT w `Authorization` header).
- RLS na `public.decks` musi ograniczaæ dostêp do rekordów u¿ytkownika.
- Brak danych wejœciowych w body minimalizuje ryzyko wstrzykniêæ, ale parametry query powinny byæ walidowane (dozwolony zestaw pól/operacji).

## 6. Obs³uga b³êdów
- `401 Unauthorized`: brak tokenu lub wygas³y token.
- `400 Bad Request`: nieprawid³owe wartoœci `select` lub `order` (jeœli walidowane po stronie klienta/serwera poœredniego).
- `500 Internal Server Error`: b³¹d po stronie Supabase, sieci lub nieoczekiwany wyj¹tek.
- Rejestrowanie b³êdów:
  - Jeœli istnieje tabela b³êdów/logów, zapisywaæ kontekst zapytania (user_id, czas, status) bez wra¿liwych danych.

## 7. Rozwa¿ania dotycz¹ce wydajnoœci
- U¿ywaæ `select=*` tylko, jeœli wszystkie pola s¹ potrzebne; w innym wypadku wybieraæ konkretne kolumny.
- Indeks na `created_at` i filtr RLS po `user_id` s¹ kluczowe dla szybkich zapytañ.
- Cache po stronie klienta (np. pamiêæ podrêczna w stanie aplikacji) dla listy zestawów.

## 8. Etapy wdro¿enia
1. Zweryfikowaæ RLS na `public.decks` dla odczytu tylko w³asnych rekordów.
2. Zdefiniowaæ `DeckDto` w warstwie modeli frontendu (Blazor WASM) zgodnie z typami z `Model.types.ts`.
3. Dodaæ serwis API w Blazor (np. `DecksApiClient`) wykonuj¹cy `GET /rest/v1/decks` z parametrami `select` i `order`.
4. Zaimplementowaæ obs³ugê b³êdów HTTP (401/400/500) i komunikaty dla u¿ytkownika.
5. Dodaæ testy integracyjne lub e2e (jeœli dostêpne) weryfikuj¹ce poprawne filtrowanie po u¿ytkowniku.
6. Uzupe³niæ dokumentacjê w `/docs` oraz `README.md` o nowy endpoint i zasady u¿ycia.
