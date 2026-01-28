# Plan Schematu Bazy Danych - 10xCards

Ten dokument zawiera specyfikacjê schematu bazy danych (PostgreSQL) dla aplikacji 10xCards, opart¹ na wymaganiach PRD, notatkach z sesji planowania oraz stosie technologicznym Supabase.

## 1. Lista tabel

### 1.1 `public.profiles`
Tabela rozszerzaj¹ca systemow¹ tabelê `auth.users` w Supabase. Zarz¹dzana automatycznie przy rejestracji u¿ytkownika (trigger).

| Kolumna | Typ Danych | Ograniczenia (Constraints) | Opis |
| :--- | :--- | :--- | :--- |
| `id` | UUID | PK, FK -> `auth.users(id)`, ON DELETE CASCADE | Klucz g³ówny to¿samy z ID u¿ytkownika Supabase Auth lub Identity. |
| `created_at` | TIMESTAMPTZ | DEFAULT `now()`, NOT NULL | Data utworzenia profilu. |
| `updated_at` | TIMESTAMPTZ | DEFAULT `now()`, NOT NULL | Data ostatniej aktualizacji. |

### 1.2 `public.decks`
Kolekcje (zestawy) fiszek u¿ytkownika.

| Kolumna | Typ Danych | Ograniczenia (Constraints) | Opis |
| :--- | :--- | :--- | :--- |
| `id` | UUID | PK, DEFAULT `gen_random_uuid()` | Unikalny identyfikator zestawu. |
| `user_id` | UUID | FK -> `profiles(id)`, NOT NULL, ON DELETE CASCADE | W³aœciciel zestawu. |
| `name` | TEXT | NOT NULL | Nazwa zestawu. |
| `created_at` | TIMESTAMPTZ | DEFAULT `now()`, NOT NULL | Data utworzenia. |
| `updated_at` | TIMESTAMPTZ | DEFAULT `now()`, NOT NULL | Data modyfikacji. |

**Ograniczenia tabeli:**
- `UNIQUE (user_id, name)`: U¿ytkownik nie mo¿e mieæ dwóch zestawów o tej samej nazwie.

### 1.3 `public.flashcards`
G³ówna tabela przechowuj¹ca treœæ fiszek oraz metadane algorytmu SRS (Spaced Repetition System).

| Kolumna | Typ Danych | Ograniczenia (Constraints) | Opis |
| :--- | :--- | :--- | :--- |
| `id` | UUID | PK, DEFAULT `gen_random_uuid()` | Unikalny identyfikator fiszki. |
| `deck_id` | UUID | FK -> `decks(id)`, NOT NULL, ON DELETE CASCADE | Zestaw, do którego nale¿y fiszka. |
| `user_id` | UUID | FK -> `profiles(id)`, NOT NULL, ON DELETE CASCADE | W³aœciciel (denormalizacja dla wydajnoœci RLS). |
| `front` | TEXT | NOT NULL | Treœæ pytania / przód fiszki. |
| `back` | TEXT | NOT NULL | Treœæ odpowiedzi / ty³ fiszki. |
| `status` | TEXT | NOT NULL, DEFAULT 'active', CHEQUE (status IN ('active', 'draft')) | Status fiszki. |
| `next_review_at` | TIMESTAMPTZ | NULLABLE | Data nastêpnej powtórki wg algorytmu. NULL oznacza now¹ fiszkê. |
| `interval` | INTEGER | NOT NULL, DEFAULT 0 | Obecny interwa³ powtórki (w dniach). |
| `ease_factor` | REAL | NOT NULL, DEFAULT 2.5 | Wspó³czynnik ³atwoœci (metryka SRS, np. EF w SM-2). |
| `repetition_count`| INTEGER | NOT NULL, DEFAULT 0 | Liczba powtórzeñ z rzêdu. |
| `created_at` | TIMESTAMPTZ | DEFAULT `now()`, NOT NULL | Data utworzenia. |
| `updated_at` | TIMESTAMPTZ | DEFAULT `now()`, NOT NULL | Data edycji. |

### 1.4 `public.generation_events`
Logi generowania fiszek przez AI w celu analityki i monitorowania zu¿ycia (bez treœci fiszek).

