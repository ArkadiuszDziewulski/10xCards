# Changelog

## Unreleased
- Added Supabase Auth sign-up integration for `/auth/register` with email confirmation messaging.
- Added `DecksApiClient` for `GET /rest/v1/decks` with validation and error handling.
- Documented decks REST endpoint in `docs/rest-api.md` and `README.md`.
- Added `POST /rest/v1/decks` support in `DecksApiClient` with validation and conflict handling.
- Documented `POST /rest/v1/decks` in `docs/decks-post-endpoint.md` and updated REST docs.
- Added delete support for decks with UI integration and error mapping.
- Documented `DELETE /rest/v1/decks` in `docs/rest-api.md` and `docs/decks-delete-endpoint.md`.
- Added `POST /rest/v1/flashcards` support in `FlashcardsApiClient` with validation and error handling.
- Documented `POST /rest/v1/flashcards` in `docs/flashcards-post.md` and updated `README.md`.
- Added redirect to `/auth/login` when unauthenticated users access `/` or `/decks`.
- Added OpenRouter service models, configuration, and HTTP integration.
- Added OpenRouter test UI section on `Pages/Home.razor`.
- Documented OpenRouter configuration in `docs/openrouter-service.md` and `docs/rest-api.md`.
