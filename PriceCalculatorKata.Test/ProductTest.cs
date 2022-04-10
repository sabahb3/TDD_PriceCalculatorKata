using Moq;
using PriceCalculatorKata.Interfaces;
using Xunit;

namespace PriceCalculatorKata.Test;

public class ProductTest
{
    private Mock<ITax> _tax;
    private Product _product;
    private Mock<IDiscount> _UniversalDiscount;

    public ProductTest()
    {
        _tax = new Mock<ITax>();
         _product = new Product(12345,"The Little Prince",20.25,_tax.Object);
         _UniversalDiscount = new Mock<IDiscount>();
    }

    [Theory]
    [InlineData(20,20.25,4.05,24.30)]
    [InlineData(21,20.25,4.25,24.50)]

    public void ShouldCalculateTaxAmountBasedOnProductPrice(int  tax, double price,double taxAmout,double finalPrice)
    {
        // Arrange
        _tax.Setup(x => x.TaxValue).Returns(tax);

        // Act
        var actualTaxAmount = _product.Tax;
        var actualFinalPrice = _product.FinalPrice;
        
        // Assert
        Assert.Equal(price,_product.Price);
        Assert.Equal(taxAmout,actualTaxAmount);
        Assert.Equal(finalPrice,actualFinalPrice);
    }
    

}