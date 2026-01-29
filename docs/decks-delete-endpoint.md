# DELETE /rest/v1/decks?id=eq.{uuid}

## Overview
Deletes a deck owned by the authenticated user. Related flashcards are removed automatically by the database cascade.

## Request
- Method: DELETE
- URL: `/rest/v1/decks?id=eq.{uuid}`
- Headers:
  - `Authorization: Bearer <token>`
- Parameters:
  - `id` (UUID, required)

## Responses
- 200 OK: returns confirmation payload with the deleted deck id
- 400 Bad Request: missing or invalid deck id
- 401 Unauthorized: missing or invalid token
- 403 Forbidden: access denied by policy
- 404 Not Found: deck not found or not owned by user
- 500 Internal Server Error: unexpected server issues

## Validation
- `id` must be a valid, non-empty UUID.

## Notes
- Authorization is enforced with RLS (`user_id` equals authenticated user).
- The delete call uses a single deck id to prevent bulk deletions.
