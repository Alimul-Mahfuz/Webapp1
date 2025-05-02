using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ProductServiceImpl(AppDbContext dbContext) : IProductService
    {
        public Paginator<Product> GetPaginatedProductList(int page = 1)
        {
            const int pageSize = 10;
            var products = dbContext.Products.AsQueryable();

            var numberOfProducts = products.Count();
            var totalPage = (int)Math.Ceiling((double)numberOfProducts / (double)pageSize);
            var paginatedProducts =
                products.Include(p => p.Category).OrderByDescending(p => p.Id).Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            return new Paginator<Product>()
            {
                Data = paginatedProducts,
                TotalPage = totalPage,
                CurrentPage = page
            };
        }

        public Product? GetProductById(int id)
        {
            return dbContext.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
        }

        public async Task CreateProduct(Product product)
        {
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            dbContext.Products.Update(product);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteProduct(Product product)
        {
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
        }
    }
}