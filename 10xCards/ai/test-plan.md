<plan_testow>

# Plan testów dla projektu 10xCards (Blazor WebAssembly)

## 1. Wprowadzenie i cele testowania
Celem testów jest potwierdzenie poprawności działania kluczowych funkcji aplikacji Blazor WebAssembly: autoryzacji, zarządzania zestawami i fiszkami, generowania fiszek AI oraz integracji z Supabase i OpenRouter. Testy mają zapewnić stabilność UI, niezawodność integracji API i poprawne obsługiwanie błędów.

## 2. Zakres testów
**W zakresie:**
- Autoryzacja i sesja użytkownika (`/auth/*`).
- Zarządzanie zestawami (`/decks`) i fiszkami (`/decks/{id}`).
- Generator fiszek AI (`/generate`).
- Integracje z Supabase (REST + Functions) i OpenRouter.
- Stany ładowania, błędy, paginacja, walidacje.

**Poza zakresem (na tym etapie):**
- Zaawansowana analityka i logowanie produkcyjne.
- Testy obciążeniowe na infrastrukturze produkcyjnej.

## 3. Typy testów
1. **Testy jednostkowe (NUnit 4.4.0)**
   - `DecksApiClient`, `FlashcardsApiClient`, `OpenRouterService`, `SupabaseAuthService`.
   - Walidacje wejścia, mapowanie błędów, obsługa wyjątków.
2. **Testy komponentów (Blazor)**
   - `Pages` i `Components` (np. `AuthForm`, `DeckList`, `FlashcardTable`).
   - Interakcje UI i renderowanie stanów.
3. **Testy integracyjne**
   - Supabase REST (decks/flashcards).
   - Supabase Functions (`generate-flashcards`).
   - OpenRouter API.
4. **Testy end-to-end (E2E)**
   - Scenariusze użytkownika w przeglądarce (np. Playwright).
5. **Testy regresji**
   - Krytyczne ścieżki: logowanie, CRUD, generowanie i zapis fiszek.
6. **Testy wydajnościowe (lekki profil)**
   - Czas renderowania i obsługi dużych tekstów na stronie generatora.

## 4. Scenariusze testowe kluczowych funkcjonalności
### 4.1 Autoryzacja
- Logowanie poprawnymi danymi ? przekierowanie do `/decks`.
- Logowanie błędnymi danymi ? komunikat z `SupabaseAuthService`.
- Rejestracja z niespójnym hasłem ? błąd walidacji.
- Reset hasła bez tokenu ? komunikat ostrzegawczy.
- Wygaśnięta sesja ? wymuszone logowanie przy wejściu na strony chronione.

### 4.2 Zestawy (Decks)
- Pobranie listy zestawów bez tokenu ? komunikat błędu.
- Utworzenie zestawu o prawidłowej nazwie ? pojawia się na liście.
- Utworzenie zestawu z nazwą zbyt długą ? błąd walidacji.
- Usunięcie zestawu ? usunięcie z listy i brak błędów.

### 4.3 Fiszki (Flashcards)
- Dodanie fiszki z pustymi polami ? błąd walidacji.
- Edycja fiszki ? aktualizacja danych.
- Usunięcie fiszki ? znika z listy.
- Paginacja: przełączanie stron i poprawny stan `PaginationState`.

### 4.4 Generator fiszek (AI)
- Tekst < 1000 znaków ? blokada generowania.
- Poprawny tekst ? generacja listy fiszek.
- Brak zaakceptowanych fiszek ? blokada zapisu.
- Zapis do istniejącego zestawu ? przekierowanie do `/decks/{id}`.
- Zapis do nowego zestawu ? tworzenie zestawu + zapis fiszek.

### 4.5 Integracje i błędy
- Supabase: błędy 401/403/404/500 mapowane na komunikaty.
- OpenRouter: retry, timeout, przekroczenie limitu.

## 5. Środowisko testowe
- .NET 10.0.2, Blazor WebAssembly.
- Środowisko testowe Supabase (oddzielny projekt).
- Klucze API OpenRouter w środowisku testowym.
- Przeglądarki: Chromium, Firefox, Edge.

## 6. Narzędzia do testowania
- **NUnit 4.4.0** – testy jednostkowe.
- **bUnit** – testy komponentów Blazor.
- **Playwright** – testy E2E.
- **Postman / REST Client** – testy API Supabase.
- **GitHub Actions** – uruchamianie testów CI.

## 7. Harmonogram testów
1. **Tydzień 1** – testy jednostkowe serwisów i API klientów.
2. **Tydzień 2** – testy komponentów UI.
3. **Tydzień 3** – testy integracyjne z Supabase/OpenRouter.
4. **Tydzień 4** – testy E2E i regresja.

## 8. Kryteria akceptacji testów
- 100% przejścia krytycznych scenariuszy: logowanie, CRUD, generowanie, zapis.
- Brak błędów blokujących (Severity 1).
- Pokrycie kluczowych serwisów testami jednostkowymi min. 70%.

## 9. Role i odpowiedzialności
- **QA Engineer**: przygotowanie planu, przypadków testowych, wykonanie testów.
- **Developerzy**: wsparcie w diagnozie błędów i poprawki.
- **Product Owner**: akceptacja kryteriów gotowości.

## 10. Procedury raportowania błędów
- Każdy błąd zgłaszany w systemie śledzenia (np. GitHub Issues).
- Minimalny zestaw informacji:
  - kroki reprodukcji,
  - oczekiwany vs rzeczywisty rezultat,
  - środowisko,
  - zrzuty ekranu/logi.
- Priorytetyzacja: P1 (blokujące), P2 (istotne), P3 (kosmetyczne).

## 11. Najbardziej wartościowe do testów jednostkowych są elementy logiki domenowej i walidacji, niezależne od UI:
•	DecksApiClient i FlashcardsApiClient – walidacje wejścia, budowanie zapytań, mapowanie błędów HTTP na wyjątki; łatwe do izolacji przez mock httpClient.
•	SupabaseAuthService – mapowanie błędów logowania, obsługa sesji i stanów użytkownika.
•	FlashcardGenerationService – walidacje żądania i mapowanie błędów wywołania funkcji.
•	OpenRouterService – walidacja requestu, retry/backoff, obsługa błędów i time-outów.
•	UserSessionState – logika stanu sesji (np. SetAuthenticated(string?), ClearSession(), UpdateFromSupabase(Supabase.Client)).
Te elementy mają deterministyczną logikę, więc unit testy szybko wykrywają regresje bez uruchamiania UI lub zależności zewnętrznych.


</plan_testow>
