# REST API

## Decks

### GET /rest/v1/decks

Pobiera listê zestawów zalogowanego u¿ytkownika. Zapytanie jest filtrowane przez RLS w Supabase na podstawie tokenu JWT.

- Metoda: `GET`
- URL: `/rest/v1/decks`
- Nag³ówek: `Authorization: Bearer <token>`
- Parametry:
  - `select` (opcjonalny, domyœlnie `*`)
  - `order` (opcjonalny, domyœlnie `created_at.desc`)
- OdpowiedŸ: `DeckDto[]`
- B³êdy:
  - `400 Bad Request` (nieprawid³owe parametry `select` lub `order`)
  - `401 Unauthorized`
  - `403 Forbidden`
  - `500 Internal Server Error`

### POST /rest/v1/decks

Tworzy nowy zestaw dla zalogowanego u¿ytkownika. `user_id` pochodzi z tokenu JWT.

- Metoda: `POST`
- URL: `/rest/v1/decks`
- Nag³ówek: `Authorization: Bearer <token>`
- Body:
  - `name` (wymagane)
- OdpowiedŸ: `DeckDto`
- B³êdy:
  - `400 Bad Request` (pusty lub zbyt d³ugi `name`)
  - `401 Unauthorized`
  - `409 Conflict` (duplikat nazwy w ramach u¿ytkownika)
  - `500 Internal Server Error`

Szczegó³y w `docs/decks-post-endpoint.md`.

### DELETE /rest/v1/decks?id=eq.{uuid}

Usuwa zestaw fiszek zalogowanego u¿ytkownika. Rekordy fiszek powi¹zane z zestawem s¹ usuwane kaskadowo przez bazê danych.

- Metoda: `DELETE`
- URL: `/rest/v1/decks?id=eq.{uuid}`
- Nag³ówek: `Authorization: Bearer <token>`
- Parametry:
  - `id` (wymagane, UUID zestawu)
- OdpowiedŸ: `DeckDeleteResponse`
- B³êdy:
  - `400 Bad Request` (nieprawid³owy identyfikator)
  - `401 Unauthorized`
  - `403 Forbidden`
  - `404 Not Found`
  - `500 Internal Server Error`

Szczegó³y w `docs/decks-delete-endpoint.md`.

## OpenRouter

Konfiguracja i u¿ycie serwisu LLM opisane s¹ w `docs/openrouter-service.md`.
