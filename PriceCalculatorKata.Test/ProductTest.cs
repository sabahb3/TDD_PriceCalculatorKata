using Moq;
using PriceCalculatorKata.Interfaces;
using Xunit;

namespace PriceCalculatorKata.Test;

public class ProductTest
{
    private Mock<ITax> _tax;

    public ProductTest()
    {
        _tax = new Mock<ITax>();
    }

    [Fact]
    public void ShouldCalculateTaxAmountBasedOnProductPrice()
    {
        // Arrange
        _tax.Setup(x => x.TaxValue).Returns(20);
        var product = new Product(12345,"The Little Prince",20.25,_tax.Object);

        // Act
        var taxAmount = product.Tax;
        
        // Assert
        Assert.Equal(20.25,product.Price);
        Assert.Equal(4.05,taxAmount);
    }
}