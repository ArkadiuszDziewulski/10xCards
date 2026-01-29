# Logowanie u¿ytkownika (Supabase Auth)

Widok logowania jest dostêpny pod `/auth/login` i korzysta z `SupabaseAuthService`, który wywo³uje `supabase.Auth.SignIn(email, password)`.

## Zachowanie po logowaniu
Po poprawnym zalogowaniu u¿ytkownik zostaje przekierowany do widoku zestawów (`/decks`). W przypadku b³êdnych danych lub problemów sieciowych wyœwietlany jest komunikat o b³êdzie.

## Konfiguracja
Upewnij siê, ¿e ustawiono:
- `Supabase:Url`
- `Supabase:Key`
