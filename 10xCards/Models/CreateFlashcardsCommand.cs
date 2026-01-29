namespace _10xCards.Models;

public sealed record CreateFlashcardsCommand
{
    public CreateFlashcardsCommand(IReadOnlyList<FlashcardCreateRequest> flashcards)
    {
        Flashcards = flashcards;
    }

    public IReadOnlyList<FlashcardCreateRequest> Flashcards { get; }
}
