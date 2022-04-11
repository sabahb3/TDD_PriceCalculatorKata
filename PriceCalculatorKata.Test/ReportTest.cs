using Moq;
using PriceCalculatorKata.Interfaces;
using Xunit;

namespace PriceCalculatorKata.Test;

public class ReportTest
{
    private Report _report;
    private Mock<IProduct> _product;
    private Mock<ISpecialDiscount> _upcDiscount;


    public ReportTest()
    {
        _product = new Mock<IProduct>();
        _upcDiscount = new Mock<ISpecialDiscount>();
        _report = new Report(_product.Object,_upcDiscount.Object);
    }
    
    [Theory]
    [InlineData(20,3.04,21.26,"price $21.26 \ndiscount amount $3.04")]
    [InlineData(0,0,24.30,"price $24.30 \n")]
    public void ShouldReportTheProduct(int universalDiscount,double discountAmount,double finalPrice, string message)
    {
        // Arrange
        _product.Setup(p => p.Price).Returns(20.25);
        _product.Setup(p => p.Discount).Returns(discountAmount);
        _product.Setup(p => p.FinalPrice).Returns(finalPrice);

        // Act
        var actualMessage = _report.DisplayProductReport();

        // Assert
        Assert.Equal(message,actualMessage);
    }
    
    [Theory]
    [InlineData(4.05,4.46,19.84,"price $19.84 \ntotal discount amount $4.46",12345,"7")]
    [InlineData(4.25,3.04,21.46,"price $21.46 \ndiscount amount $3.04",789,"7")]
    public void ShouldReportTheProductWithUPCDiscount(double taxAmount,double discountAmount,double finalPrice, string message,int upc, string discount )
    {
        // Arrange
        _product.Setup(p => p.Price).Returns(20.25);
        _product.Setup(p => p.Tax).Returns(taxAmount);
        _product.Setup(p => p.Discount).Returns(discountAmount);
        _product.Setup(p => p.FinalPrice).Returns(finalPrice);
        _product.Setup(x => x.UPC).Returns(12345);
        var upcDiscount = new Discount();
        upcDiscount.SetDiscount(discount);
        _upcDiscount.Setup(d => d.Contains(upc, out upcDiscount)).Returns(upc==_product.Object.UPC);

        // Act
        var actualMessage = _report.DisplayProductReport();

        // Assert
        Assert.Equal(message,actualMessage);
    }
    
    
    
}

