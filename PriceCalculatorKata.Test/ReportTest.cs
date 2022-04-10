using Moq;
using PriceCalculatorKata.Interfaces;
using Xunit;

namespace PriceCalculatorKata.Test;

public class ReportTest
{
    private Report _report;
    private Mock<IProduct> _product;
    private Mock<ITax> _tax;
    private Mock<IDiscount> _UniversalDiscount;

    public ReportTest()
    {
        _product = new Mock<IProduct>();
        _tax = new Mock<ITax>();
        _UniversalDiscount = new Mock<IDiscount>();
        _report = new Report(_product.Object);
    }
    
    [Theory]
    [InlineData(20,3.04,21.26,"price $21.26 \n$3.04 amount which was deduced")]
    [InlineData(0,0,24.30,"price $24.30 \n")]
    public void ShouldReportTheProduct(int universalDiscount,double discountAmount,double finalPrce, string message )
    {
        // Arrange
        _tax.Setup(t => t.TaxValue).Returns(20);
        _UniversalDiscount.Setup(d => d.DiscountValue).Returns(universalDiscount);
        _product.Setup(p => p.Price).Returns(20.25);
        _product.Setup(p => p.Discount).Returns(discountAmount);
        _product.Setup(p => p.FinalPrice).Returns(finalPrce);

        // Act
        var actualMessage = _report.DisplayProductReport();

        // Assert
        Assert.Equal(message,actualMessage);
    }
    
}

