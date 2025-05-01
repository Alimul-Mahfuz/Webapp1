using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[Authorize]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService productService, ICategoryService categoryService)
    {
        this._productService = productService;
        this._categoryService = categoryService;
    }

    [HttpGet]
    public IActionResult Index(int page = 1, string search = "")
    {
        var paginator = _productService.GetPaginatedProductList(page);
        return View(paginator);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var category = _categoryService.GetCategories();
        ViewBag.Category = category;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Category = _categoryService.GetCategories();
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

        await _productService.CreateProduct(product);

        return RedirectToAction(nameof(Index));
    }


    public IActionResult Edit(int id)
    {
        var product = _productService.GetProductById(id);
        var category = _categoryService.GetCategories();
        ViewBag.Category = category;
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product product)
    {
        ViewBag.Category = _categoryService.GetCategories();

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        var productToUpdate = _productService.GetProductById(product.Id);
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
                var uploadFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                    productToUpdate.Image.TrimStart('/'));
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
            var filePath = Path.Combine(uploadsFolder, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await product.ImageFile.CopyToAsync(stream);
            }

            productToUpdate.Image = "/uploads/" + fileName;
        }

        await _productService.UpdateProduct(productToUpdate);
        TempData["SuccessMessage"] = "Product updated";

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
        
        await _productService.DeleteProduct(product);
 
        TempData["SuccessMessage"] = "Product deleted";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Back()
    {
        return RedirectToAction("Index");
    }
}