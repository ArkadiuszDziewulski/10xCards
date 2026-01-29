# OpenRouter Service

## Konfiguracja

W pliku `wwwroot/appsettings.json` ustaw dane dostepu do OpenRouter:

```json
{
  "openrouter": {
    "Url": "https://openrouter.ai/api/v1",
    "Key": "sk-or-v1-REPLACE_WITH_KEY"
  }
}
```

- `Url` to bazowy adres API (serwis dodaje `/chat/completions`).
- `Key` przechowuj w zmiennych srodowiskowych podczas pracy produkcyjnej.

## Uzycie serwisu

Serwis `OpenRouterService` pozwala zbudowac zadanie przy pomocy `BuildRequest`, a nastepnie wyslac je metoda `SendChatAsync`.

Przyklad wiadomosci:

- system: `"Jestes pomocnym asystentem."`
- user: `"Wygeneruj podsumowanie."`

## Obsluga bledow

- Blad konfiguracji powoduje wyjatek przy inicjalizacji serwisu.
- Odpowiedzi `401/403`, `429` oraz `5xx` sa mapowane na czytelne komunikaty bledu.
- Dla bledow przejsciowych stosowany jest prosty mechanizm ponownych prob.
