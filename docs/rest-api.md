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
