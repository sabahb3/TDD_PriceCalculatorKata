using Xunit;

namespace PriceCalculatorKata.Test;

public class ProductTest
{
    [Fact]
    public void ShouldCalculateTaxAmountBasedOnProductPrice()
    {
        // Arrange
        var product = new Product(12345,"The Little Prince",20.25);

        // Act
        var taxAmount = product.GetTax();
        
        // Assert
        Assert.Equal(20.25,product.Price);
        Assert.Equal(4.05,taxAmount);
    }
}