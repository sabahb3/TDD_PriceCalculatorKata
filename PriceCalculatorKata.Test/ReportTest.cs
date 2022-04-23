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
    private Mock<IAccounting> _accounting;
    private Mock<ICalculations> _calculations;




    public ReportTest()
    {
        _accounting = new Mock<IAccounting>();
        _accounting.Setup(c => c.CapAmount(20.25)).Returns(20.25);
        // _accounting.Setup(d => d.UniversalDiscount).Returns(15);
        _product = new Mock<IProduct>();
        _product.Setup(p => p.CurrencyCode).Returns("USD");
        _product.Setup(p => p.Price).Returns(20.25);
        _product.Setup(x => x.UPC).Returns(12345);
        _product.Setup(x => x.Expenses).Returns(new List<IExpenses>());
        _calculations = new Mock<ICalculations>();
        _calculations.Setup(t => t.CalculateTax(_product.Object, _accounting.Object)).Returns(4.2525);
        _accounting.Setup(d => d.UniversalDiscount).Returns(15);
        _accounting.Setup(t => t.Tax).Returns(20);
        _accounting.Setup(c => c.CapAmount(20.25)).Returns(20.25);
        _report = new Report();


    }
    
    [Theory]
    [InlineData(3.04,21.26,"Cost = 20.25 USD\n Tax = 4.05 USD\n Discounts = 3.04 USD\n TOTAL = 21.26 USD\n 3.04 USD discount")]
    [InlineData(0,24.30,"Cost = 20.25 USD\n Tax = 4.05 USD\n TOTAL = 24.30 USD\n no discounts")]
    public void ShouldReportTheProduct(double discountAmount,double finalPrice, string message)
    {
        _calculations.Setup(t => t.CalculateTax(_product.Object,_accounting.Object)).Returns(4.05);
        _calculations.Setup(d => d.CalculateTotalDiscount(_product.Object,_accounting.Object)).Returns(discountAmount);
        if (discountAmount == 0) _accounting.Setup(d => d.UniversalDiscount).Returns(0);
        _calculations.Setup(p => p.CalculateFinalPrice(_product.Object,_accounting.Object)).Returns(finalPrice);

        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object,_accounting.Object);

        // Assert
        Assert.Equal(message,actualMessage);
    }
    
    [Theory]
    [InlineData(20,4.05,4.46,1.42,19.84,"Cost = 20.25 USD\n Tax = 4.05 USD\n Discounts = 4.46 USD\n TOTAL = 19.84 USD\n 4.46 USD total discount",12345,"7")]
    [InlineData(21,4.25,3.04,0,21.46,"Cost = 20.25 USD\n Tax = 4.25 USD\n Discounts = 3.04 USD\n TOTAL = 21.46 USD\n 3.04 USD discount",789,"7")]
    public void ShouldReportTheProductWithUPCDiscount(int tax,double taxAmount,double discountAmount,double upcDiscount, double finalPrice, string message,int upc, string discount )
    {
        // Arrange
        _accounting.Setup(t => t.Tax).Returns(tax);
        _calculations.Setup(c => c.CalculateTax
            (_product.Object,_accounting.Object)).Returns(taxAmount);
        _calculations.Setup(c=>c.CalculateTotalDiscount
            (_product.Object,_accounting.Object)).Returns(discountAmount);
        _calculations.Setup(c => c.CalculateFinalPrice
            (_product.Object,_accounting.Object)).Returns(finalPrice);
        _calculations.Setup(d=>d.UpcDiscount(_product.Object,_accounting.Object)).Returns(upcDiscount);
        var actualUpcDiscount = new Discount();
        actualUpcDiscount.SetDiscount(discount);
        _accounting.Setup(d=>d.UpcDiscount(12345,out actualUpcDiscount)).Returns(upc==_product.Object.UPC);
    
        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object,_accounting.Object);
    
        // Assert
        Assert.Equal(message,actualMessage);
    }
    
    [Fact]
    public void ShouldReportingProductWithExpenses()
    {
        // Arrange
        _accounting.Setup(t => t.Tax).Returns(21);
        _product.Setup(p => p.Expenses).Returns(CalculationsTest.InitializingExpenses());
        _calculations.Setup(c=>c.CalculateTotalDiscount(_product.Object,_accounting.Object)).Returns(4.455);
        _calculations.Setup(c => c.CalculateExpenses(_product.Object))
            .Returns(2.4025);
        _calculations.Setup(c => c.CalculateFinalPrice
            (_product.Object,_accounting.Object)).Returns(22.45);
        _calculations.Setup(c => c.UpcDiscount(_product.Object,_accounting.Object)).Returns(1.4175);
        var upcDiscount = new Discount();
        upcDiscount.SetDiscount("7");
        _accounting.Setup(d => d.UpcDiscount(12345, out upcDiscount)).Returns(true);
    
        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object,_accounting.Object);
        var message =
            "Cost = 20.25 USD\n Tax = 4.25 USD\n Discounts = 4.46 USD\n Packaging = 0.20 USD\n Transport = 2.20 USD\n TOTAL = 22.45 USD\n 4.46 USD total discount";
    
        // Assert
        Assert.Equal(message,actualMessage);
    }
    
    [Fact]
    public void ShouldReportProductWithNoDiscountAndAdditionalCost()
    {
        // Arrange
        _accounting.Setup(t => t.Tax).Returns(21);
        _accounting.Setup(d => d.UniversalDiscount).Returns(0);
        _calculations.Setup(c=>c.CalculateTotalDiscount
            (_product.Object,_accounting.Object)).Returns(0);
        _calculations.Setup(c => c.CalculateFinalPrice
            (_product.Object,_accounting.Object)).Returns(24.50);
        _calculations.Setup(c => c.UpcDiscount(_product.Object,_accounting.Object)).Returns(0);
        var actualUpcDiscount = new Discount();
        _accounting.Setup(d => d.UpcDiscount(12345, out actualUpcDiscount)).Returns(false);
    
        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object,_accounting.Object);
    
        var message =
            "Cost = 20.25 USD\n Tax = 4.25 USD\n TOTAL = 24.50 USD\n no discounts";
    
        // Assert
        Assert.Equal(message,actualMessage);
    }
    [Fact]
    public void ShouldReportProductWithCurrencyCode()
    {
        // Arrange
        _accounting.Setup(t => t.Tax).Returns(21);
        _accounting.Setup(d => d.UniversalDiscount).Returns(0);
        _calculations.Setup(c=>c.CalculateTotalDiscount
            (_product.Object,_accounting.Object)).Returns(0);
        _calculations.Setup(c => c.CalculateFinalPrice
            (_product.Object,_accounting.Object)).Returns(24.50);
        Discount discount= new Discount();
        _accounting.Setup(d => d.UpcDiscount(12345, out discount)).Returns(false);
        _calculations.Setup(c => c.CalculateExpenses(_product.Object)).Returns(0);
        
        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object,_accounting.Object);
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

        _accounting.Setup(t => t.Tax).Returns(21);
        _accounting.Setup(c => c.CombinedDiscount).Returns(CombinedDiscount.Multiplicative);
        _calculations.Setup(c=>c.CalculateTotalDiscount
            (_product.Object,_accounting.Object)).Returns(4.2424);
        _calculations.Setup(c => c.CalculateFinalPrice
            (_product.Object,_accounting.Object)).Returns(20.8676);
        _calculations.Setup(c => c.UpcDiscount(_product.Object,_accounting.Object)).Returns(1.42);
        _calculations.Setup(c => c.CalculateExpenses(_product.Object)).Returns(0.61);
        Discount discount=new Discount
        {
            Precedence = DiscountPrecedence.AfterTax,
        };
        discount.SetDiscount("7");
        _accounting.Setup(d => d.UpcDiscount(12345, out discount)).Returns(true);
    
        // Act
        var actualMessage = _report.DisplayProductReport(_product.Object,_accounting.Object);
        var message =
            "Cost = 20.25 USD\n Tax = 4.25 USD\n Discounts = 4.24 USD\n Transport = 0.61 USD\n TOTAL = 20.87 USD\n 4.24 USD total discount";
    
        // Assert
        Assert.Equal(message,actualMessage);
    }
}

