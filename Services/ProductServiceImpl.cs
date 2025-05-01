using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ProductServiceImpl : IProductService
    {
        private readonly AppDbContext _dbContext;

        public ProductServiceImpl(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public Paginator<Product> GetPaginatedProductList(int page = 1)
        {
            var pageSize = 10;
            var products = this._dbContext.Products.AsQueryable();

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
            return _dbContext.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
        }

        public async Task CreateProduct(Product product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteProduct(Product product)
        {
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
        }
    }
}