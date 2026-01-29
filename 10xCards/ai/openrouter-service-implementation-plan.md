# Opis us³ugi

Us³uga `OpenRouterService` odpowiada za komunikacjê z API OpenRouter w celu generowania odpowiedzi LLM dla czatów w aplikacji Blazor WebAssembly. Wykorzystuje konfiguracjê z `wwwroot/appsettings.json` oraz zapewnia ustandaryzowany sposób wysy³ania komunikatów systemowych i u¿ytkownika, konfiguracji parametrów modelu oraz obs³ugi odpowiedzi ustrukturyzowanych.

# Opis konstruktora

Konstruktor inicjalizuje us³ugê na podstawie konfiguracji i zale¿noœci infrastrukturalnych.

- Wstrzykiwane zale¿noœci:
  - `HttpClient` skonfigurowany do wywo³añ API.
  - `IConfiguration` do pobrania `openrouter:Url` oraz `openrouter:Key`.
  - (Opcjonalnie) `ILogger<OpenRouterService>` do logowania b³êdów.
- Walidacja konfiguracji:
  - Sprawdzenie obecnoœci i poprawnoœci `Url` oraz `Key`.
  - Wczesne przerwanie i jawny b³¹d, jeœli konfiguracja jest nieprawid³owa.

# Publiczne metody i pola

1. `Task<OpenRouterResponse> SendChatAsync(OpenRouterRequest request, CancellationToken ct)`
   - Wysy³a ¿¹danie czatu do OpenRouter.
   - Obs³uguje komunikaty systemowe i u¿ytkownika.
   - Pozwala przekazaæ `response_format` dla odpowiedzi JSON zgodnej ze schematem.

2. `OpenRouterRequest BuildRequest(...)`
   - Pomocnicze tworzenie kompletnego ¿¹dania z wartoœciami domyœlnymi.
   - Umo¿liwia ustawienie modelu i parametrów modelu.

3. `OpenRouterConfig Config` (tylko do odczytu)
   - Zawiera `Url`, `Key` oraz wartoœci domyœlne (np. model, timeout).

# Prywatne metody i pola

1. `HttpRequestMessage CreateHttpRequest(OpenRouterRequest request)`
   - Buduje ¿¹danie HTTP z poprawnymi nag³ówkami (`Authorization`, `HTTP-Referer`, `X-Title`).

2. `Task<OpenRouterResponse> ParseResponseAsync(HttpResponseMessage response)`
   - Mapuje odpowiedŸ API do modelu domenowego.
   - Waliduje poprawnoœæ JSON oraz wymaganych pól.

3. `void ValidateRequest(OpenRouterRequest request)`
   - Sprawdza model, komunikaty oraz ustawienia `response_format`.

# Obs³uga b³êdów

## Scenariusze b³êdów

1. Brak konfiguracji `openrouter:Url` lub `openrouter:Key`.
2. Nieprawid³owy format odpowiedzi JSON.
3. Brak wymaganych pól w odpowiedzi (np. brak `choices`).
4. B³¹d sieci (timeout, DNS, brak po³¹czenia).
5. OdpowiedŸ `401/403` (b³êdny klucz API).
6. OdpowiedŸ `429` (limit rate).
7. OdpowiedŸ `5xx` (b³¹d serwera OpenRouter).

## Podejœcie

- Stosuj wczesne walidacje i jednoznaczne komunikaty b³êdów.
- Loguj kontekst b³êdu bez ujawniania kluczy API.
- Zwracaj u¿ytkownikowi bezpieczne i krótkie komunikaty b³êdów.

# Kwestie bezpieczeñstwa

1. Nie logowaæ kluczy API ani pe³nych treœci wra¿liwych.
2. Przechowywaæ klucz API wy³¹cznie w konfiguracji i zmiennych œrodowiskowych.
3. Walidowaæ dane wejœciowe, aby unikn¹æ wstrzykniêæ w treœæ promptu.
4. Stosowaæ `CancellationToken` do kontrolowania timeoutów.

# Plan wdro¿enia krok po kroku

1. **Utwórz modele w `Models`**
   - `OpenRouterRequest`, `OpenRouterResponse`, `OpenRouterMessage`, `OpenRouterResponseFormat`.
   - Uwzglêdnij pole `response_format` zgodne ze schematem API.

2. **Utwórz konfiguracjê w `Services`**
   - Klasa `OpenRouterConfig` mapuj¹ca `openrouter:Url` oraz `openrouter:Key`.

