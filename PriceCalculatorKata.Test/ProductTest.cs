using Moq;
using PriceCalculatorKata.Interfaces;
using Xunit;

namespace PriceCalculatorKata.Test;

public class ProductTest
{
    private Mock<ITax> _tax;
    private Product _product;
    private Mock<IDiscount> _UniversalDiscount;
    private Mock<ISpecialDiscount> _UpcDiscounts;

    public ProductTest()
    {
        _tax = new Mock<ITax>();
        _UniversalDiscount = new Mock<IDiscount>();
        _UpcDiscounts = new Mock<ISpecialDiscount>();
        _product = new Product(12345,"The Little Prince",20.25,_tax.Object,_UniversalDiscount.Object,_UpcDiscounts.Object);
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

    [Theory]
    [InlineData(20, 15, 7, 12345,4.46,19.84, "price $19.84 \ntotal discount amount $4.46")]
    [InlineData(21, 15, 7, 789, 3.04,21.46, "price $21.46 \ndiscount amount $3.04")]
    public void ShouldTakeUpcDiscountInAccountWhileCalculatingFinalPrice(int tax, int universalDiscount,
        int upcDiscount, int upcNumber,double totalDiscount,double finalPrice, string message)
    {
        // Arrange
        _tax.Setup(x => x.TaxValue).Returns(tax);
        _UniversalDiscount.Setup(ud => ud.DiscountValue).Returns(universalDiscount);
        var upcD = new Discount();
        upcD.SetDiscount(upcDiscount.ToString());
        _UpcDiscounts.Setup(d=>d.Contains(upcNumber, out upcD)).Returns(true);

        // Act
        var actualFinalPrice = _product.FinalPrice;
        var discountAmount = _product.Discount;
        
        // Assert
        Assert.Equal(totalDiscount,discountAmount);
        Assert.Equal(finalPrice,actualFinalPrice);
    }




}
