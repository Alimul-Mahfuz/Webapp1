using WebApplication1.Models;

namespace WebApplication1.Services;

public interface ICategoryService
{
    List<Category> GetCategories();
    Paginator<Category> GetPaginatedCategories(int page = 1, string search = "");
    Task CreateCategory(Category category);
    Task<Category?> GetCategoryById(int id);
    Task UpdateCategory(Category category);
}