# REST API Plan - 10xCards

Ten dokument definiuje strukturê API REST dla aplikacji 10xCards. Ze wzglêdu na architekturê opart¹ na **Supabase**, wiêkszoœæ punktów koñcowych standardowych operacji CRUD jest realizowana bezpoœrednio przez **PostgREST** (automatyczne API na bazie schematu DB). Logika niestandardowa (AI) jest realizowana przez **Supabase Edge Functions**.

## 1. Zasoby

| Zasób | Tabela Bazy Danych | Opis | Typ API |
| :--- | :--- | :--- | :--- |
| **Auth** | `auth.users` | Zarz¹dzanie sesj¹ i u¿ytkownikami (Rejestracja, Logowanie) | Supabase GoTrue |
| **Profiles** | `public.profiles` | Dane profilowe u¿ytkownika | PostgREST |
| **Decks** | `public.decks` | Zestawy fiszek | PostgREST |
| **Flashcards** | `public.flashcards` | Fiszki i dane SRS | PostgREST |
| **AI Generation**| - | Generowanie treœci przez LLM | Edge Function |
| **History** | `public.generation_events` | Logi operacji AI | PostgREST |

## 2. Punkty koñcowe (Endpoints)

### 2.1 AI Service (Custom Logic)

#### `POST /functions/v1/generate-flashcards`
Punkt koñcowy orkiestruj¹cy proces generowania fiszek przez LLM (OpenRouter).

- **Opis**: Przyjmuje tekst od u¿ytkownika, wysy³a do modelu AI, parsuje odpowiedŸ i zwraca listê propozycji fiszek JSON. Loguje zdarzenie w `generation_events`.
- **Parametry zapytania**: Brak.
- **Nag³ówki**: `Authorization: Bearer <user_token>`
- **Body ¯¹dania (JSON)**:
  ```json
  {
    "text": "Tekst Ÿród³owy do przetworzenia...",
    "amount": 10
  }
  ```
- **Body Odpowiedzi (JSON)**:
  ```json
  {
    "success": true,
    "flashcards": [
      { "front": "Pytanie 1", "back": "OdpowiedŸ 1" },
      ...
    ],
    "generationId": "uuid-z-tabeli-generation-events"
  }
  ```
- **Kody B³êdów**:
  - `400 Bad Request`: Tekst za krótki (<1000) lub za d³ugi (>10000).
  - `500 Internal Server Error`: B³¹d komunikacji z OpenRouter.

---

### 2.2 Decks (Standard CRUD - PostgREST)

#### `GET /rest/v1/decks`
Pobiera listê zestawów u¿ytkownika.

- **Parametry URL**: `?select=*&order=created_at.desc`
- **OdpowiedŸ**: Tablica obiektów `Deck`.

#### `POST /rest/v1/decks`
Tworzy nowy zestaw.

- **Body (JSON)**: `{ "name": "Nazwa Zestawu" }` (user_id z tokena)
- **Sukces**: `201 Created`.
- **B³¹d**: `409 Conflict` (Naruszenie unikalnoœci pary `user_id, name`).

#### `DELETE /rest/v1/decks?id=eq.{uuid}`
Usuwa zestaw i kaskadowo wszystkie jego fiszki.

---

### 2.3 Flashcards (Standard CRUD & SRS)

#### `GET /rest/v1/flashcards`
Pobieranie fiszek.

- **Parametry**: `?deck_id=eq.{uuid}&select=*`

#### `GET /rest/v1/flashcards?next_review_at=lte.now()&status=eq.active`
Pobieranie fiszek do **Sesji Nauki** (Due cards).

- **Opis**: Pobiera aktywne fiszki, których termin powtórki min¹³.
- **OdpowiedŸ**: Tablica obiektów `Flashcard`.

#### `POST /rest/v1/flashcards`
Dodawanie nowych fiszek (pojedynczo lub batch).

- **Body (JSON - Array)**:
  ```json
  [
    { "deck_id": "uuid", "front": "Pytanie", "back": "Odp", "status": "active" }
  ]
  ```
- **Sukces**: `201 Created`.

#### `PATCH /rest/v1/flashcards?id=eq.{uuid}`
Aktualizacja fiszki (edycja treœci LUB aktualizacja algorytmu SRS).

- **Scenariusz: Wynik Nauki (SRS)**:
  Klient (C#) oblicza nowe parametry i wysy³a:
  ```json
  {
    "next_review_at": "2023-11-01T10:00:00Z",
    "interval": 4,
    "ease_factor": 2.6,
    "repetition_count": 2
  }
  ```
- **Sukces**: `204 No Content`.

---

## 3. Uwierzytelnianie i Autoryzacja

- **Mechanizm**: JWT (Supabase Auth).
- **Wymaganie**: Ka¿de ¿¹danie musi zawieraæ nag³ówek `Authorization: Bearer <token>`.
- **Autoryzacja (RLS)**: Polityki bezpieczeñstwa bazy danych Postgres (`auth.uid() = user_id`) zapewniaj¹ izolacjê danych u¿ytkowników na poziomie wierszy.

## 4. Walidacja i Logika Biznesowa

### 4.1 Walidacja (Schema Constraints & API)
1.  **D³ugoœæ tekstu (AI)**: 1000 - 10000 znaków (walidowane w Edge Function).
2.  **Unikalnoœæ Nazwy Zestawu**: Unikalna w obrêbie u¿ytkownika (`409 Conflict`).
3.  **Wymagane Pola**: `front`, `back`, `deck_id` (NOT NULL).
4.  **Status**: Tylko przyjmowane wartoœci `'active'`, `'draft'`.

### 4.2 Kluczowa Logika Biznesowa
1.  **Orkiestracja AI**: Edge Function ukrywa klucz OpenRouter i loguje zu¿ycie (`input_length`, `total_generated_count`) do tabeli `generation_events`.
2.  **Algorytm Powtórek (SRS)**: Zaimplementowany po stronie klienta (Blazor/C#) z wykorzystaniem biblioteki open-source. API s³u¿y jedynie do persystencji wyliczonych dat (`next_review_at`) i parametrów (`interval`, `ease_factor`).
3.  **Kaskadowe Usuwanie**: Usuniêcie u¿ytkownika lub zestawu automatycznie czyœci powi¹zane zasoby dziêki `ON DELETE CASCADE` w bazie danych.
