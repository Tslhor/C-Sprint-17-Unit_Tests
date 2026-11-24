using Microsoft.AspNetCore.Mvc;
using OrderSystem.Controllers;
using OrderSystem.Data;
using OrderSystem.Models;
using OrderSystem.Tests.Helpers;

namespace OrderSystem.Tests.Controllers;

[TestFixture]
public class CategoryControllerTests
{
    [Test]
    public void Index_WithoutSortOrder_ReturnsCategoriesSortedAscendingByName()
    {
        using var context = TestDbContextFactory.Create();
        SeedCategories(context);
        var controller = new CategoryController(context);

        var result = controller.Index(null);

        var viewResult = result as ViewResult;
        var names = GetCategoryNames(viewResult);

        Assert.That(viewResult, Is.Not.Null);
        Assert.That(names, Is.EqualTo(new[] { "Books", "Clothing", "Electronics" }));
        Assert.That(controller.ViewBag.NameSortParam, Is.EqualTo("name_desc"));
    }

    [Test]
    public void Index_WithNameDescSortOrder_ReturnsCategoriesSortedDescendingByName()
    {
        using var context = TestDbContextFactory.Create();
        SeedCategories(context);
        var controller = new CategoryController(context);

        var result = controller.Index("name_desc");

        var viewResult = result as ViewResult;
        var names = GetCategoryNames(viewResult);

        Assert.That(viewResult, Is.Not.Null);
        Assert.That(names, Is.EqualTo(new[] { "Electronics", "Clothing", "Books" }));
        Assert.That(controller.ViewBag.NameSortParam, Is.EqualTo("name_asc"));
    }

    private static List<string> GetCategoryNames(ViewResult? viewResult)
    {
        var model = viewResult?.Model as IEnumerable<object>;
        return model!
            .Select(item => (string)item.GetType().GetProperty("Name")!.GetValue(item)!)
            .ToList();
    }

    private static void SeedCategories(ProductDbContext context)
    {
        context.Categories.AddRange(
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Books" },
            new Category { Id = 3, Name = "Clothing" });
        context.SaveChanges();
    }
}
