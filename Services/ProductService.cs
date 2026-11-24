using Microsoft.EntityFrameworkCore;
using OrderSystem.Data;
using OrderSystem.Models;

namespace OrderSystem.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _dbContext;
        public ProductService(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Product> SearchProductsByName(string substring)
        {
            if (string.IsNullOrWhiteSpace(substring))
            {
                return _dbContext.Products.Include(p => p.Category).ToList(); 
            }

            return _dbContext.Products
                    .Include(p => p.Category)
                    .AsEnumerable() 
                    .Where(p => p.Name.Contains(substring, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            return _dbContext.Products.Where(p => p.CategoryId == categoryId);
        }

        public IEnumerable<Product> GetProductsSorted(string sortField)
        {
            var normalizedField = string.IsNullOrWhiteSpace(sortField) ? nameof(Product.Id) : sortField;

            var validField = normalizedField switch
            {
                nameof(Product.Id) => nameof(Product.Id),
                nameof(Product.Name) => nameof(Product.Name),
                nameof(Product.Price) => nameof(Product.Price),
                nameof(Product.CategoryId) => nameof(Product.CategoryId),
                _ => throw new ArgumentException($"Invalid sort field: '{sortField}'.", nameof(sortField))
            };

            return _dbContext.Products.OrderBy(p => EF.Property<object>(p, validField)).ToList();
        }

        public IEnumerable<Product> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return _dbContext.Products.Include(p => p.Category).ToList();
            }

            return _dbContext.Products
                .Include(p => p.Category)
                .AsEnumerable()
                .Where(p => p.Name != null && p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
