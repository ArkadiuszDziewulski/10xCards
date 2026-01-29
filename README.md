# 10xCards

Blazor WebAssembly aplikacja do zarz¹dzania zestawami fiszek i nauki z wykorzystaniem Supabase.

## REST API (Supabase PostgREST)

### GET /rest/v1/decks

Endpoint zwraca listê zestawów zalogowanego u¿ytkownika.

- Nag³ówek: `Authorization: Bearer <token>`
- Parametry: `select`, `order`
- OdpowiedŸ: `DeckDto[]`

### POST /rest/v1/decks

Endpoint tworzy nowy zestaw dla zalogowanego u¿ytkownika.

- Nag³ówek: `Authorization: Bearer <token>`
- Body: `{ "name": "..." }`
- OdpowiedŸ: `DeckDto`

Wiêcej szczegó³ów w `docs/rest-api.md` oraz `docs/decks-post-endpoint.md`.
