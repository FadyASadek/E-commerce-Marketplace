using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OnlineStore.Models;
using System.Collections.Generic;
using System.Linq;

namespace OnlineStore.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly myContext context;

        public ProductRepository(myContext context)
        {
            this.context = context;
        }

        public void AddProduct(Product product)
        {
            try
            {
                context.products.Add(product);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the product.", ex);
            }
        }

        public void DeleteProduct(int id)
        {
            var product = context.products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                product.IsDeleted = true;
                context.SaveChanges();
            }
        }



        public Product GetProductById(int id)
        {
            return context.products
                .Include(s => s.subCategory).ThenInclude(c => c.category)
                .Include(p => p.Images).Include(u => u.User)
                .FirstOrDefault(p => p.Id == id && !p.IsDeleted); 
        }

        public IEnumerable<Product> GetProducts()
        {
            return context.products
                .Where(p => !p.IsDeleted) 
                .AsNoTracking()
                .Include(s => s.subCategory).ThenInclude(c => c.category)
                .Include(p => p.Images).Include(u => u.User)
                .ToList();
        }

        public IEnumerable<Product> GetPRoductForPagnation(int page, int pageSize, out int TotalProduct)
        {
            var query = context.products.Where(p => !p.IsDeleted); 

            TotalProduct = query.Count();

            return query.Include(s => s.subCategory).ThenInclude(c => c.category)
                         .Include(p => p.Images)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();
        }

        public int GetProductCount()
        {
            return context.products.Count(p => !p.IsDeleted); 
        }

        public IEnumerable<Product> GetProductsByCategoryId(int id)
        {
            return context.products
                .Where(p => p.subCategory.categoryId == id && !p.IsDeleted) 
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Product> GetProductsBysubCategryId(int id)
        {
            return context.products
                .Where(p => p.supCategoryId == id && !p.IsDeleted) 
                .Include(p => p.subCategory).ThenInclude(sc => sc.category)
                .AsNoTracking()
                .ToList();
        }



        public void UpdateProduct(int id, Product product)
        {
            Product oldProduct = GetProductById(id);
            if (oldProduct != null)
            {
                oldProduct.Name = product.Name;
                oldProduct.Description = product.Description;
                oldProduct.Price = product.Price;
                oldProduct.Quantity = product.Quantity;
                oldProduct.MainImage = product.MainImage;
                oldProduct.supCategoryId = product.supCategoryId;
                context.SaveChanges();
            }
        }

        public void AddProductImage(ProductImages image)
        {
            context.productImages.Add(image);
            context.SaveChanges();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return context.Database.BeginTransaction();
        }
    }
}