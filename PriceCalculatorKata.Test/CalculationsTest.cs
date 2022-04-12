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
    private Mock<ICap> _cap;
    private Calculations _calculations;

    public CalculationsTest()
    {
        _tax = new Mock<ITax>();
        _universalDiscount = new Mock<IDiscount>();
        _upcDiscount = new Mock<ISpecialDiscount>();
        _cap = new Mock<ICap>();
        _cap.Setup(c => c.GetCapAmount(20.25)).Returns(20.25);
        _calculations = new Calculations(_tax.Object,_universalDiscount.Object,_upcDiscount.Object,_cap.Object);
    }

    [Fact]
    public void ShouldCalculateProductsTax()
    {
        // Arrange
        _tax.Setup(t => t.TaxValue).Returns(20);
        
        // Act
        var tax = _calculations.CalculateTax(20.25,12345,_calculations.CombinedDiscount);
        
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
        Assert.Equal(3.0375,discount);
    }

    [Theory]
    [InlineData(12345,1.4175)]
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
    [InlineData(12345,4.455)]
    [InlineData(789,3.0375)]
    public void ShouldCalculateTotalDiscount(int upc, double discount)
    {
        // Arrange
        var upcD = new Discount();
        upcD.SetDiscount("7");
        _upcDiscount.Setup(d=>d.Contains(upc, out upcD)).Returns(true);
        _universalDiscount.Setup(d => d.DiscountValue).Returns(15);

        // Act
        var actualDiscount = _calculations.CalculateTotalDiscount(20.25, 12345,_calculations.CombinedDiscount);
        
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
        var totalDiscount = _calculations.CalculateTotalDiscount(20.25, 12345,_calculations.CombinedDiscount);
        var tax = _calculations.CalculateTax(20.25,12345,_calculations.CombinedDiscount);
        var finalPrice = _calculations.CalculateFinalPrice(20.25, 12345,new List<IExpenses>(),_calculations.CombinedDiscount);
        
        // Assert
        Assert.Equal(4.2424,totalDiscount);
        Assert.Equal(3.7665,tax);
        Assert.Equal(19.7741,finalPrice);


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
        var expensesCost = _calculations.CalculateExpenses(expenses, 20.25);
        var tax = _calculations.CalculateTax(20.25, 12345,_calculations.CombinedDiscount);
        var discounts = _calculations.CalculateTotalDiscount(20.25, 12345,_calculations.CombinedDiscount);
        var finalPrice = _calculations.CalculateFinalPrice(20.25, 12345,expenses,_calculations.CombinedDiscount);
        
        // Assert
        Assert.Equal(2.4025,expensesCost);
        Assert.Equal(4.2525,tax);
        Assert.Equal(4.455,discounts);
        Assert.Equal(22.45,finalPrice);
    }

    [Theory]
    [InlineData(4.455,22.45,CombinedDiscount.Additive)]
    [InlineData(4.2424,22.6626,CombinedDiscount.Multiplicative)]
    public void ShouldTakeIntoAccountDiscountsCombinationWay(double discounts, double finalPrice, CombinedDiscount combining)
    {
        // Arrange
        _tax.Setup(t => t.TaxValue).Returns(21);
        _universalDiscount.Setup(d => d.DiscountValue).Returns(15);
        var upcD = new Discount();
        upcD.SetDiscount("7");
        _upcDiscount.Setup(d=>d.Contains(12345, out upcD)).Returns(true);
        var expenses = InitializingExpenses();
        _calculations.CombinedDiscount = combining;

        // Act
        var tax = _calculations.CalculateTax(20.25, 12345,_calculations.CombinedDiscount);
        var actualDiscounts = _calculations.CalculateTotalDiscount(20.25, 12345,_calculations.CombinedDiscount);
        var actualFinalPrice = _calculations.CalculateFinalPrice(20.25, 12345,expenses,_calculations.CombinedDiscount);

        // Assert
        Assert.Equal(discounts,actualDiscounts);
        Assert.Equal(finalPrice,actualFinalPrice);

    }

    [Theory]
    [InlineData(4.05,20.4525,4.05)]
    [InlineData(4,20.5025,4)]
    [InlineData(4.455,20.0475,6.08)]
    public void ShouldUseCapWhileCalculatingDiscount(double discount,double finalPrice,double capAmount)
    {
        // Arrange
        _tax.Setup(t => t.TaxValue).Returns(21);
        _universalDiscount.Setup(d => d.DiscountValue).Returns(15);
        var upcD = new Discount();
        upcD.SetDiscount("7");
        _upcDiscount.Setup(d=>d.Contains(12345, out upcD)).Returns(true);
        var expenses = InitializingExpenses();
        _cap.Setup(c => c.Type).Returns(PriceType.Absolute);
        _cap.Setup(c => c.GetCapAmount(20.25)).Returns(capAmount);
        
        // Act
        var actualDiscount = _calculations.CalculateTotalDiscount(20.25, 12345, _calculations.CombinedDiscount);
        var actualFinalPrice =
            _calculations.CalculateFinalPrice(20.25, 12345, new List<IExpenses>(), _calculations.CombinedDiscount);
        
        // Assert
        Assert.Equal(discount,actualDiscount);
        Assert.Equal(finalPrice,actualFinalPrice);
    }
    public static List<IExpenses> InitializingExpenses()
    {
        List <IExpenses > expenses= new List<IExpenses>();
        Mock<IExpenses> packagingExpense = new Mock<IExpenses>();
        packagingExpense.Setup(e => e.Description).Returns("Packaging");
        packagingExpense.Setup(e => e.Amount).Returns(0.01);
        packagingExpense.Setup(e => e.Type).Returns(PriceType.Percentage);
        
        Mock<IExpenses> transportExpense = new Mock<IExpenses>();
        transportExpense.Setup(e => e.Description).Returns("Transport");
        transportExpense.Setup(e => e.Amount).Returns(2.2);
        transportExpense.Setup(e => e.Type).Returns(PriceType.Absolute);
        
        expenses.Add(packagingExpense.Object);
        expenses.Add(transportExpense.Object);
        return expenses;
    }
    
}