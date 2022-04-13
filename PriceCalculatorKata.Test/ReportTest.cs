using System.Collections.Generic;
using Moq;
using PriceCalculatorKata.Enumerations;
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
        _product.Setup(p => p.CurrencyCode).Returns("USD");
        _upcDiscount = new Mock<ISpecialDiscount>();
        _report = new Report();
    }
    
    [Theory]
    [InlineData(3.04,21.26,"Cost = 20.25 USD\n Tax = 0.00 USD\n Discounts = 3.04 USD\n TOTAL = 21.26 USD\n 3.04 USD discount")]
    [InlineData(0,24.30,"Cost = 20.25 USD\n Tax = 0.00 USD\n TOTAL = 24.30 USD\n no discounts")]
    public void ShouldReportTheProduct(double discountAmount,double finalPrice, string message)
    {
        // Arrange
        _product.Setup(p => p.Price).Returns(20.25);
        _product.Setup(p => p.Discount).Returns(discountAmount);
        _product.Setup(p => p.FinalPrice).Returns(finalPrice);

        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object);

        // Assert
        Assert.Equal(message,actualMessage);
    }
    
    [Theory]
    [InlineData(4.05,4.46,1.42,19.84,"Cost = 20.25 USD\n Tax = 4.05 USD\n Discounts = 4.46 USD\n TOTAL = 19.84 USD\n 4.46 USD total discount",12345,"7")]
    [InlineData(4.25,3.04,0,21.46,"Cost = 20.25 USD\n Tax = 4.25 USD\n Discounts = 3.04 USD\n TOTAL = 21.46 USD\n 3.04 USD discount",789,"7")]
    public void ShouldReportTheProductWithUPCDiscount(double taxAmount,double discountAmount,double upcDiscount, double finalPrice, string message,int upc, string discount )
    {
        // Arrange
        _product.Setup(p => p.Price).Returns(20.25);
        _product.Setup(p => p.Tax).Returns(taxAmount);
        _product.Setup(p => p.Discount).Returns(discountAmount);
        _product.Setup(p => p.FinalPrice).Returns(finalPrice);
        _product.Setup(x => x.UPC).Returns(12345);
        _product.Setup(p => p.UpcDiscount).Returns(upcDiscount);
        var actualUpcDiscount = new Discount();
        actualUpcDiscount.SetDiscount(discount);
        _upcDiscount.Setup(d => d.Contains(upc, out actualUpcDiscount)).Returns(upc==_product.Object.UPC);

        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object);

        // Assert
        Assert.Equal(message,actualMessage);
    }

    [Fact]
    public void ShouldReportingProductWithExpenses()
    {
        // Arrange
        _product.Setup(p => p.Price).Returns(20.25);
        _product.Setup(p => p.Tax).Returns(4.25);
        _product.Setup(p => p.Discount).Returns(4.46);
        _product.Setup(p => p.FinalPrice).Returns(22.44);
        _product.Setup(x => x.UPC).Returns(12345);
        _product.Setup(p => p.UpcDiscount).Returns(1.42);
        _product.Setup(p => p.Expenses).Returns(CalculationsTest.InitializingExpenses());
        var upcDiscount = new Discount();
        upcDiscount.SetDiscount("7");
        _upcDiscount.Setup(d => d.Contains(12345, out upcDiscount)).Returns(true);

        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object);

        var message =
            "Cost = 20.25 USD\n Tax = 4.25 USD\n Discounts = 4.46 USD\n Packaging = 0.20 USD\n Transport = 2.20 USD\n TOTAL = 22.44 USD\n 4.46 USD total discount";

        // Assert
        Assert.Equal(message,actualMessage);
    }

    [Fact]
    public void ShouldReportProductWithNoDiscountAndAdditionalCost()
    {
        // Arrange
        _product.Setup(p => p.Price).Returns(20.25);
        _product.Setup(p => p.Tax).Returns(4.25);
        _product.Setup(p => p.FinalPrice).Returns(24.50);

        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object);

        var message =
            "Cost = 20.25 USD\n Tax = 4.25 USD\n TOTAL = 24.50 USD\n no discounts";

        // Assert
        Assert.Equal(message,actualMessage);
    }
    [Fact]
    public void ShouldReportProductWithCurrencyCode()
    {
        // Arrange
        _product.Setup(p => p.Price).Returns(20.25);
        _product.Setup(p => p.Tax).Returns(4.25);
        _product.Setup(p => p.FinalPrice).Returns(24.50);
        _product.Setup(p => p.CurrencyCode).Returns("USD");

        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object);

        var message =
            "Cost = 20.25 USD\n Tax = 4.25 USD\n TOTAL = 24.50 USD\n no discounts";

        // Assert
        Assert.Equal(message,actualMessage);
    }
    [Fact]
    public void ShouldReportProductWithTwoDecimalDigitsPrecision ()
    {
        // Arrange
        _product.Setup(p => p.Price).Returns(20.25);
        _product.Setup(p => p.UPC).Returns(12345);
        _product.Setup(p => p.Tax).Returns(4.2525);
        _product.Setup(p => p.Discount).Returns(4.2424);
        _product.Setup(p => p.FinalPrice).Returns(20.8676);
        _product.Setup(p => p.CurrencyCode).Returns("USD");
        _product.Setup(p => p.UpcDiscount).Returns(1.42);
        _product.Setup(p => p.Expenses).Returns(new List<IExpenses>()
        {
            new Expense("Transport", 3, PriceType.Percentage)
        });
        Discount discount;
        _upcDiscount.Setup(d => d.Contains(12345, out discount)).Returns(true);

        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object);

        var message =
            "Cost = 20.25 USD\n Tax = 4.25 USD\n Discounts = 4.24 USD\n Transport = 0.61 USD\n TOTAL = 20.87 USD\n 4.24 USD total discount";

        // Assert
        Assert.Equal(message,actualMessage);
    }
}

