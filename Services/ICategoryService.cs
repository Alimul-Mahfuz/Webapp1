using WebApplication1.Models;

namespace WebApplication1.Services;

public interface ICategoryService
{
    List<Category> GetCategories();
}