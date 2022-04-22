using System.Collections.Generic;
using System.Linq;
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
    private Mock<Calculations> _calculations;
    private Mock<ITax> _tax;
    private Mock<IDiscount> _universalDiscount;
    private Mock<ICap> _cap;


    public ReportTest()
    {
        _product = new Mock<IProduct>();
        _product.Setup(p => p.CurrencyCode).Returns("USD");
        _upcDiscount = new Mock<ISpecialDiscount>();
        _tax = new Mock<ITax>();
        _universalDiscount = new Mock<IDiscount>();
        _upcDiscount = new Mock<ISpecialDiscount>();
        _cap = new Mock<ICap>();
        _cap.Setup(c => c.GetCapAmount(20.25)).Returns(20.25);
        _calculations = new Mock<Calculations>(_tax.Object,_universalDiscount.Object,_upcDiscount.Object,_cap.Object);
        _calculations.Setup(c => c.CombinedDiscount).Returns(CombinedDiscount.Additive);
        _report = new Report();
        _universalDiscount.Setup(d => d.Precedence).Returns(DiscountPrecedence.AfterTax);
        _universalDiscount.Setup(d => d.DiscountValue).Returns(15);
        
        _product.Setup(p => p.Price).Returns(20.25);
        _product.Setup(x => x.UPC).Returns(12345);
        _calculations.Setup(c => c.CalculateTax
            (20.25,12345,CombinedDiscount.Additive)).Returns(4.2525);

    }
    
    [Theory]
    [InlineData(3.04,21.26,"Cost = 20.25 USD\n Tax = 0.00 USD\n Discounts = 3.04 USD\n TOTAL = 21.26 USD\n 3.04 USD discount")]
    [InlineData(0,24.30,"Cost = 20.25 USD\n Tax = 0.00 USD\n TOTAL = 24.30 USD\n no discounts")]
    public void ShouldReportTheProduct(double discountAmount,double finalPrice, string message)
    {
        // Arrange
        _calculations.Setup(c => c.CalculateTax(20.25, 12345, CombinedDiscount.Additive)).Returns(0);
        _calculations.Setup(c=>c.CalculateTotalDiscount(20.25,12345,_calculations.Object.CombinedDiscount))
            .Returns(discountAmount);
        _calculations.Setup(c => c.CalculateFinalPrice(20.25,12345,_product.Object.Expenses,_calculations.Object.CombinedDiscount))
            .Returns(finalPrice);
        _calculations.Setup(c => c.CalculateUPCDiscount(20.25, 12345)).Returns(0);
        var actualUpcDiscount = new Discount();
        actualUpcDiscount.SetDiscount("0");
        _upcDiscount.Setup(d => d.Contains(12345, out actualUpcDiscount)).
            Returns(false);

        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object,_calculations.Object);

        // Assert
        Assert.Equal(message,actualMessage);
    }
    
    [Theory]
    [InlineData(4.05,4.46,1.42,19.84,"Cost = 20.25 USD\n Tax = 4.05 USD\n Discounts = 4.46 USD\n TOTAL = 19.84 USD\n 4.46 USD total discount",12345,"7")]
    [InlineData(4.25,3.04,0,21.46,"Cost = 20.25 USD\n Tax = 4.25 USD\n Discounts = 3.04 USD\n TOTAL = 21.46 USD\n 3.04 USD discount",789,"7")]
    public void ShouldReportTheProductWithUPCDiscount(double taxAmount,double discountAmount,double upcDiscount, double finalPrice, string message,int upc, string discount )
    {
        // Arrange
        _calculations.Setup(c => c.CalculateTax
            (20.25,12345,_calculations.Object.CombinedDiscount)).Returns(taxAmount);
        _calculations.Setup(c=>c.CalculateTotalDiscount
            (20.25,12345,_calculations.Object.CombinedDiscount)).Returns(discountAmount);
        _calculations.Setup(c => c.CalculateFinalPrice
            (20.25,12345,_product.Object.Expenses,_calculations.Object.CombinedDiscount)).Returns(finalPrice);
        _calculations.Setup(c => c.CalculateUPCDiscount(20.25, 12345)).Returns(upcDiscount);
        var actualUpcDiscount = new Discount();
        actualUpcDiscount.SetDiscount(discount);
        _upcDiscount.Setup(d => d.Contains(upc, out actualUpcDiscount)).Returns(upc==_product.Object.UPC);

        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object,_calculations.Object);

        // Assert
        Assert.Equal(message,actualMessage);
    }

    [Fact]
    public void ShouldReportingProductWithExpenses()
    {
        // Arrange
        _product.Setup(p => p.Expenses).Returns(CalculationsTest.InitializingExpenses());
        _calculations.Setup(c=>c.CalculateTotalDiscount
            (20.25,12345,CombinedDiscount.Additive)).Returns(4.46);
        _calculations.Setup(c => c.CalculateExpenses(CalculationsTest.InitializingExpenses(), 20.25))
            .Returns(2.4);
        _calculations.Setup(c => c.CalculateFinalPrice
            (20.25,12345,_product.Object.Expenses,CombinedDiscount.Additive)).Returns(22.44);
        _calculations.Setup(c => c.CalculateUPCDiscount(20.25, 12345)).Returns(1.42);
        var upcDiscount = new Discount();
        upcDiscount.SetDiscount("7");
        _upcDiscount.Setup(d => d.Contains(12345, out upcDiscount)).Returns(true);

        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object,_calculations.Object);
        var message =
            "Cost = 20.25 USD\n Tax = 4.25 USD\n Discounts = 4.46 USD\n Packaging = 0.20 USD\n Transport = 2.20 USD\n TOTAL = 22.44 USD\n 4.46 USD total discount";

        // Assert
        Assert.Equal(message,actualMessage);
    }

    [Fact]
    public void ShouldReportProductWithNoDiscountAndAdditionalCost()
    {
        // Arrange
        _calculations.Setup(c=>c.CalculateTotalDiscount
            (20.25,12345,_calculations.Object.CombinedDiscount)).Returns(0);
        _calculations.Setup(c => c.CalculateFinalPrice
            (20.25,12345,_product.Object.Expenses,_calculations.Object.CombinedDiscount)).Returns(24.50);
        _calculations.Setup(c => c.CalculateUPCDiscount(20.25, 12345)).Returns(0);
        var actualUpcDiscount = new Discount();
        _upcDiscount.Setup(d => d.Contains(12345, out actualUpcDiscount)).Returns(false);

        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object,_calculations.Object);

        var message =
            "Cost = 20.25 USD\n Tax = 4.25 USD\n TOTAL = 24.50 USD\n no discounts";

        // Assert
        Assert.Equal(message,actualMessage);
    }
    [Fact]
    public void ShouldReportProductWithCurrencyCode()
    {
        // Arrange
        _calculations.Setup(c=>c.CalculateTotalDiscount
            (20.25,12345,_calculations.Object.CombinedDiscount)).Returns(0);
        _calculations.Setup(c => c.CalculateFinalPrice
            (20.25,12345,_product.Object.Expenses,_calculations.Object.CombinedDiscount)).Returns(24.50);
        Discount discount= new Discount();
        _upcDiscount.Setup(d => d.Contains(12345, out discount)).Returns(false);
        _calculations.Setup(c => c.CalculateExpenses(_product.Object.Expenses, 20.25)).Returns(0);
        
        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object,_calculations.Object);
        var message =
            "Cost = 20.25 USD\n Tax = 4.25 USD\n TOTAL = 24.50 USD\n no discounts";

        // Assert
        Assert.Equal(message,actualMessage);
    }
    [Fact]
    public void ShouldReportProductWithTwoDecimalDigitsPrecision ()
    {
        // Arrange
        _product.Setup(p => p.Expenses).Returns(new List<IExpenses>()
        {
            new Expense("Transport", 3, PriceType.Percentage)
        });
        
        _calculations.Setup(c=>c.CalculateTotalDiscount
            (20.25,12345,_calculations.Object.CombinedDiscount)).Returns(4.2424);
        _calculations.Setup(c => c.CalculateFinalPrice
            (20.25,12345,_product.Object.Expenses,_calculations.Object.CombinedDiscount)).Returns(20.8676);
        _calculations.Setup(c => c.CalculateUPCDiscount(20.25, 12345)).Returns(1.42);
        _calculations.Setup(c => c.CalculateExpenses(_product.Object.Expenses, 20.25)).Returns(0.61);
        Discount discount;
        _upcDiscount.Setup(d => d.Contains(12345, out discount)).Returns(true);

        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object,_calculations.Object);
        var message =
            "Cost = 20.25 USD\n Tax = 4.25 USD\n Discounts = 4.24 USD\n Transport = 0.61 USD\n TOTAL = 20.87 USD\n 4.24 USD total discount";

        // Assert
        Assert.Equal(message,actualMessage);
    }
}

