using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using OrderSystem.Controllers;
using OrderSystem.Data;
using OrderSystem.Models;
using OrderSystem.Services;
using OrderSystem.Tests.Helpers;

namespace OrderSystem.Tests.Controllers;

[TestFixture]
public class ProductControllerTests
{
    [Test]
    public void Index_WithSearchString_ReturnsViewWithFilteredProducts()
    {
        using var context = TestDbContextFactory.Create();
        var serviceMock = new Mock<IProductService>();
        serviceMock.Setup(s => s.SearchProductsByName("lap")).Returns(new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Price = 999.99m, CategoryId = 1 }
        });

        var controller = new ProductController(context, serviceMock.Object);

        var result = controller.Index("lap");

        var viewResult = result as ViewResult;
        var model = viewResult?.Model as IEnumerable<Product>;

        Assert.That(viewResult, Is.Not.Null);
        Assert.That(model, Is.Not.Null);
        Assert.That(model!.Count(), Is.EqualTo(1));
        Assert.That(model.First().Name, Is.EqualTo("Laptop"));
        Assert.That(controller.ViewBag.SearchString, Is.EqualTo("lap"));
    }

    [Test]
    public void Index_WithoutSearchString_ReturnsAllProductsWithCategoriesAndDoesNotCallService()
    {
        using var context = TestDbContextFactory.Create();
        context.Categories.Add(new Category { Id = 1, Name = "Electronics" });
        context.Products.AddRange(
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, CategoryId = 1 },
            new Product { Id = 2, Name = "Smartphone", Price = 599.99m, CategoryId = 1 });
        context.SaveChanges();

        var serviceMock = new Mock<IProductService>();
        var controller = new ProductController(context, serviceMock.Object);

        var result = controller.Index(null!);

        var viewResult = result as ViewResult;
        var model = viewResult?.Model as IEnumerable<Product>;

        Assert.That(viewResult, Is.Not.Null);
        Assert.That(model, Is.Not.Null);
        Assert.That(model!.Count(), Is.EqualTo(2));
        Assert.That(model.All(p => p.Category != null && p.Category.Name == "Electronics"), Is.True);
        serviceMock.Verify(s => s.SearchProductsByName(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void Create_Get_ReturnsViewWithCategoriesInViewBag()
    {
        using var context = TestDbContextFactory.Create();
        context.Categories.Add(new Category { Id = 1, Name = "Electronics" });
        context.SaveChanges();

        var serviceMock = new Mock<IProductService>();
        var controller = new ProductController(context, serviceMock.Object);

        var result = controller.Create();

        var viewResult = result as ViewResult;

        Assert.That(viewResult, Is.Not.Null);
        Assert.That(controller.ViewBag.Categories, Is.InstanceOf<SelectList>());
    }

    [Test]
    public void Create_Post_WhenModelStateIsValid_AddsProductAndRedirectsToIndex()
    {
        using var context = TestDbContextFactory.Create();
        context.Categories.Add(new Category { Id = 1, Name = "Electronics" });
        context.SaveChanges();

        var serviceMock = new Mock<IProductService>();
        var controller = new ProductController(context, serviceMock.Object);
        var product = new Product { Name = "Tablet", Price = 299.99m, CategoryId = 1 };

        var result = controller.Create(product);

        var redirectResult = result as RedirectToActionResult;

        Assert.That(redirectResult, Is.Not.Null);
        Assert.That(redirectResult!.ActionName, Is.EqualTo("Index"));
        Assert.That(context.Products.Any(p => p.Name == "Tablet"), Is.True);
    }

    [Test]
    public void Create_Post_WhenModelStateIsInvalid_ReturnsViewWithSameModel()
    {
        using var context = TestDbContextFactory.Create();
        context.Categories.Add(new Category { Id = 1, Name = "Electronics" });
        context.SaveChanges();

        var serviceMock = new Mock<IProductService>();
        var controller = new ProductController(context, serviceMock.Object);
        controller.ModelState.AddModelError("Name", "Name is required.");

        var product = new Product { Price = 299.99m, CategoryId = 1 };

        var result = controller.Create(product);

        var viewResult = result as ViewResult;

        Assert.That(viewResult, Is.Not.Null);
        Assert.That(viewResult!.Model, Is.SameAs(product));
        Assert.That(controller.ViewBag.Categories, Is.InstanceOf<SelectList>());
        Assert.That(context.Products.Count(), Is.EqualTo(0));
    }

    [Test]
    public void Details_WhenProductExists_ReturnsViewWithProduct()
    {
        using var context = TestDbContextFactory.Create();
        context.Categories.Add(new Category { Id = 1, Name = "Electronics" });
        context.Products.Add(new Product { Id = 1, Name = "Laptop", Price = 999.99m, CategoryId = 1 });
        context.SaveChanges();

        var serviceMock = new Mock<IProductService>();
        var controller = new ProductController(context, serviceMock.Object);

        var result = controller.Details(1);

        var viewResult = result as ViewResult;
        var model = viewResult?.Model as Product;

        Assert.That(viewResult, Is.Not.Null);
        Assert.That(model, Is.Not.Null);
        Assert.That(model!.Name, Is.EqualTo("Laptop"));
        Assert.That(model.Category, Is.Not.Null);
        Assert.That(model.Category!.Name, Is.EqualTo("Electronics"));
    }

    [Test]
    public void Details_WhenProductDoesNotExist_ReturnsNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var serviceMock = new Mock<IProductService>();
        var controller = new ProductController(context, serviceMock.Object);

        var result = controller.Details(99);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
