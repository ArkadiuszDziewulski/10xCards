# POST /rest/v1/flashcards

## Purpose
Creates flashcards in Supabase REST for the current user. Supports batch inserts.

## Request
- Method: `POST`
- URL: `/rest/v1/flashcards`
- Body: JSON array of flashcards
  - Required fields: `deck_id`, `front`, `back`
  - Optional fields: `status` (`active` or `draft`)
  - `user_id` is set by the client from the access token to satisfy RLS policies
- Headers:
  - `Authorization: Bearer {token}` (required)
  - `Prefer: return=representation` (optional, default) or `return=minimal`

## Response
- `201 Created`: list of flashcards when `Prefer: return=representation`.
- `201 Created`: empty body when `Prefer: return=minimal`.
- `400 Bad Request`: missing fields, invalid UUID, invalid status.
- `401 Unauthorized`: missing or invalid access token.
- `404 Not Found`: deck not found or not accessible.
- `500 Internal Server Error`: unexpected server or network errors.

## Client usage
`FlashcardsApiClient.CreateAsync` validates input, trims content, sends the batch request to Supabase, and maps the response to `FlashcardDto`.
