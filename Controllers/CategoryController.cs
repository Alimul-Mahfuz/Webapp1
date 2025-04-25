using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[Authorize]
public class CategoryController : Controller
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public IActionResult Index(int Page = 1, string search = "")
    {
        int pageSize = 3;
        var categories = _context.Categories.AsQueryable();
        if (!String.IsNullOrEmpty(search))
        {
            categories = categories.Where(c => c.Name.Contains(search));
        }


        var totalCategories = categories.Count();
        var totalPages = (int)Math.Ceiling((double)totalCategories / pageSize);
        var paginatedCategories =
            categories.OrderByDescending(c => c.Id).Skip((Page - 1) * pageSize).Take(pageSize).ToList();
        var paginator = new Paginator<Category>()
        {
            Data = paginatedCategories,
            CurrentPage = Page,
            TotalPage = totalPages,
            TotalCount = totalCategories
        };
        return View(paginator);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        _context.Categories.Add(category);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}