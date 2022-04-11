using Moq;
using PriceCalculatorKata.Interfaces;
using Xunit;

namespace PriceCalculatorKata.Test;

public class ProductTest
{
    private Product _product;
    private Mock<Calculations> _calculations;
    private Mock<ITax> _tax;
    private Mock<IDiscount> _universalDiscount;
    private Mock<ISpecialDiscount> _upcDiscount;


    public ProductTest()
    {
        _tax = new Mock<ITax>();
        _universalDiscount = new Mock<IDiscount>();
        _upcDiscount = new Mock<ISpecialDiscount>();
        _calculations = new Mock<Calculations>(_tax.Object,_universalDiscount.Object,_upcDiscount.Object);
        _product = new Product(12345,"The Little Prince",20.25,_calculations.Object);
    }

    [Theory]
    [InlineData(20,20.25,4.05,24.30)]
    [InlineData(21,20.25,4.25,24.50)]
    public void ShouldCalculateTaxAmountBasedOnProductPrice(int  tax, double price,double taxAmount,double finalPrice)
    {
        // Arrange
        _calculations.Setup(x => x.CalculateTax(price,_product.UPC)).Returns(taxAmount);
        _calculations.Setup(x => x.CalculateFinalPrice(price, _product.UPC)).Returns(finalPrice);

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
        _calculations.Setup(c => c.CalculateFinalPrice(20.25, 12345)).Returns(21.26);
        _calculations.Setup(t => t.CalculateTax(_product.Price,_product.UPC)).Returns(4.05);
        _calculations.Setup(d => d.CalculateTotalDiscount(_product.Price, _product.UPC)).Returns(3.04);
        
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

    [Theory]
    [InlineData(20, 15, 7, 12345,4.46,19.84, "price $19.84 \ntotal discount amount $4.46")]
    [InlineData(21, 15, 7, 789, 3.04,21.46, "price $21.46 \ndiscount amount $3.04")]
    public void ShouldTakeUpcDiscountInAccountWhileCalculatingFinalPrice(int tax, int universalDiscount,
        int upcDiscount, int upcNumber,double totalDiscount,double finalPrice, string message)
    {
        // Arrange
        _calculations.Setup(c => c.CalculateFinalPrice(_product.Price,_product.UPC)).Returns(finalPrice);
        _calculations.Setup(d => d.CalculateTotalDiscount(_product.Price, _product.UPC)).Returns(totalDiscount);

        // Act
        var actualFinalPrice = _product.FinalPrice;
        var discountAmount = _product.Discount;
        
        // Assert
        Assert.Equal(totalDiscount,discountAmount);
        Assert.Equal(finalPrice,actualFinalPrice);
    }




}
