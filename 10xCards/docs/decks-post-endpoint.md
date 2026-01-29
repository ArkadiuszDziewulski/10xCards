# POST /rest/v1/decks

## Overview
Creates a new deck for the authenticated user. The access token provides the user id; the request body supplies the deck name.

## Request
- Method: POST
- URL: `/rest/v1/decks`
- Headers:
  - `Authorization: Bearer <token>`
  - `Content-Type: application/json`
- Body:
  - `name` (string, required)

## Responses
- 201 Created: returns the created deck record
- 400 Bad Request: missing or invalid name
- 401 Unauthorized: missing or invalid token
- 409 Conflict: duplicate deck name for the user
- 500 Internal Server Error: unexpected server issues

## Validation
- `name` must be non-empty and within the configured maximum length.

## Notes
- The `user_id` is derived from the JWT token (`sub` or `user_id` claim).
- Uniqueness is enforced by the database constraint `(user_id, name)`.
