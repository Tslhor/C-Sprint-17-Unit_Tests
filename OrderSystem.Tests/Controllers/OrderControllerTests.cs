using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrderSystem.Controllers;
using OrderSystem.Models;
using OrderSystem.Tests.Helpers;

namespace OrderSystem.Tests.Controllers;

[TestFixture]
public class OrderControllerTests
{
    [Test]
    public void Create_Get_ReturnsViewWithProductsInViewBag()
    {
        using var context = TestDbContextFactory.Create();
        context.Categories.Add(new Category { Id = 1, Name = "Electronics" });
        context.Products.Add(new Product { Id = 1, Name = "Laptop", Price = 999.99m, CategoryId = 1 });
        context.SaveChanges();

        var controller = new OrderController(context);

        var result = controller.Create();

        var viewResult = result as ViewResult;

        Assert.That(viewResult, Is.Not.Null);
        Assert.That(controller.ViewBag.Products, Is.InstanceOf<SelectList>());
    }

    [Test]
    public void Create_Post_WhenModelStateIsValid_AddsOrderAndRedirectsToProductIndex()
    {
        using var context = TestDbContextFactory.Create();
        var controller = new OrderController(context);
        var order = new Order { UserId = "user-1", OrderDate = DateTime.UtcNow };

        var result = controller.Create(order);

        var redirectResult = result as RedirectToActionResult;

        Assert.That(redirectResult, Is.Not.Null);
        Assert.That(redirectResult!.ActionName, Is.EqualTo("Index"));
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Product"));
        Assert.That(context.Orders.Count(), Is.EqualTo(1));
    }

    [Test]
    public void Create_Post_WhenModelStateIsInvalid_ReturnsViewWithSameModel()
    {
        using var context = TestDbContextFactory.Create();
        context.Categories.Add(new Category { Id = 1, Name = "Electronics" });
        context.Products.Add(new Product { Id = 1, Name = "Laptop", Price = 999.99m, CategoryId = 1 });
        context.SaveChanges();

        var controller = new OrderController(context);
        controller.ModelState.AddModelError("OrderDate", "OrderDate is required.");
        var order = new Order { UserId = "user-1" };

        var result = controller.Create(order);

        var viewResult = result as ViewResult;

        Assert.That(viewResult, Is.Not.Null);
        Assert.That(viewResult!.Model, Is.SameAs(order));
        Assert.That(controller.ViewBag.Products, Is.InstanceOf<SelectList>());
        Assert.That(context.Orders.Count(), Is.EqualTo(0));
    }
}
