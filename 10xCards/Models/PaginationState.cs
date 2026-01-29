namespace _10xCards.Models;

public sealed class PaginationState
{
    public int PageSize { get; set; } = 10;
    public int CurrentPage { get; set; } = 1;
    public int TotalItems { get; set; }

    public int TotalPages
    {
        get
        {
            if (PageSize <= 0)
            {
                return 1;
            }

            return Math.Max(1, (int)Math.Ceiling((double)TotalItems / PageSize));
        }
    }
}
