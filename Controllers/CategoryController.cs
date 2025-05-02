using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[Authorize]
public class CategoryController(ICategoryService categoryService) : Controller
{
    [HttpGet]
    public IActionResult Index(int Page = 1, string search = "")
    {
        var categories = categoryService.GetPaginatedCategories(Page, search);
        return View(categories);
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

        categoryService.CreateCategory(category);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await categoryService.GetCategoryById(id);
        return View(category);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Category category)
    {
        await categoryService.UpdateCategory(category);
        return RedirectToAction("Index");
    }
}