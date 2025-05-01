using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class CategoryServiceImpl:ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryServiceImpl(AppDbContext context)
    {
        _context = context;
    }
    
    public List<Category> GetCategories()
    {
        return _context.Categories.ToList();   
    }
}