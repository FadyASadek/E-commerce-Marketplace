using Microsoft.EntityFrameworkCore.Storage;
using OnlineStore.Models;

namespace OnlineStore.Repository
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        Product GetProductById(int id);
        void AddProduct(Product product);
        void UpdateProduct(int id,Product product);
        void DeleteProduct(int id);
        void AddProductImage(ProductImages image);
         IDbContextTransaction BeginTransaction();
        IEnumerable<Product> GetPRoductForPagnation(int page, int pageSize, out int TotalProduct);
        int GetProductCount();
        IEnumerable<Product> GetProductsByCategoryId(int id);
        IEnumerable<Product> GetProductsBysubCategryId(int id);

    }
}
