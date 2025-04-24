using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class ProductController : Controller
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index(int page = 1, string search = "")
    {
        var pageSize = 10;
        var products = _context.Products.AsQueryable();

        var numberOfProducts = products.Count();
        var totalPage = (int)Math.Ceiling((double)numberOfProducts / (double)pageSize);
        var paginatedProducts =
            products.Include(p => p.Category).OrderByDescending(p => p.Id).Skip((page - 1) * pageSize).Take(pageSize)
                .ToList();
        var paginator = new Paginator<Product>()
        {
            Data = paginatedProducts,
            TotalPage = totalPage,
            CurrentPage = page
        };

        return View(paginator);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var category = _context.Categories.ToList();
        ViewBag.Category = category;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Category = _context.Categories.ToList();
            return View(product);
        }

        _context.Products.Add(product);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var product = _context.Products.Find(id);
        var category = _context.Categories.ToList();
        ViewBag.Category = category;
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Product product)
    {
        ViewBag.Category = _context.Categories.ToList();

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        var productToUpdate = _context.Products.Find(product.Id);
        if (productToUpdate == null)
        {
            return NotFound();
        }

        productToUpdate.Name = product.Name;
        productToUpdate.CategoryId = product.CategoryId;
        productToUpdate.Price = product.Price;
        productToUpdate.Description = product.Description;
        productToUpdate.Active = product.Active;
        _context.Products.Update(productToUpdate);
        _context.SaveChanges();
        TempData["SuccessMessage"] = "Product updated";

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
        }
        _context.Products.Remove(product);
        _context.SaveChanges();
        TempData["SuccessMessage"] = "Product deleted";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Back()
    {
        return RedirectToAction("Index");
    }
}