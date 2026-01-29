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

### DELETE /rest/v1/decks?id=eq.{uuid}

Endpoint usuwa zestaw fiszek zalogowanego u¿ytkownika.

- Nag³ówek: `Authorization: Bearer <token>`
- Parametry: `id` (UUID zestawu)
- OdpowiedŸ: `DeckDeleteResponse`

Wiêcej szczegó³ów w `docs/rest-api.md`, `docs/decks-post-endpoint.md` oraz `docs/decks-delete-endpoint.md`.
