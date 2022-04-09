using Moq;
using PriceCalculatorKata.Interfaces;
using Xunit;

namespace PriceCalculatorKata.Test;

public class ProductTest
{
    private Mock<ITax> _tax;
    private Product _product;

    public ProductTest()
    {
        _tax = new Mock<ITax>();
         _product = new Product(12345,"The Little Prince",20.25,_tax.Object);

    }

    [Fact]
    public void ShouldCalculateTaxAmountBasedOnProductPrice()
    {
        // Arrange
        _tax.Setup(x => x.TaxValue).Returns(20);

        // Act
        var taxAmount = _product.Tax;
        
        // Assert
        Assert.Equal(20.25,_product.Price);
        Assert.Equal(4.05,taxAmount);
    }
}