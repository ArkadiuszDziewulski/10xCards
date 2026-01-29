# Architektura UI dla 10xCards

## 1. Przegl¹d struktury UI

Aplikacja 10xCards zostanie zbudowana jako **Single Page Application (SPA)** przy u¿yciu **Blazor WebAssembly (.NET 10)**. Interfejs u¿ytkownika bêdzie oparty na frameworku **Bootstrap 5.3**, co zapewni responsywnoœæ (RWD) na urz¹dzeniach mobilnych i desktopowych.

G³ównym za³o¿eniem architektury jest podzia³ na **strefê publiczn¹** (Landing Page, Logowanie, Rejestracja) oraz **strefê prywatn¹** (Dashboard, Generator, Nauka), chronion¹ przez mechanizm uwierzytelniania Supabase.

Ze wzglêdu na specyfikê Blazor WASM, wiêkszoœæ logiki interakcji (w tym algorytm SRS) bêdzie wykonywana po stronie klienta, co minimalizuje opóŸnienia podczas sesji nauki.

## 2. Lista widoków

### 2.1. Strona Logowania / Rejestracji
- **Œcie¿ka:** `/login`, `/register`
- **G³ówny cel:** Uwierzytelnienie u¿ytkownika lub utworzenie nowego konta.
- **Kluczowe informacje:** Pola formularza (email, has³o), komunikaty b³êdów walidacji.
- **Kluczowe komponenty:** `AuthForm`, `ValidationSummary`.
- **Uwagi:** Obs³uga Supabase Auth. Przekierowanie do Dashboardu po sukcesie.

### 2.2. Dashboard (Moje Zestawy)
- **Œcie¿ka:** `/` (dla zalogowanych) lub `/decks`
- **G³ówny cel:** Centralny hub zarz¹dzania wiedz¹. Wyœwietlenie listy dostêpnych talii fiszek.
- **Kluczowe informacje:** Lista zestawów (nazwa, iloœæ fiszek, iloœæ fiszek "do powtórki" - jeœli dostêpne).
- **Kluczowe komponenty:**
  - `DeckList`: Siatka lub lista kafelków z zestawami.
  - `CreateDeckModal`: Modal do szybkiego tworzenia nowego pustego zestawu.
  - `StudyButton`: Przycisk przy zestawie kieruj¹cy do sesji nauki (jeœli s¹ fiszki do nauki).
- **UX/Akcje:** Dodaj zestaw, Usuñ zestaw (z potwierdzeniem), PrzejdŸ do detali zestawu, Ucz siê zestawu.

### 2.3. Generator Fiszek AI
- **Œcie¿ka:** `/generate`
- **G³ówny cel:** Konwersja tekstu w ustrukturyzowane fiszki przy pomocy LLM.
- **Kluczowe informacje:** Pole tekstowe na materia³ Ÿród³owy, lista wygenerowanych propozycji.
- **Kluczowe komponenty:**
  - `SourceInput`: Du¿e pole tekstowe z licznikiem znaków (1k-10k).
  - `GenerationLoader`: WskaŸnik postêpu (AI mo¿e chwilê pracowaæ).
  - `FlashcardReviewList`: Lista edytowalnych fiszek (Front/Back) z checkboxami "ZatwierdŸ".
  - `DeckSelector`: Dropdown wyboru istniej¹cego zestawu lub pole dla nowej nazwy.
- **UX:** Proces krokowy (1. Wklej -> 2. Generuj -> 3. Weryfikuj/Edytuj -> 4. Zapisz). Obs³uga b³êdów API (np. timeout, limit tokenów).

### 2.4. Szczegó³y Zestawu (Zarz¹dzanie Fiszkami)
- **Œcie¿ka:** `/decks/{deckId}`
- **G³ówny cel:** Przegl¹d, edycja i rêczne dodawanie fiszek w ramach konkretnego zestawu.
- **Kluczowe informacje:** Nazwa zestawu, lista wszystkich fiszek (przód/ty³/status).
- **Kluczowe komponenty:**
  - `FlashcardTable`: Tabela/Lista z paginacj¹.
  - `FlashcardEditor`: Modal lub tryb inline do edycji treœci fiszki.
  - `ManualAddButton`: Otwiera formularz dodawania nowej fiszki (Front/Back).
- **UX:** Filtrowanie/szukanie (opcjonalnie w MVP), usuwanie pojedynczych fiszek.

### 2.5. Sesja Nauki (Study Mode)
- **Œcie¿ka:** `/study/{deckId}`
- **G³ówny cel:** Wykonanie powtórek w oparciu o algorytm SRS.
- **Kluczowe informacje:** Treœæ aktualnej fiszki (przód), po ods³oniêciu - ty³ + przyciski oceny.
- **Kluczowe komponenty:**
  - `StudyCard`: Komponent wyœwietlaj¹cy fiszkê z animacj¹/prze³¹czeniem Front/Back.
  - `GradingBar`: Panel z przyciskami (np. Znowu, Trudne, Dobre, £atwe).
  - `SessionSummary`: Podsumowanie po zakoñczeniu fiszek w kolejce.
