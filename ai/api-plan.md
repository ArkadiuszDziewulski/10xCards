# REST API Plan

## 1. Resources
| Resource | Database Table | Description |
| :--- | :--- | :--- |
| **Profile** | `public.profiles` | User profile data linked to Supabase Auth. |
| **Deck** | `public.decks` | Collections of flashcards. |
| **Flashcard** | `public.flashcards` | Individual flashcards with SRS metadata. |
| **Study** | N/A (Logic) | Endpoints for SRS learning sessions. |
| **AI** | `public.generation_events` | AI-powered flashcard generation and logging. |

## 2. Endpoints

### 2.1 Decks
Manage user collections of flashcards.

- **GET `/api/v1/decks`**
  - **Description**: List all decks for the authenticated user.
  - **Query Params**: `sortBy` (created_at), `order` (asc/desc).
  - **Response (200 OK)**: `[{ "id": "uuid", "name": "string", "created_at": "iso_date" }]`

- **POST `/api/v1/decks`**
  - **Description**: Create a new deck.
  - **Payload**: `{ "name": "string" }`
  - **Response (201 Created)**: `{ "id": "uuid", "name": "string" }`
  - **Errors**: 400 (Validation), 409 (Conflict - Name exists).

- **GET `/api/v1/decks/{id}`**
  - **Description**: Get deck details and summary statistics.
  - **Response (200 OK)**: `{ "id": "uuid", "name": "string", "card_count": 0 }`

- **PUT `/api/v1/decks/{id}`**
  - **Description**: Rename a deck.
  - **Payload**: `{ "name": "string" }`
  - **Response (200 OK)**: `{ "id": "uuid", "name": "string" }`

- **DELETE `/api/v1/decks/{id}`**
  - **Description**: Delete a deck and all its flashcards (cascade).
  - **Response (204 No Content)**

---

### 2.2 Flashcards
Manage material content and SRS metadata.

- **GET `/api/v1/decks/{deck_id}/flashcards`**
  - **Description**: List flashcards in a specific deck.
  - **Response (200 OK)**: `[{ "id": "uuid", "front": "...", "back": "...", "status": "active" }]`

- **POST `/api/v1/flashcards`**
  - **Description**: Create a flashcard manually.
  - **Payload**: `{ "deck_id": "uuid", "front": "string", "back": "string", "status": "active" }`
  - **Response (201 Created)**: Full flashcard object.

- **PATCH `/api/v1/flashcards/{id}`**
  - **Description**: Edit flashcard content.
  - **Payload**: `{ "front": "string", "back": "string", "status": "active" }` (partial updates allowed)
  - **Response (200 OK)**

- **DELETE `/api/v1/flashcards/{id}`**
  - **Description**: Permanently remove a flashcard.
  - **Response (204 No Content)**

- **POST `/api/v1/flashcards/batch`**
  - **Description**: Save multiple flashcards at once (used after AI generation approval).
  - **Payload**: `{ "deck_id": "uuid", "cards": [{ "front": "string", "back": "string" }] }`
  - **Response (201 Created)**: `{ "count": 5 }`

---

### 2.3 Study Session (SRS)
Interactive learning logic.

- **GET `/api/v1/study/next`**
  - **Description**: Fetch cards due for review (where `next_review_at <= now()`).
  - **Query Params**: `limit` (default 20), `deck_id` (optional).
  - **Response (200 OK)**: `[{ "id": "uuid", "front": "...", "back": "..." }]`

- **POST `/api/v1/flashcards/{id}/review`**
  - **Description**: Record a review result to update SRS metadata.
  - **Payload**: `{ "rating": 0-5 }` (where 0 is forgot, 5 is perfect).
  - **Success (200 OK)**: `{ "next_review_at": "iso_date", "interval": 4 }`
  - **Business Logic**: Invokes SRS library (e.g., SM-2) to calculate new `ease_factor`, `interval`, and `next_review_at`.

---

### 2.4 AI Generation
AI-powered features and analytics logging.

- **POST `/api/v1/ai/generate`**
  - **Description**: Generate flashcard proposals from input text via LLM.
  - **Payload**: `{ "text": "string (1000-10000 chars)", "deck_id": "uuid?" }`
  - **Response (200 OK)**: `{ "generation_id": "uuid", "proposals": [{ "front": "string", "back": "string" }] }`
  - **Business Logic**: Calls OpenRouter API, parses responses, and inserts a record into `generation_events`.

- **PATCH `/api/v1/ai/generation-events/{id}/accept`**
  - **Description**: Update the `accepted_count` for analytics.
  - **Payload**: `{ "count": "integer" }`
  - **Response (204 No Content)**

## 3. Authentication & Authorization
- **Mechanism**: Supabase Auth (JWT).
- **Header**: `Authorization: Bearer <token>`.
- **Authorization**: 
  - API must verify the JWT and extract the `user_id` (sub).
  - All database queries must enforce `user_id` matches the authenticated user (backed by RLS).

## 4. Validation & Business Logic

### Validation Rules
- **Text Length**: `ai/generate` input must be between 1,000 and 10,000 characters.
- **Deck Name**: Must be unique for the specific `user_id`. Max 100 characters.
- **Flashcard Content**: `front` and `back` cannot be empty.
- **SRS Rating**: Must be an integer between 0 and 5.

### Business Logic Implementation
- **SRS Metadata**: Calculated exclusively on the server to ensure consistency. Default values: `ease_factor = 2.5`, `interval = 0`.
- **Cascade Deletion**: When a user deletes their account (`profiles`), all `decks`, `flashcards`, and `generation_events` are automatically removed (handled at DB level via `ON DELETE CASCADE`).
- **Status Management**: Flashcards default to `active`. `draft` status is reserved for future extensions or bulk imports.