| Kolumna | Typ Danych | Ograniczenia (Constraints) | Opis |
| :--- | :--- | :--- | :--- |
| `id` | UUID | PK, DEFAULT `gen_random_uuid()` | Identyfikator zdarzenia generowania. |
| `user_id` | UUID | FK -> `profiles(id)`, NOT NULL, ON DELETE CASCADE | U¿ytkownik zlecaj¹cy generowanie. |
| `target_deck_id`| UUID | FK -> `decks(id)`, NULLABLE, ON DELETE SET NULL | Zestaw docelowy (jeœli wybrano/utworzono). |
| `input_length` | INTEGER | NOT NULL | D³ugoœæ tekstu wejœciowego (liczba znaków). |
| `total_generated_count` | INTEGER | NOT NULL | Liczba fiszek zaproponowanych przez AI. |
| `accepted_count` | INTEGER | NOT NULL, DEFAULT 0 | Liczba fiszek ostatecznie zaakceptowanych przez u¿ytkownika. |
| `created_at` | TIMESTAMPTZ | DEFAULT `now()`, NOT NULL | Czas operacji. |

## 2. Relacje miêdzy tabelami

- **`profiles` (1) : (N) `decks`**
  - Jeden u¿ytkownik mo¿e posiadaæ wiele zestawów.
  - Usuniêcie profilu kaskadowo usuwa zestawy.

- **`profiles` (1) : (N) `flashcards`**
  - Relacja bezpoœrednia dla u³atwienia RLS.
  - Ka¿da fiszka jest przypisana do jednego u¿ytkownika.

- **`decks` (1) : (N) `flashcards`**
  - Zestaw zawiera wiele fiszek.
  - Fiszka musi nale¿eæ do zestawu.
  - Usuniêcie zestawu kaskadowo usuwa zawarte w nim fiszki.

- **`profiles` (1) : (N) `generation_events`**
  - Historia generowania jest przypisana do u¿ytkownika.

## 3. Indeksy (Wydajnoœæ)

Aby zapewniæ skalowalnoœæ i szybkie dzia³anie zapytañ, zostan¹ utworzone nastêpuj¹ce indeksy:

1.  **Indeksy kluczy obcych i g³ównych filtrów:**
    - `decks_user_id_idx` ON `decks (user_id)`
    - `flashcards_deck_id_idx` ON `flashcards (deck_id)`
    - `flashcards_user_id_idx` ON `flashcards (user_id)`

2.  **Indeksy operacyjne (SRS i Nauka):**
    - `flashcards_next_review_idx` ON `flashcards (next_review_at)` - krytyczny dla pobierania fiszek do sesji nauki ("daj mi fiszki na dziœ").

3.  **Indeksy porz¹dkowe:**
    - `decks_created_at_idx` ON `decks (created_at DESC)` - dla domyœlnego sortowania zestawów.

## 4. Zasady PostgreSQL (Row Level Security - RLS)

Wszystkie tabele bêd¹ mia³y w³¹czone RLS (`ALTER TABLE ... ENABLE ROW LEVEL SECURITY`).

Polityka bezpieczeñstwa jest jednolita dla wszystkich tabel (`profiles`, `decks`, `flashcards`, `generation_events`):
- **Zasada:** U¿ytkownik ma dostêp tylko do wierszy, gdzie `user_id` jest równe jego `auth.uid()`.
- **Operacje:** `SELECT`, `INSERT`, `UPDATE`, `DELETE`.

**Szczegó³y implementacji (SQL Policy):**
```sql
CREATE POLICY "Users can manage their own [table_name]"
ON public.[table_name]
FOR ALL
USING (auth.uid() = user_id)
WITH CHECK (auth.uid() = user_id);
```

Dla tabeli `profiles`:
- Policy: "Users can view own profile" (`auth.uid() = id`), "Users can update own profile".
- Insert jest zazwyczaj obs³ugiwany przez trigger systemowy (Database trigger on `auth.users`).

## 5. Dodatkowe uwagi projektowe

- **Generacja UUID**: Wykorzystanie funkcji PostgreSQL `gen_random_uuid()` (dostêpnej natywnie od Postgres 13+) eliminuje koniecznoœæ generowania ID po stronie aplikacji.
- **Optymalizacja RLS**: Dodanie kolumny `user_id` do tabeli `flashcards` (mimo posiadania `deck_id`) jest celow¹ denormalizacj¹. Pozwala to na prostsz¹ i szybsz¹ weryfikacjê uprawnieñ bez koniecznoœci wykonywania `JOIN` z tabel¹ `decks` przy ka¿dym zapytaniu o fiszkê.
- **Status Fiszki**: Pole `status` pozwala na przysz³e rozszerzenia (np. 'archived', 'suspended'), a w MVP odró¿nia fiszki aktywne w nauce od szkiców.
- **SRS**: Pola `ease_factor`, `interval`, `repetition_count` s¹ zgodne z popularnymi algorytmami typu SuperMemo-2 (SM-2), co u³atwi integracjê z bibliotekami open-source w C#.
