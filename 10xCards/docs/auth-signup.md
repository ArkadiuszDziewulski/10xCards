# Rejestracja u¿ytkownika (Supabase Auth)

Widok rejestracji jest dostêpny pod `/auth/register` i korzysta z `supabase.Auth.SignUp`.

## Zachowanie po rejestracji
Supabase wysy³a e-mail z linkiem potwierdzaj¹cym. Konto jest aktywne dopiero po klikniêciu linku. W UI po rejestracji wyœwietlany jest komunikat o koniecznoœci potwierdzenia.

## Konfiguracja
Upewnij siê, ¿e ustawiono:
- `Supabase:Url`
- `Supabase:Key`
