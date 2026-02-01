# Generator fiszek AI

## Opis widoku

Widok `Generator fiszek AI` umo¿liwia wklejenie tekstu Ÿród³owego (1 000–10 000 znaków), wygenerowanie propozycji fiszek przez LLM, ich edycjê oraz zapis do wybranego zestawu.

Widok mo¿e zostaæ otwarty z poziomu szczegó³ów zestawu (`/decks/{deckId}`) przez akcjê `Dodaj fiszkê`. W takim przypadku wybór zestawu jest wstêpnie ustawiany na przekazany identyfikator.

## G³ówne elementy

- `SourceInput` z licznikiem znaków i walidacj¹ d³ugoœci.
- `GenerationLoader` wyœwietlany w trakcie generowania.
- `FlashcardReviewList` z edycj¹ i akceptacj¹ fiszek.
- `DeckSelector` dla wyboru istniej¹cego zestawu lub utworzenia nowego.
- `GenerationActions` z przyciskami `Generuj`, `Zapisz`, `Wyczyœæ`.

## Przep³yw zapisu

1. U¿ytkownik wybiera istniej¹cy zestaw lub podaje nazwê nowego.
2. Wygenerowane fiszki s¹ zapisywane dopiero po akceptacji u¿ytkownika.
3. Zaakceptowane fiszki s¹ zapisywane przez `FlashcardsApiClient`.
4. Po zapisie aplikacja przekierowuje do `/decks/{deckId}`.
