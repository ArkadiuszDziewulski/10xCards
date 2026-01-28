# 10xCards

Blazor WebAssembly aplikacja do zarz¹dzania zestawami fiszek i nauki z wykorzystaniem Supabase.

## REST API (Supabase PostgREST)

### GET /rest/v1/decks

Endpoint zwraca listê zestawów zalogowanego u¿ytkownika.

- Nag³ówek: `Authorization: Bearer <token>`
- Parametry: `select`, `order`
- OdpowiedŸ: `DeckDto[]`

Wiêcej szczegó³ów w `docs/rest-api.md`.
