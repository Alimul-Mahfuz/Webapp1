using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class CategoryServiceImpl(AppDbContext context) : ICategoryService
{
    public List<Category> GetCategories()
    {
        return context.Categories.ToList();
    }

    public Paginator<Category> GetPaginatedCategories(int page = 1, string search = "")
    {
        const int pageSize = 3;
        var categories = context.Categories.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            categories = categories.Where(c => c.Name.Contains(search));
        }


        var totalCategories = categories.Count();
        var totalPages = (int)Math.Ceiling((double)totalCategories / pageSize);
        var paginatedCategories =
            categories.OrderByDescending(c => c.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new Paginator<Category>()
        {
            Data = paginatedCategories,
            CurrentPage = page,
            TotalPage = totalPages,
            TotalCount = totalCategories
        };
    }

    public async Task CreateCategory(Category category)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        return await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task UpdateCategory(Category category)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync();
    }
}