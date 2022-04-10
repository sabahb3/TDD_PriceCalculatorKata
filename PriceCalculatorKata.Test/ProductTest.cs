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
        _UniversalDiscount = new Mock<IDiscount>();
        _product = new Product(12345,"The Little Prince",20.25,_tax.Object,_UniversalDiscount.Object);
    }

    [Theory]
    [InlineData(20,20.25,4.05,24.30)]
    [InlineData(21,20.25,4.25,24.50)]
    public void ShouldCalculateTaxAmountBasedOnProductPrice(int  tax, double price,double taxAmount,double finalPrice)
    {
        // Arrange
        _tax.Setup(x => x.TaxValue).Returns(tax);

        // Act
        var actualTaxAmount = _product.Tax;
        var actualFinalPrice = _product.FinalPrice;
        
        // Assert
        Assert.Equal(price,_product.Price);
        Assert.Equal(taxAmount,actualTaxAmount);
        Assert.Equal(finalPrice,actualFinalPrice);
    }
    
    [Fact]
    public void ShouldTakeDiscountInAccountWhileCalculatingFinalPrice()
    {
        // Arrange
        var value = Tax.GetTax();
        
        _tax.Setup(x => x.TaxValue).Returns(20);
        _UniversalDiscount.Setup(d => d.DiscountValue).Returns(15);
    
        // Act
        var taxAmount = _product.Tax;
        var discountAmount = _product.Discount;
        var finalPrice = _product.FinalPrice;
    
        // Assert
        Assert.Equal(20.25,_product.Price);
        Assert.Equal(4.05,taxAmount);
        Assert.Equal(3.04,discountAmount);
        Assert.Equal(21.26,finalPrice);
    
    }


    
    
    

}
