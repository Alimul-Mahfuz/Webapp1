using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IProductService
{
    Paginator<Product> GetPaginatedProductList(int page = 1);
    Product? GetProductById(int id);
    Task CreateProduct(Product product);
    Task UpdateProduct(Product product);
    Task DeleteProduct(Product product);
    
}