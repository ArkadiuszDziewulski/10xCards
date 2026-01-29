<conversation_summary>
<decisions>
1. Przyjêto dwupoziomow¹ hierarchiê nawigacji: Lista Talii (`/decks`) -> Szczegó³y Talii (lista fiszek).
2. Proces generowania AI odbywa siê w dedykowanym widoku (`/generate`) z mechanizmem "Staging Area", gdzie fiszki s¹ weryfikowane w pamiêci RAM przed zapisem do bazy.
3. Synchronizacja postêpów nauki (SRS) bêdzie odbywaæ siê asynchronicznie ("fire-and-forget") po ocenie ka¿dej fiszki.
4. Edycja i tworzenie pojedynczych fiszek odbywa siê w oknach modalnych, aby nie gubiæ kontekstu talii.
5. Zastosowano paginacjê typu "Za³aduj wiêcej" (Load More) zamiast nieskoñczonego przewijania czy klasycznych stron.
6. Walidacja danych (np. limity znaków) jest duplikowana po stronie klienta, aby odci¹¿yæ API.
7. Zabezpieczono formularz generowania przed przypadkowym opuszczeniem za pomoc¹ `NavigationLock`.
8. Zdecydowano o obs³udze formatowania Markdown w treœci fiszek.
</decisions>
<matched_recommendations>
1. Struktura nawigacji g³ówniej (Lista talii jako widok zarz¹dczy).
2. Prezentacja wyników AI w widoku weryfikacji przed zapisem.
3. Asynchroniczna synchronizacja stanu SRS (po ka¿dej karcie).
4. Obs³uga d³ugotrwa³ych ¿¹dañ AI (Loading states).
5. Organizacja formularzy edycji w modalach.
6. Globalny system powiadomieñ (Toasty) dla informacji zwrotnych.
7. Responsywnoœæ sesji nauki (Thumb zone na mobile).
8. Call to Action w stanach pustych (Empty states).
9. Zarz¹dzanie sesj¹ Auth (AuthenticationStateProvider).
10. Walidacja po stronie klienta.
11. Paginacja "Za³aduj wiêcej".
12. Blokada nawigacji przy niezapisanych zmianach.
13. Renderowanie Markdown w fiszkach.
14. Lokalne filtrowanie i sortowanie talii (Search input).
15. Prezentacja interwa³ów SRS w formie relatywnej na przyciskach.
16. Potwierdzenie usuniêcia talii (Modal/Confirm).
17. Obs³uga b³êdów AI z mo¿liwoœci¹ ponowienia próby.
18. Edycja nazwy talii bezpoœrednio w nag³ówku szczegó³ów.
19. Zachowanie przycisku "Wstecz" przerywaj¹ce sesjê nauki.
20. WskaŸnik postêpu sesji.
</matched_recommendations>
<ui_architecture_planning_summary>
### G³ówne wymagania i struktura
Aplikacja Blazor WebAssembly oparta na dwupoziomowej nawigacji. G³ównym centrum dowodzenia jest widok "Moje talie". Aplikacja k³adzie du¿y nacisk na UX poprzez immediate feedback (Toasty), loading states przy operacjach AI oraz zabezpieczenie utraty danych (NavigationLock).

### Kluczowe widoki i przep³ywy
1. **Pulpit / Moje Talie (`/decks`)**:
   - Lista kafelkowa/wierszowa z paginacj¹ "Load More".
   - Lokalne filtrowanie.
   - Operacje CRUD na taliach (Usuwanie z potwierdzeniem).
2. **Szczegó³y Talii (`/decks/{id}`)**:
   - Lista fiszek w danej talii.
   - Modale do edycji i dodawania rêcznego ("szybkie akcje").
   - Wejœcie do trybu "Sesja Nauki".
3. **Generator AI (`/generate`)**:
   - Du¿e pole tekstowe z walidacj¹ d³ugoœci.
   - Stan ³adowania podczas komunikacji z Edge Function.
   - Obs³uga b³êdów z opcj¹ "Retry".
   - (Planowane) Wyœwietlenie wyników w formie listy do akceptacji (Staging).
4. **Sesja Nauki**:
   - Pe³noekranowy widok na mobile.
   - Du¿e przyciski oceny (SRS).
   - Pasek postêpu.

### Strategia Integracji i Stanu
- **Pobieranie danych**: Bezpoœrednie strza³y do PostgREST via `supabase-csharp`.
- **Generowanie AI**: POST do Edge Function -> Odbiór JSON -> Edycja w C# (RAM) -> Batch POST do bazy.
- **SRS**: Logika obliczania interwa³ów w C# (klient), tylko zapis dat (PATCH) idzie do API.
- **Auth**: Automatyczne odœwie¿anie tokenów przez bibliotekê klienta.

### Responsywnoœæ i Dostêpnoœæ
- Interfejs oparty o Bootstrap 5.
- Sesja nauki dostosowana do obs³ugi jedn¹ rêk¹ na telefonach.
- Semantyczne u¿ycie HTML i ikon dla czytelnoœci.
</ui_architecture_planning_summary>
<unresolved_issues>
1. Implementacja komponentu `GeneratedCardsModal` lub sekcji weryfikacji wyników w `Generate.razor` (obecnie kod zawiera placeholdery).
2. Szczegó³y implementacji klienta algorytmu SRS (SuperMemo/Anki) w C# - wymagana osobna klasa logiki biznesowej.
3. Wybór konkretnej biblioteki do renderowania Markdown w Blazor (np. Markdig).
4. Implementacja globalnego serwisu Toast/Notification (obecnie brak w kodzie).
5. Pe³na obs³uga uwierzytelniania w `App.razor` (np. `AuthorizeRouteView`), aby chroniæ widoki `/decks` i `/generate` przed niezalogowanymi u¿ytkownikami.
</unresolved_issues>
</conversation_summary>