# Specyfikacja architektury modu³u autentykacji (US-001, US-002)

## 1. Architektura interfejsu u¿ytkownika

### Strony i layouty
- `Pages/Auth/Register.razor`
  - Formularz rejestracji z polami: e-mail, has³o, powtórzenie has³a.
  - Po sukcesie: automatyczne zalogowanie i przekierowanie do widoku generowania fiszek (gdy potwierdzenie e-mail jest wy³¹czone),
    w przeciwnym razie komunikat o koniecznoœci potwierdzenia adresu e-mail.
- `Pages/Auth/Login.razor`
  - Formularz logowania z polami: e-mail, has³o.
  - Po sukcesie: przekierowanie do widoku generowania fiszek.
- `Pages/Auth/ForgotPassword.razor`
  - Formularz odzyskiwania has³a: pole e-mail.
  - Po wys³aniu: komunikat o wys³aniu linku resetu.
- `Pages/Auth/ResetPassword.razor`
  - Formularz ustawienia nowego has³a (has³o, powtórzenie has³a).
  - Strona dostêpna po wejœciu z linku odzyskiwania (parametry tokenów w URL).
- `Layout/AuthLayout.razor`
  - Minimalny layout dla widoków auth (bez nawigacji do danych u¿ytkownika).
- `Layout/MainLayout.razor`
  - Rozszerzenie o stan zalogowania: widok u¿ytkownika, akcja wylogowania.

### Komponenty i odpowiedzialnoœci
- `Components/Auth/AuthForm.razor`
  - Wspólny szkielet formularzy: nag³ówek, sloty na pola, przycisk akcji, obs³uga stanu „loading”.
- `Components/Auth/AuthInput.razor`
  - Wspólna obs³uga walidacji i stylów Bootstrap dla pól e-mail/has³o.
- `Components/Auth/AuthErrorBanner.razor`
  - Ujednolicone wyœwietlanie b³êdów z backendu/Supabase.
- `Components/Auth/AuthGuard.razor`
  - Ochrona widoków wymagaj¹cych autoryzacji: przekierowanie do `Login`.
- `Components/Nav/UserMenu.razor`
  - Widok w `MainLayout`: e-mail u¿ytkownika + przycisk wylogowania.

### Walidacja i komunikaty b³êdów
- Walidacja client-side (Blazor DataAnnotations):
  - E-mail: format, wymagane.
  - Has³o: wymagane, minimalna d³ugoœæ zgodna z konfiguracj¹ Supabase (np. 8+ znaków).
  - Powtórzenie has³a: wymagane, identyczne jak has³o.
- Mapowanie b³êdów Supabase na komunikaty u¿ytkownika:
  - `invalid_login_credentials` ? „Nieprawid³owy e-mail lub has³o.”
  - `email_not_confirmed` ? „Konto wymaga potwierdzenia adresu e-mail.” (jeœli w³¹czone).
  - `user_already_registered` ? „Konto z tym adresem e-mail ju¿ istnieje.”
  - B³êdy sieci/timeout ? „Nie uda³o siê po³¹czyæ z us³ug¹. Spróbuj ponownie.”

### Scenariusze kluczowe
- Rejestracja: walidacja ? Supabase sign-up ? auto-login i przekierowanie (gdy brak wymogu potwierdzenia e-mail) lub komunikat o potwierdzeniu.
- Logowanie: walidacja ? Supabase sign-in ? zapis sesji ? przekierowanie.
- Odzyskiwanie has³a: walidacja e-mail ? Supabase reset ? komunikat.
- Wylogowanie: wyczyszczenie sesji ? przekierowanie do `Login`.
- Dostêp do widoków chronionych: `AuthGuard` sprawdza sesjê, w razie braku przekierowuje.

## 2. Logika backendowa

### Struktura kontraktów (modele)
- `Models/Auth/LoginRequest`
  - `Email`, `Password`.
- `Models/Auth/RegisterRequest`
  - `Email`, `Password`, `ConfirmPassword`.
- `Models/Auth/ForgotPasswordRequest`
  - `Email`.
- `Models/Auth/ResetPasswordRequest`
  - `Password`, `ConfirmPassword`, `AccessToken`.
- `Models/Auth/AuthResult`
  - `IsSuccess`, `ErrorCode`, `ErrorMessage`, `UserEmail`.

### API (wariant rekomendowany)
Blazor WebAssembly komunikuje siê bezpoœrednio z Supabase SDK w `Services/SupabaseAuthService`. Jeœli wymagane bêdzie ukrycie kluczy lub rozszerzenie logiki (np. audyt), dodaj lekki BFF:
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/logout`
- `POST /api/auth/forgot-password`
- `POST /api/auth/reset-password`

### Walidacja danych wejœciowych
- Walidacja po stronie klienta (DataAnnotations) i powtórna walidacja w serwisie auth:
  - format e-mail, minimalna d³ugoœæ has³a, zgodnoœæ has³a i potwierdzenia.
- Guard clauses w serwisach: natychmiastowy zwrot b³êdu gdy dane nie spe³niaj¹ kryteriów.

### Obs³uga wyj¹tków
- Ka¿de wywo³anie Supabase opakowane w `try/catch`.
- Mapowanie b³êdów na `AuthResult` z `ErrorCode` i przyjaznym opisem.
- Rejestrowanie b³êdów w logach aplikacji (np. `ILogger`).

## 3. System autentykacji (Supabase Auth + Astro)

### Supabase Auth
- Rejestracja: `Supabase.Client.Auth.SignUp(email, password)`.
- Logowanie: `Supabase.Client.Auth.SignIn(email, password)`.
- Wylogowanie: `Supabase.Client.Auth.SignOut()`.
- Odzyskiwanie has³a: `Supabase.Client.Auth.ResetPasswordForEmail(email, redirectUrl)`.
- Ustawienie nowego has³a: `Supabase.Client.Auth.UpdateUser(new Password = ...)` po otrzymaniu tokenu.

### Integracja w Blazor WASM
- `Services/SupabaseAuthService` odpowiada za ca³¹ komunikacjê z Supabase Auth.
- `Services/UserSessionState` przechowuje stan sesji u¿ytkownika i informuje UI o zmianach.
- `Components/Auth/AuthGuard` kontroluje dostêp do stron wymagaj¹cych logowania.

### Wspó³praca z Astro
- Jeœli w projekcie wystêpuje warstwa Astro (np. jako host dla statycznych stron lub gateway), powinna obs³u¿yæ:
  - Routing callback dla resetu has³a i potwierdzenia e-mail.
  - Przekierowanie do odpowiednich stron Blazor (`/auth/reset-password`).
- W przypadku braku Astro, obs³uga callback pozostaje wy³¹cznie po stronie Blazor WASM.

## Wnioski kluczowe
- Funkcjonalnoœæ rejestracji/logowania/odzyskiwania opiera siê o Supabase Auth bez zmiany obecnych przep³ywów aplikacji.
- UI rozdziela widoki auth i non-auth poprzez dedykowany `AuthLayout` i `MainLayout`.
- Komponenty auth s¹ wielokrotnego u¿ycia i izoluj¹ walidacjê oraz komunikaty b³êdów.
- Logika integracyjna znajduje siê w `Services`, z opcj¹ dodania lekkiego BFF jeœli wymagane bêdzie dodatkowe zabezpieczenie.
- Specyfikacja obejmuje historyjki US-001 i US-002; pozosta³e historyjki wymagaj¹ osobnych planów modu³owych.
