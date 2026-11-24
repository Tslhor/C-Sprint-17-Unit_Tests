using System.ComponentModel.DataAnnotations;
using OrderSystem.Models;

namespace OrderSystem.Tests.Models;

[TestFixture]
public class ProductValidationTests
{
    [Test]
    public void Product_WhenNameIsMissing_IsInvalid()
    {
        var model = new Product { Price = 100m, CategoryId = 1 };

        var result = ValidateModel(model);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.MemberNames.Contains(nameof(Product.Name))), Is.True);
    }

    [TestCase(0.0)]
    [TestCase(1000.01)]
    public void Product_WhenPriceIsOutOfRange_IsInvalid(decimal price)
    {
        var model = new Product { Name = "Keyboard", Price = price, CategoryId = 1 };

        var result = ValidateModel(model);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.MemberNames.Contains(nameof(Product.Price))), Is.True);
    }

    [TestCase(0.01)]
    [TestCase(50.00)]
    [TestCase(1000.00)]
    public void Product_WhenNameAndPriceAreValid_IsValid(decimal price)
    {
        var model = new Product { Name = "Keyboard", Price = price, CategoryId = 1 };

        var result = ValidateModel(model);

        Assert.That(result.IsValid, Is.True);
    }

    private static (bool IsValid, List<ValidationResult> Errors) ValidateModel(Product model)
    {
        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

        return (isValid, validationResults);
    }
}
