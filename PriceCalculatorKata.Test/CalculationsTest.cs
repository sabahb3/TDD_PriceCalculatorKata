using System.Collections.Generic;
using Moq;
using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Enumerations;

using Xunit;

namespace PriceCalculatorKata.Test;

public class CalculationsTest
{
    private Mock<ITax> _tax;
    private Mock<IDiscount> _universalDiscount;
    private Mock<ISpecialDiscount> _upcDiscount;
    private Calculations _calculations;

    public CalculationsTest()
    {
        _tax = new Mock<ITax>();
        _universalDiscount = new Mock<IDiscount>();
        _upcDiscount = new Mock<ISpecialDiscount>();
        _calculations = new Calculations(_tax.Object,_universalDiscount.Object,_upcDiscount.Object);
    }

    [Fact]
    public void ShouldCalculateProductsTax()
    {
        // Arrange
        _tax.Setup(t => t.TaxValue).Returns(20);
        
        // Act
        var tax = _calculations.CalculateTax(20.25,12345);
        
        // Assert
        Assert.Equal(4.05,tax);
    }

    [Fact]
    public void ShouldCalculateUniversalDiscount()
    {
        // Arrange
        _universalDiscount.Setup(d => d.DiscountValue).Returns(15);
        
        // Act
        var discount = _calculations.CalculateUniversalDiscount(20.25);
        
        // Assert
        Assert.Equal(3.04,discount);
    }

    [Theory]
    [InlineData(12345,1.42)]
    [InlineData(789,0)]
    public void ShouldCalculateUPCDiscount(int upc, double discount)
    {
        // Arrange
        var upcD = new Discount();
        upcD.SetDiscount("7");
        _upcDiscount.Setup(d=>d.Contains(upc, out upcD)).Returns(true);
        
        // Act
        var actualDiscount = _calculations.CalculateUPCDiscount(20.25, 12345);
        
        // Assert
        Assert.Equal(discount,actualDiscount);
    }
    
    [Theory]
    [InlineData(12345,4.46)]
    [InlineData(789,3.04)]
    public void ShouldCalculateTotalDiscount(int upc, double discount)
    {
        // Arrange
        var upcD = new Discount();
        upcD.SetDiscount("7");
        _upcDiscount.Setup(d=>d.Contains(upc, out upcD)).Returns(true);
        _universalDiscount.Setup(d => d.DiscountValue).Returns(15);

        // Act
        var actualDiscount = _calculations.CalculateTotalDiscount(20.25, 12345);
        
        // Assert
        Assert.Equal(discount,actualDiscount);
    }

    [Fact]
    public void ShouldLookAtPrecedenceWhileCalculatingTax()
    {
        // Arrange
        _tax.Setup(t => t.TaxValue).Returns(20);
        _universalDiscount.Setup(d => d.DiscountValue).Returns(15);
        _universalDiscount.Setup(d => d.Precedence).Returns(DiscountPrecedence.AfterTax);
        var upcD = new Discount();
        upcD.SetDiscount("7");
        upcD.Precedence = DiscountPrecedence.BeforeTax;
        _upcDiscount.Setup(d=>d.Contains(12345, out upcD)).Returns(true);
        
        // Act
        var totalDiscount = _calculations.CalculateTotalDiscount(20.25, 12345);
        var tax = _calculations.CalculateTax(20.25,12345);
        var finalPrice = _calculations.CalculateFinalPrice(20.25, 12345);
        
        // Assert
        Assert.Equal(4.24,totalDiscount);
        Assert.Equal(3.77,tax);
        Assert.Equal(19.78,finalPrice);


    }

    [Fact]
    public void ShouldAddExpenseCostToFinalPrice()
    {
        // Arrange
        var expenses = InitializingExpenses();
        _tax.Setup(t => t.TaxValue).Returns(21);
        _universalDiscount.Setup(d => d.DiscountValue).Returns(15);
        var upcD = new Discount();
        upcD.SetDiscount("7");
        _upcDiscount.Setup(d=>d.Contains(12345, out upcD)).Returns(true);
        
        // Act
        var expensesCost = _calculations.CaluculateExpensis(expenses, 20.25);
        var tax = _calculations.CalculateTax(20.25, 12345);
        var finalPrice = _calculations.CalculateFinalPrice(20.25, 12345);
        
        // Assert
        Assert.Equal(2.22,expensesCost);
        Assert.Equal(4.25,tax);
        Assert.Equal(22.44,finalPrice);
    }

    private List<IExpenses> InitializingExpenses()
    {
        List <IExpenses > expenses= new List<IExpenses>();
        Mock<IExpenses> packagingExpense = new Mock<IExpenses>();
        packagingExpense.Setup(e => e.Description).Returns("Packaging");
        packagingExpense.Setup(e => e.Amount).Returns(1);
        packagingExpense.Setup(e => e.Type).Returns(PriceType.Percentage);
        
        Mock<IExpenses> transportExpense = new Mock<IExpenses>();
        packagingExpense.Setup(e => e.Description).Returns("Transport");
        packagingExpense.Setup(e => e.Amount).Returns(2.2);
        packagingExpense.Setup(e => e.Type).Returns(PriceType.Absolute);
        
        expenses.Add(packagingExpense.Object);
        expenses.Add(transportExpense.Object);
        return expenses;
    }
}