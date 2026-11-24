using OrderSystem.Models;
using OrderSystem.Services;
using OrderSystem.Data;
using OrderSystem.Tests.Helpers;

namespace OrderSystem.Tests.Services;

[TestFixture]
public class ProductServiceTests
{
    [Test]
    public void SearchProductsByName_WhenSubstringIsNullOrWhiteSpace_ReturnsAllProducts()
    {
        using var context = TestDbContextFactory.Create();
        SeedProducts(context);
        var service = new ProductService(context);

        var result = service.SearchProductsByName(" ").ToList();

        Assert.That(result, Has.Count.EqualTo(3));
    }

    [Test]
    public void SearchProductsByName_WhenSubstringMatches_ReturnsCaseInsensitiveMatches()
    {
        using var context = TestDbContextFactory.Create();
        SeedProducts(context);
        var service = new ProductService(context);

        var result = service.SearchProductsByName("lap").ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Laptop"));
    }

    [Test]
    public void GetProductsByCategory_ReturnsOnlyProductsInCategory()
    {
        using var context = TestDbContextFactory.Create();
        SeedProducts(context);
        var service = new ProductService(context);

        var result = service.GetProductsByCategory(1).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(p => p.CategoryId == 1), Is.True);
    }

    [Test]
    public void GetProductsByCategory_WhenCategoryDoesNotExist_ReturnsEmpty()
    {
        using var context = TestDbContextFactory.Create();
        SeedProducts(context);
        var service = new ProductService(context);

        var result = service.GetProductsByCategory(999).ToList();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetProductsSorted_WhenSortFieldIsName_ReturnsAlphabeticallySortedProducts()
    {
        using var context = TestDbContextFactory.Create();
        SeedProducts(context);
        var service = new ProductService(context);

        var result = service.GetProductsSorted("Name").Select(p => p.Name).ToList();

        Assert.That(result, Is.EqualTo(new[] { "Laptop", "Novel", "Smartphone" }));
    }

    [Test]
    public void GetProductsSorted_WhenSortFieldIsPrice_ReturnsProductsOrderedByAscendingPrice()
    {
        using var context = TestDbContextFactory.Create();
        SeedProducts(context);
        var service = new ProductService(context);

        var result = service.GetProductsSorted(nameof(Product.Price)).Select(p => p.Name).ToList();

        Assert.That(result, Is.EqualTo(new[] { "Novel", "Smartphone", "Laptop" }));
    }

    [Test]
    public void GetProductsSorted_WhenSortFieldIsInvalid_ThrowsArgumentException()
    {
        using var context = TestDbContextFactory.Create();
        SeedProducts(context);
        var service = new ProductService(context);

        Assert.Throws<ArgumentException>(() => service.GetProductsSorted("InvalidField"));
    }

    [Test]
    public void GetProductsSorted_WhenSortFieldIsNullOrWhiteSpace_DefaultsToIdOrder()
    {
        using var context = TestDbContextFactory.Create();
        SeedProducts(context);
        var service = new ProductService(context);

        var result = service.GetProductsSorted(" ").Select(p => p.Id).ToList();

        Assert.That(result, Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void SearchProducts_WhenSearchTermMatches_ReturnsCaseInsensitiveMatches()
    {
        using var context = TestDbContextFactory.Create();
        SeedProducts(context);
        var service = new ProductService(context);

        var result = service.SearchProducts("pHoNe").ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Smartphone"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void SearchProducts_WhenSearchTermIsNullOrWhiteSpace_ReturnsAllProducts(string? searchTerm)
    {
        using var context = TestDbContextFactory.Create();
        SeedProducts(context);
        var service = new ProductService(context);

        var result = service.SearchProducts(searchTerm!).ToList();

        Assert.That(result, Has.Count.EqualTo(3));
    }

    private static void SeedProducts(ProductDbContext context)
    {
        var electronics = new Category { Id = 1, Name = "Electronics" };
        var books = new Category { Id = 2, Name = "Books" };

        context.Categories.AddRange(electronics, books);
        context.Products.AddRange(
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, CategoryId = 1 },
            new Product { Id = 2, Name = "Smartphone", Price = 599.99m, CategoryId = 1 },
            new Product { Id = 3, Name = "Novel", Price = 14.99m, CategoryId = 2 });

        context.SaveChanges();
    }
}
