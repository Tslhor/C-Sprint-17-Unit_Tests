using global::OrderSystem.Models;


namespace OrderSystem.Services
{
    public interface IProductService
    {
        IEnumerable<Product> SearchProductsByName(string substring);
        IEnumerable<Product> GetProductsByCategory(int categoryId);
        IEnumerable<Product> GetProductsSorted(string sortField);
        IEnumerable<Product> SearchProducts(string searchTerm);
    }
}

