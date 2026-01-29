# PATCH /rest/v1/flashcards?id=eq.{uuid}

## Purpose
Updates a single flashcard in Supabase REST, including SRS scheduling fields for study results.

## Request
- Method: `PATCH`
- URL: `/rest/v1/flashcards?id=eq.{uuid}`
- Query parameters:
  - `id=eq.{uuid}` (required)
- Headers:
  - `Authorization: Bearer {token}` (required)
  - `Prefer: return=minimal` (recommended)
- Body (JSON):
  - `next_review_at` (RFC3339 string, required for SRS)
  - `interval` (int, >= 0)
  - `ease_factor` (float, > 0)
  - `repetition_count` (int, >= 0)

Example payload:
{
  "next_review_at": "2023-11-01T10:00:00Z",
  "interval": 4,
  "ease_factor": 2.6,
  "repetition_count": 2
}

## Response
- `204 No Content`: updated successfully.
- `400 Bad Request`: invalid UUID or invalid SRS values.
- `401 Unauthorized`: missing or invalid access token.
- `404 Not Found`: flashcard not found or not accessible.
- `500 Internal Server Error`: unexpected server or network errors.

## Client usage
`FlashcardsApiClient.UpdateSrsAsync` validates input, maps payload to `FlashcardSrsUpdateRequest`, sends the PATCH request, and maps Supabase errors to `FlashcardsApiException`.
