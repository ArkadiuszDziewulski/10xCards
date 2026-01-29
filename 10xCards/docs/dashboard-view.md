# Widok Dashboard (Moje zestawy)

## Cel
Widok Dashboard prezentuje listê zestawów fiszek zalogowanego u¿ytkownika oraz umo¿liwia tworzenie i usuwanie zestawów.

## Routing
- `/` (domyœlny widok po zalogowaniu)
- `/decks`

## Komponenty
- `Pages/DashboardPage.razor` – kontener logiki i stanów widoku.
- `Components/Dashboard/DeckList.razor` – siatka zestawów.
- `Components/Dashboard/DeckCard.razor` – pojedynczy kafelek zestawu.
- `Components/Dashboard/CreateDeckModal.razor` – modal tworzenia zestawu.
- `Components/Dashboard/DeleteConfirmationModal.razor` – modal potwierdzenia usuniêcia.

## Integracja API
Widok korzysta z `DecksApiClient`:
- `GET /rest/v1/decks` – pobranie listy zestawów.
- `POST /rest/v1/decks` – utworzenie zestawu.
- `DELETE /rest/v1/decks?id=eq.{uuid}` – usuniêcie zestawu.

## Stany i obs³uga b³êdów
- £adowanie listy zestawów prezentuje spinner.
- B³êdy pobierania/operacji s¹ pokazywane w alertach.
- Pusty stan wyœwietla komunikat zachêcaj¹cy do utworzenia pierwszego zestawu.