3. **Utwórz us³ugê w `Services`**
   - `OpenRouterService` z metod¹ `SendChatAsync`.
   - Wykorzystuj `HttpClient` z domyœlnymi nag³ówkami.

4. **Implementuj obs³ugê komunikatów**
   - Komunikat systemowy i u¿ytkownika jako kolejne elementy listy `messages`.

5. **Implementuj `response_format`**
   - U¿yj formatu:
     - `{ type: "json_schema", json_schema: { name: "ChatResponse", strict: true, schema: { ... } } }`
   - Weryfikuj, czy schemat jest poprawny i serializowalny.

6. **Dodaj konfiguracjê modelu i parametrów**
   - Ustaw `model` oraz opcje (`temperature`, `max_tokens`, `top_p`).

7. **Dodaj obs³ugê b³êdów i logowanie**
   - Obs³u¿ kody odpowiedzi HTTP oraz b³êdy deserializacji.

8. **Dodaj testowe wywo³anie w warstwie UI**
   - U¿yj Blazor do wywo³ania `SendChatAsync`.

9. **Zweryfikuj konfiguracjê**
   - SprawdŸ poprawnoœæ `wwwroot/appsettings.json`.

10. **Przygotuj dokumentacjê w `docs`**
   - Opisz konfiguracjê i przyk³ady zapytañ.

# Szczegó³y implementacyjne komponentów OpenRouter

## 1. Komponenty i cele

1. **Klient HTTP** — wysy³anie ¿¹dañ do API OpenRouter.
2. **Model ¿¹dania** — mapowanie danych na format wymagany przez API.
3. **Model odpowiedzi** — mapowanie odpowiedzi na format aplikacji.
4. **Konfiguracja** — przechowywanie URL i klucza API.
5. **Warstwa serwisowa** — logika tworzenia i walidacji zapytañ.

## 2. Szczegó³y funkcjonalnoœci i wyzwañ

### 1. Klient HTTP
- Funkcjonalnoœæ: wysy³anie ¿¹dañ POST do `/chat/completions`.
- Wyzwania:
  1. Time-outy i b³êdy sieci.
  2. Brak autoryzacji.
- Rozwi¹zania:
  1. Ustawienie `Timeout` oraz obs³uga `CancellationToken`.
  2. Wstrzykiwanie poprawnego klucza API w nag³ówku.

### 2. Model ¿¹dania
- Funkcjonalnoœæ: przechowywanie `messages`, `model`, `response_format`.
- Wyzwania:
  1. Niekompletne dane wejœciowe.
  2. Niepoprawny `response_format`.
- Rozwi¹zania:
  1. Walidacja przed wys³aniem.
  2. Weryfikacja struktury schematu JSON.

### 3. Model odpowiedzi
- Funkcjonalnoœæ: mapowanie `choices` i treœci odpowiedzi.
- Wyzwania:
  1. Nieoczekiwany format JSON.
- Rozwi¹zania:
  1. Obs³uga wyj¹tków deserializacji.

### 4. Konfiguracja
- Funkcjonalnoœæ: dostêp do `Url` i `Key`.
- Wyzwania:
  1. Brak wartoœci konfiguracyjnych.
- Rozwi¹zania:
  1. Walidacja podczas inicjalizacji serwisu.

### 5. Warstwa serwisowa
- Funkcjonalnoœæ: logika biznesowa wywo³añ LLM.
- Wyzwania:
  1. Wysoki koszt zapytañ.
- Rozwi¹zania:
  1. Ustawienie limitów oraz kontrola parametrów modelu.

# Obs³uga elementów API OpenRouter

1. **Komunikat systemowy**
   - Przyk³ad: `{"role": "system", "content": "Jesteœ pomocnym asystentem."}`
2. **Komunikat u¿ytkownika**
   - Przyk³ad: `{"role": "user", "content": "Wygeneruj podsumowanie."}`
3. **response_format**
   - Przyk³ad:
     - `{ "type": "json_schema", "json_schema": { "name": "ChatResponse", "strict": true, "schema": { "type": "object", "properties": { "answer": { "type": "string" } }, "required": ["answer"], "additionalProperties": false } } }`
4. **Nazwa modelu**
   - Przyk³ad: `"model": "openai/gpt-4.1"`
5. **Parametry modelu**
   - Przyk³ad: `"temperature": 0.2`, `"max_tokens": 400`, `"top_p": 0.9`
