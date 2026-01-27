<conversation_summary>
<decisions>
1. **Struktura danych:** Fiszki bêd¹ organizowane w konkretne "Zestawy" (Decks), a nie jako p³aska lista.
2. **Profil u¿ytkownika:** Aplikacja nie wymaga przechowywania dodatkowych danych o u¿ytkowniku (brak tabeli publicznej `profiles`, oparcie o `auth.users`).
3. **Przechowywanie propozycji AI:** Wygenerowane, ale niezaakceptowane propozycje fiszek bêd¹ przechowywane wy³¹cznie w pamiêci przegl¹darki (brak tabeli tymczasowej w DB).
4. **Obs³uga b³êdów AI:** B³êdy API bêd¹ logowane w konsoli przegl¹darki, nie w bazie danych.
5. **Zakres algorytmu:** Algorytm powtórek ma dzia³aæ w kontekœcie u¿ytkownika (globalnie dla jego fiszek).
6. **Unikalnoœæ:** Nazwy zestawów musz¹ byæ unikalne w obrêbie u¿ytkownika.
7. **Integralnoœæ danych (Usuwanie):** Nie mo¿na usun¹æ zestawu, jeœli zawiera fiszki (Relacja RESTRICT, nie CASCADE dla zestawów).
8. **Generowanie:** Brak sztywnego limitu iloœci generowanych pytañ w bazie danych.
</decisions>

<matched_recommendations>
1. **Relacje:** Relacja Jeden-do-Wielu miêdzy Zestawem a Fiszk¹.
2. **Algorytm Spaced Repetition:** Przechowywanie parametrów algorytmu (next_review, ease_factor) bezpoœrednio w tabeli `flashcards`.
3. **Analityka AI:** Utworzenie tabeli `ai_generation_events` do œledzenia metryk (iloœæ wygenerowanych/zaakceptowanych).
4. **Historia nauki:** Utworzenie tabeli `study_logs` do zapisywania historii powtórek.
5. **Typy danych:** U¿ycie `UUID` dla kluczy g³ównych, `text` dla treœci fiszek, `timestamptz` (UTC) dla dat.
6. **Bezpieczeñstwo:** W³¹czenie RLS (Row Level Security) na wszystkich tabelach z wymogiem `auth.uid() = user_id`.
7. **Indeksowanie:** Zastosowanie indeksów z³o¿onych (np. `user_id` + `next_review_at`) dla optymalizacji sesji nauki.
8. **Atrybuty zestawu:** Tabela zestawów zawiera `name` i `description`.
</matched_recommendations>

<database_planning_summary>
### G³ówne wymagania dotycz¹ce schematu
Schemat bazy danych dla MVP 10xCards w Supabase bêdzie sk³ada³ siê ze schematu `public` z silnym naciskiem na RLS. System opiera siê na relacji u¿ytkownik -> zestawy -> fiszki. Dane o stanie nauki s¹ wbudowane w fiszki, a historia jest logowana osobno.

### Kluczowe encje i relacje
1.  **Zestawy (`decks`)**:
    *   Kolumny: `id` (UUID), `user_id` (FK), `name`, `description`, `created_at`.
    *   Relacja: Nale¿y do U¿ytkownika. Ma wiele Fiszek.
    *   Ograniczenia: Unikalnoœæ pary (`user_id`, `name`). Usuniêcie mo¿liwe tylko gdy pusty (RESTRICT).
2.  **Fiszki (`flashcards`)**:
    *   Kolumny: `id` (UUID), `deck_id` (FK), `user_id` (FK), `front`, `back`, `next_review_at`, `interval_days`, `ease_factor`, `source_generation_id` (FK nullable), `is_active` (bool).
    *   Relacja: Nale¿y do Zestawu. Powi¹zana z `ai_generation_events` (opcjonalnie).
3.  **Zdarzenia AI (`ai_generation_events`)**:
    *   Kolumny: `id` (UUID), `user_id` (FK), `created_at`, `model_name`, `input_char_count`.
    *   Cel: Analityka zu¿ycia i konwersji (u¿ywane przez pole `source_generation_id` w fiszkach).
4.  **Logi nauki (`study_logs`)**:
    *   Kolumny: `id` (UUID), `user_id` (FK), `flashcard_id` (FK), `grade`, `reviewed_at`.
    *   Cel: Historia postêpów, niezmienna (append-only).

### Bezpieczeñstwo i Skalowalnoœæ
*   **RLS**: Ka¿da tabela musi posiadaæ politykê sprawdzaj¹c¹ `auth.uid()`.
*   **Wydajnoœæ**: Indeksy na polach uczestnicz¹cych w filtrowaniu algorytmu powtórek (`user_id`, `next_review_at`, `is_active`).
*   **Identyfikatory**: U¿ycie UUID zapewnia ³atw¹ skalowalnoœæ i migracjê danych w przysz³oœci.

</database_planning_summary>

<unresolved_issues>
1.  **Automatyzacja Startowa:** Szczegó³y implementacji triggera tworz¹cego "Domyœlny zestaw" dla nowego u¿ytkownika (omówione w rekomendacjach, ale niepotwierdzone ostatecznym wyborem u¿ytkownika).
2.  **Zaawansowane Wyszukiwanie:** Decyzja o wdro¿eniu indeksów GIN (`pg_trgm`) do wyszukiwania pe³notekstowego (rekomendacja z ostatniej rundy bez wyraŸnego potwierdzenia).
3.  **Precyzja typów liczbowych:** Ostateczne potwierdzenie typu dla algorytmu (`real` vs `double precision`), przyjêto domyœlnie rekomendacjê `real`.
</unresolved_issues>
</conversation_summary>