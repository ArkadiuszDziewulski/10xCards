<conversation_summary> <decisions>
1.	Utworzenie tabeli decks do grupowania fiszek.
2.	Wymóg braku zapisywania treœci odrzuconych fiszek w bazie danych.
3.	Zastosowanie standardowego usuwania rekordów (DELETE) zamiast "soft delete".
4.	Powi¹zanie tabeli profiles z auth.users (Supabase Auth).
5.	Zastosowanie UUID jako standardu kluczy g³ównych.
6.	Wdro¿enie Row Level Security (RLS) dla wszystkich tabel.
7.	Przechowywanie metadanych algorytmu SRS bezpoœrednio w tabeli flashcards.
8.	Zliczanie statystyk generowania (wygenerowane vs zaakceptowane) w tabeli generation_events zamiast przechowywania odrzuconych rekordów.
9.	Wymuszenie przypisania fiszki do zestawu (brak "sierot").
10.	Unikalnoœæ nazw zestawów w obrêbie u¿ytkownika. </decisions>
<matched_recommendations>
1.	Utworzenie tabeli public.profiles z relacj¹ 1:1 do auth.users i kaskadowym usuwaniem.
2.	Struktura tabeli flashcards zawieraj¹ca pola front, back (szczegó³y treœci) oraz pola SRS (next_review_at, interval, ease_factor).
3.	Struktura tabeli decks z relacj¹ 1:N do flashcards i kluczem obcym deck_id w tabeli fiszek.
4.	Struktura tabeli generation_events zawieraj¹ca liczniki (total_generated_count, accepted_count) oraz odniesienie do target_deck_id.
5.	Indeksowanie kolumn user_id (wszystkie tabele), deck_id (fiszki) oraz next_review_at (fiszki).
6.	U¿ycie typu danych TEXT dla treœci fiszek.
7.	Konfiguracja polityk RLS sprawdzaj¹cych auth.uid() = user_id.
8.	Constraint UNIQUE (user_id, name) dla tabeli decks.
9.	Kaskadowe usuwanie fiszek przy usuniêciu zestawu (ON DELETE CASCADE).
10.	Dodatkowo kolumna user_id w tabeli flashcards dla optymalizacji RLS, mimo relacji z decks. </matched_recommendations>
<database_planning_summary> Na podstawie wymagañ PRD oraz ustaleñ z rozmowy, plan bazy danych dla MVP oparty na PostgreSQL (Supabase) prezentuje siê nastêpuj¹co:
G³ówne wymagania schematu: Baza danych musi obs³ugiwaæ u¿ytkowników, zarz¹dzanie treœci¹ edukacyjn¹ (zestawy i fiszki), œledzenie historii generowania treœci przez AI oraz obs³ugê algorytmu powtórek (SRS).
Kluczowe encje i relacje:
1.	profiles: Tabela u¿ytkownika rozszerzaj¹ca auth.users.
•	Relacja: 1:1 z auth.users.
2.	decks: Tabela grupuj¹ca fiszki.
•	Pola: id (UUID), user_id (FK), name, created_at.
•	Limit: Unikalna nazwa per u¿ytkownik.
3.	flashcards: G³ówna encja z treœci¹.
•	Pola: id (UUID), user_id (FK), deck_id (FK), front, back, status (active/draft), metadane SRS (next_review_at, interval, ease_factor, repetition_count).
•	Relacja: N:1 z decks (musi nale¿eæ do zestawu), N:1 z profiles.
4.	generation_events: Historia u¿ycia AI.
•	Pola: id, user_id, input_length, total_generated_count, accepted_count, target_deck_id.
•	Cel: Analiza kosztów i skutecznoœci AI bez przechowywania zbêdnych danych (odrzuconych fiszek).
Bezpieczeñstwo i skalowalnoœæ:
•	RLS: Kluczowe dla separacji danych. Ka¿da tabela posiada kolumnê user_id, co umo¿liwia proste i szybkie polityki bezpieczeñstwa (auth.uid() = user_id).
•	Indeksowanie: Indeksy na kluczach obcych (deck_id, user_id) oraz polach czêsto filtrowanych (next_review_at do harmonogramu nauki).
•	Typy danych: U¿ycie TEXT zapewnia elastycznoœæ dla treœci, a UUID u³atwia skalowanie i migracje danych.
Integralnoœæ danych:
•	Usuniêcie konta usuwa wszystkie dane u¿ytkownika (Cascade).
•	Usuniêcie zestawu usuwa wszystkie fiszki w nim zawarte. </database_planning_summary>
<unresolved_issues> Brak istotnych nierozwi¹zanych kwestii. Schemat jest gotowy do implementacji w SQL. Nale¿y jedynie upewniæ siê przy implementacji backendu, ¿e wybrana biblioteka SRS w C# akceptuje nazewnictwo kolumn przyjête w bazie (np. ease_factor vs ef). </unresolved_issues> </conversation_summary>