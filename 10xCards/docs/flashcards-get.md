# GET /rest/v1/flashcards

## Purpose
Returns flashcards for a given deck from Supabase REST using `deck_id=eq.{uuid}`.

## Request
- Method: `GET`
- URL: `/rest/v1/flashcards`
- Query parameters:
  - `deck_id=eq.{uuid}` (required)
  - `select=*` (required)

## Response
- `200 OK`: list of flashcards.
- `400 Bad Request`: invalid deck id or missing parameters.
- `401 Unauthorized`: missing or invalid access token.
- `404 Not Found`: no flashcards found (optional behavior).
- `500 Internal Server Error`: unexpected server or network errors.

## Client usage
`FlashcardsApiClient.GetByDeckIdAsync` builds the request URL, validates input, and maps Supabase responses to `FlashcardDto`.

---

# GET /rest/v1/flashcards?next_review_at=lte.now()&status=eq.active

## Purpose
Returns due flashcards for the current user where `next_review_at` is in the past and `status` is `active`.

## Request
- Method: `GET`
- URL: `/rest/v1/flashcards`
- Query parameters:
  - `next_review_at=lte.now()` (required)
  - `status=eq.active` (required)
  - `select=*` (required)
  - `limit={int}` (optional)
  - `offset={int}` (optional)

## Response
- `200 OK`: list of flashcards.
- `400 Bad Request`: invalid query parameters (including negative `limit`/`offset`).
- `401 Unauthorized`: missing or invalid access token.
- `404 Not Found`: no flashcards found (optional behavior).
- `500 Internal Server Error`: unexpected server or network errors.

## Client usage
`FlashcardsApiClient.GetDueFlashcardsAsync` builds the request URL, validates input, and maps Supabase responses to `FlashcardDto`.
