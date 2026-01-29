# API Endpoint Implementation Plan: POST /rest/v1/decks

## 1. Przeglπd punktu koÒcowego
Endpoint tworzy nowy zestaw fiszek uøytkownika. `user_id` jest pobierane z tokena uwierzytelniajπcego, a nazwa zestawu pochodzi z treúci øπdania. W przypadku naruszenia unikalnoúci pary `user_id, name` zwracany jest b≥πd konfliktu.

## 2. SzczegÛ≥y øπdania
- Metoda HTTP: `POST`
- Struktura URL: `/rest/v1/decks`
- Parametry:
  - Wymagane: brak w URL/query
  - Opcjonalne: brak
- Request Body (JSON):
  - `name` (string, wymagane)
- èrÛd≥o `user_id`: token JWT dostarczony przez Supabase Auth (nag≥Ûwek `Authorization: Bearer <token>`)

## 3. Wykorzystywane typy
- DTO:
  - `CreateDeckRequest` (w≥asny DTO z polem `Name`)
  - `DeckDto` (mapowanie na `Tables<'decks'>`)
- Command modele:
  - `CreateDeckCommand` (zawiera `UserId` i `Name`)
- Typy Supabase:
  - `TablesInsert<'decks'>` dla operacji `insert`
  - `Tables<'decks'>` dla odpowiedzi

## 4. SzczegÛ≥y odpowiedzi
- `201 Created`: zwrÛÊ utworzony rekord zestawu (lub minimum `id`, `name`, `created_at`, `updated_at`)
- `400 Bad Request`: brak `name`, pusta/za d≥uga nazwa, niepoprawny format JSON
- `401 Unauthorized`: brak/niepoprawny token
- `409 Conflict`: naruszenie unikalnoúci `(user_id, name)`
- `500 Internal Server Error`: b≥Ídy po stronie serwera/Supabase

## 5. Przep≥yw danych
1. Klient Blazor wysy≥a `POST /rest/v1/decks` z `name` i tokenem.
2. Warstwa API/serwis (`DecksApiClient`) mapuje øπdanie na `CreateDeckCommand` i pobiera `user_id` z tokena.
3. Serwis wywo≥uje Supabase REST (lub SDK) `insert` do tabeli `decks`.
4. Supabase egzekwuje RLS i constraint `UNIQUE (user_id, name)`.
5. API zwraca `201` z utworzonym rekordem lub mapuje b≥πd do `409`.

## 6. WzglÍdy bezpieczeÒstwa
- Wymagane uwierzytelnienie przez Supabase Auth (JWT).
- Autoryzacja oparta o RLS w Supabase; `user_id` nie pochodzi z payloadu.
- Walidacja danych wejúciowych po stronie klienta i serwera (minimalny zakres, d≥ugoúÊ, brak pustych wartoúci).
- Ochrona przed nadpisaniem `user_id` przez ignorowanie pÛl spoza kontraktu.

## 7. Obs≥uga b≥ÍdÛw
- Walidacja wejúcia:
  - `name` null/empty/whitespace -> `400`.
  - `name` przekracza limit d≥ugoúci (zgodny z ustalonymi regu≥ami UI/DB) -> `400`.
- Konflikt unikalnoúci:
  - B≥πd z Supabase `23505` mapowany do `409`.
- Uwierzytelnienie:
  - Brak tokena lub niewaøny -> `401`.
- B≥Ídy nieoczekiwane:
  - Logowanie po stronie klienta/serwisu i zwrot `500`.
- Tabela b≥ÍdÛw: brak w specyfikacji; logowanie w istniejπcym mechanizmie aplikacji (np. logger).

## 8. WydajnoúÊ
- Pojedynczy insert do `decks` bez dodatkowych zapytaÒ.
- Indeks/constraint `UNIQUE (user_id, name)` zapewnia szybkie wykrycie konfliktu.
- Unikanie zbÍdnych round-tripÛw (brak pre-check; poleganie na constraint DB).

## 9. Kroki implementacji
1. ZidentyfikowaÊ istniejπce miejsce integracji z Supabase (`Services/DecksApiClient.cs`) i ustaliÊ wzorzec wywo≥aÒ REST.
2. DodaÊ DTO `CreateDeckRequest` oraz `DeckDto` (w `Models`), mapujπce odpowiednio request i response.
3. DodaÊ model `CreateDeckCommand` i logikÍ mapowania (`name` + `user_id` z tokena).
4. RozszerzyÊ `DecksApiClient` o metodÍ `CreateDeckAsync` uøywajπcπ `TablesInsert<'decks'>`.
5. DodaÊ walidacjÍ wejúcia (guard clauses) przed wywo≥aniem Supabase.
6. ZmapowaÊ b≥Ídy Supabase na odpowiednie kody statusu (`409` dla `23505`).
7. ZapewniÊ logowanie b≥ÍdÛw zgodnie z istniejπcym wzorcem.
8. DodaÊ testy integracyjne lub scenariusze rÍczne (sukces, konflikt, brak tokena, b≥Ídny payload).