- **Bezpieczeñstwo/UX:** Brak mo¿liwoœci edycji w trakcie nauki (skupienie). Stan "Brak fiszek do nauki" jeœli u¿ytkownik wejdzie tu przez pomy³kê. Zapisywanie postêpu do API po ka¿dej ocenie lub w ma³ych paczkach.

### 2.6. Profil / Ustawienia
- **Œcie¿ka:** `/profile`
- **G³ówny cel:** Zarz¹dzanie kontem.
- **Kluczowe komponenty:** Przycisk "Wyloguj", Sekcja "Danger Zone" (Usuñ konto).

## 3. Mapa podró¿y u¿ytkownika

### Scenariusz G³ówny: Generowanie AI i Nauka
1.  **Start:** U¿ytkownik wchodzi na stronê, loguje siê.
2.  **Dashboard:** Widzi puste portfolio. Klika "Generuj Fiszki AI" w nawigacji.
3.  **Generator (Input):** Wkleja rozdzia³ z podrêcznika biologii. Wybiera "Generuj".
4.  **Generator (Review):** System zwraca 12 fiszek. U¿ytkownik odrzuca 2 b³êdne, poprawia literówkê w jednej.
5.  **Generator (Save):** W polu "Zestaw" wpisuje now¹ nazwê "Biologia - Komórka". Klika "Zapisz".
6.  **Przekierowanie:** System przenosi do `/decks/{newId}` (Szczegó³y zestawu).
7.  **Szczegó³y:** U¿ytkownik widzi utworzone fiszki. Mo¿e dodaæ rêcznie jedn¹ dodatkow¹.
8.  **Nauka:** U¿ytkownik klika "Rozpocznij naukê".
9.  **Sesja:**
    *   Widzi pytanie "Co to jest mitochondrium?".
    *   Myœli, klika "Poka¿ odpowiedŸ".
    *   Widzi odpowiedŸ, klika "Dobre" (obliczenie interwa³u w tle).
    *   Pojawia siê nastêpna fiszka.
10. **Koniec:** Po wyczerpaniu kolejki, widzi ekran gratulacyjny i wraca do Dashboardu.

## 4. Uk³ad i struktura nawigacji

Zastosowano uk³ad oparty na `MainLayout.razor` dla strefy uwierzytelnionej.

### Pasek Nawigacyjny (TopBar / Nabvar)
- **Logo / Nazwa aplikacji**: Link do Dashboardu.
- **Menu Desktop**:
  - "Moje Zestawy" (`/decks`)
  - "Generator AI" (`/generate`) - wyró¿niony wizualnie (CTA).
- **Menu U¿ytkownika (Prawa strona)**:
  - Adres email (lub awatar).
  - Dropdown: Profil, Wyloguj.

### Responsywnoœæ (Mobile)
- Menu zwijane do "hamburgera" na urz¹dzeniach < 768px.
- Widoki tabelaryczne (Szczegó³y Zestawu) zmieniaj¹ siê w widok "Karty/Listy" na mobile dla czytelnoœci.

## 5. Kluczowe komponenty

1.  **`AuthGuard` (Wrapper)**
    - Komponent owijaj¹cy treœæ stron wymagaj¹cych logowania. Przekierowuje do `/login` jeœli brak sesji `AuthenticationStateProvider`.

2.  **`FlashcardComponent` (Prezentacyjny)**
    - Reu¿ywalny komponent wizualizuj¹cy fiszkê (prostok¹t z cieniem, wyœrodkowany tekst).
    - Obs³uguje tryb "tylko odczyt" (lista) oraz "interaktywny" (sesja nauki - flip).

3.  **`DeckCard`**
    - Kafel reprezentuj¹cy zestaw fiszek na Dashboardzie. Zawiera badge z liczb¹ fiszek do powtórki (jeœli > 0).

4.  **`SRSController` (Logic helper)**
    - Klasa/Serwis C# wstrzykiwana do widoku nauki. Nie posiada UI. Odpowiada za przyjêcie oceny u¿ytkownika (1-4) i aktualnej fiszki, a nastêpnie zwrócenie zaktualizowanego obiektu fiszki z nowym `NextReviewAt`, `Interval` i `EaseFactor`.

5.  **`LoadingOverlay`**
    - Globalny lub lokalny spinner blokuj¹cy UI podczas operacji asynchronicznych (generowanie AI, zapisywanie do bazy).

6.  **`NotificationService` (Toast)**
    - P³ywaj¹ce powiadomienia (prawy górny róg) informuj¹ce o sukcesie ("Zestaw zapisany") lub b³êdzie ("B³¹d po³¹czenia z serverem").
