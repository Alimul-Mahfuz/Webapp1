namespace WebApplication1.Models;

public class Paginator<T>
{
    public List<T> Data { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPage { get; set; }
    public int TotalCount { get; set; }

    public string? SearchTerm { get; set; }

    public string? GetNextPageUrl()
    {
        return CurrentPage < TotalPage ? $"?page={CurrentPage + 1}&search={SearchTerm}" : null;
    }

    public string? GetPreviousPageUrl()
    {
        return CurrentPage > 1 ? $"?page={CurrentPage - 1}&search={SearchTerm}" : null;
    }
}