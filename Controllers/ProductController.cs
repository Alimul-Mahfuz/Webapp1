using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[Authorize]
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
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Category = _context.Categories.ToList();
            return View(product);
        }

        // Save uploaded file
        if (product.ImageFile != null && product.ImageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate unique filename
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save file to wwwroot/uploads
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await product.ImageFile.CopyToAsync(stream);
            }

            product.Image = "/uploads/" + fileName;
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
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

       

        if (product.ImageFile != null && product.ImageFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(productToUpdate.Image))
            {
                var uploadFile=Path.Combine(Directory.GetCurrentDirectory(),"wwwroot",productToUpdate.Image.TrimStart('/'));
                if (System.IO.File.Exists(uploadFile))
                {
                    System.IO.File.Delete(uploadFile);
                }
            }
        
            //Uploading new file
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
            var filePath=Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            { 
                product.ImageFile.CopyTo(stream);
            }
            productToUpdate.Image="/uploads/" + fileName;
        }
        
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