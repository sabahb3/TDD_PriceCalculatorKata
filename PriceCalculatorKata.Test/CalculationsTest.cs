using System.Collections.Generic;
using Moq;
using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Enumerations;
using PriceCalculatorKata.Structures;

using Xunit;

namespace PriceCalculatorKata.Test;

public class CalculationsTest
{

    private Mock<IAccounting> _accounting;
    private Product _product;


    public CalculationsTest()
    {
        _accounting = new Mock<IAccounting>();
        _accounting.Setup(c => c.CapAmount(20.25)).Returns(20.25);
        _product = new Product(12345,"The Little Prince",new Currency("USD",20.25),new List<IExpenses>());
    }

    [Fact]
    public void ShouldCalculateProductsTax()
    {
        // Arrange
        _accounting.Setup(t => t.Tax).Returns(20);

        // Act
        var tax = _product.CalculateTax(_accounting.Object);
        
        // Assert
        Assert.Equal(4.05,tax);
    }
    
    [Fact]
    public void ShouldCalculateUniversalDiscount()
    {
        // Arrange
        _accounting.Setup(d => d.UniversalDiscount).Returns(15);
        Discount discount=new Discount();
        _accounting.Setup(d => d.UpcDiscount(12345, out discount)).Returns(false);
        
        // Act
        var actualDiscount = _product.CalculateTotalDiscount(_accounting.Object);
        
        // Assert
        Assert.Equal(3.0375,actualDiscount);
    }
    
    [Theory]
    [InlineData(12345,1.4175)]
    [InlineData(789,0)]
    public void ShouldCalculateUPCDiscount(int upc, double discount)
    {
        // Arrange
        var upcD = new Discount();
        upcD.SetDiscount("7");
        _accounting.Setup(d => d.UpcDiscount(upc, out upcD)).Returns(_product.UPC == upc);
        
        // Act
        var actualDiscount =_product.UpcDiscount(_accounting.Object);
        
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
        _accounting.Setup(d => d.UpcDiscount(upc, out upcD)).Returns(_product.UPC == upc);
        _accounting.Setup(d => d.UniversalDiscount).Returns(15);
    
        // Act
        var actualDiscount = _product.CalculateTotalDiscount(_accounting.Object);
        
        // Assert
        Assert.Equal(discount,actualDiscount);
    }
    
    [Fact]
    public void ShouldLookAtPrecedenceWhileCalculatingTax()
    {
        // Arrange
        _accounting.Setup(t => t.Tax).Returns(20);
        _accounting.Setup(d => d.UniversalDiscount).Returns(15);
        _accounting.Setup(d => d.UniversalDiscountPrecedence).Returns(DiscountPrecedence.AfterTax);
        var upcD = new Discount();
        upcD.SetDiscount("7");
        upcD.Precedence = DiscountPrecedence.BeforeTax;
        _accounting.Setup(d => d.UpcDiscount(_product.UPC, out upcD)).Returns(true);
        
        // Act
        var totalDiscount = _product.CalculateTotalDiscount(_accounting.Object);
        var tax = _product.CalculateTax(_accounting.Object);
        var finalPrice =_product.CalculateFinalPrice(_accounting.Object);
        
        // Assert
        Assert.Equal(4.2424,totalDiscount);
        Assert.Equal(3.7665,tax);
        Assert.Equal(19.7741,finalPrice);
    
    
    }
    
    [Fact]
    public void ShouldAddExpenseCostToFinalPrice()
    {
        // Arrange
        var  _product = new Product(12345,"The Little Prince",new Currency("USD",20.25),InitializingExpenses());
        _accounting.Setup(t => t.Tax).Returns(21);
        _accounting.Setup(d => d.UniversalDiscount).Returns(15);
        var upcD = new Discount();
        upcD.SetDiscount("7");
        _accounting.Setup(d => d.UpcDiscount(_product.UPC, out upcD)).Returns(true);
        
        
        // Act
        var expensesCost = _product.CalculateExpenses();
        var tax = _product.CalculateTax(_accounting.Object);
        var discounts = _product.CalculateTotalDiscount(_accounting.Object);
        var finalPrice = _product.CalculateFinalPrice(_accounting.Object);
        
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
        var  _product = new Product(12345,"The Little Prince",new Currency("USD",20.25),InitializingExpenses());
        _accounting.Setup(t => t.Tax).Returns(21);
        _accounting.Setup(d => d.UniversalDiscount).Returns(15);
        var upcD = new Discount();
        upcD.SetDiscount("7");
        _accounting.Setup(d => d.UpcDiscount(_product.UPC, out upcD)).Returns(true);
        _accounting.Setup(c => c.CombinedDiscount).Returns(combining);
    
        // Act
        var tax = _product.CalculateTax(_accounting.Object);
        var actualDiscounts = _product.CalculateTotalDiscount(_accounting.Object);
        var actualFinalPrice = _product.CalculateFinalPrice(_accounting.Object);
    
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
        _accounting.Setup(t => t.Tax).Returns(21);
        _accounting.Setup(d => d.UniversalDiscount).Returns(15);
        var upcD = new Discount();
        upcD.SetDiscount("7");
        _accounting.Setup(d => d.UpcDiscount(12345, out upcD)).Returns(true);
        var expenses = InitializingExpenses();
        _accounting.Setup(c => c.CapType).Returns(PriceType.Absolute);
        _accounting.Setup(c => c.CapAmount(_product.Price)).Returns(capAmount);
        
        // Act
        var actualDiscount = _product.CalculateTotalDiscount(_accounting.Object);
        var actualFinalPrice = _product.CalculateFinalPrice(_accounting.Object);

            // Assert
        Assert.Equal(discount,actualDiscount);
        Assert.Equal(finalPrice,actualFinalPrice);
    }
    
    [Fact]
    public void ShouldUseFourDecimalDigitsPrecisionWhileCalculating()
    {
        // Arrange
        var _product = new Product(12345,"The Little Prince",new Currency("USD",20.25),new List<IExpenses>
        {
            new Expense("Transport", 3, PriceType.Percentage)
    
        });

        _accounting.Setup(t => t.Tax).Returns(21);
        _accounting.Setup(d => d.UniversalDiscount).Returns(15);
        var upcD = new Discount();
        upcD.SetDiscount("7");
        _accounting.Setup(d => d.UpcDiscount(12345, out upcD)).Returns(true);
        _accounting.Setup(c => c.CombinedDiscount).Returns(CombinedDiscount.Multiplicative);
        
        // Act
        var expensesCost = _product.CalculateExpenses();
        var tax = _product.CalculateTax(_accounting.Object);
        var discounts = _product.CalculateTotalDiscount(_accounting.Object);
        var finalPrice = _product.CalculateFinalPrice(_accounting.Object);
        
        // Assert
        Assert.Equal(0.6075,expensesCost);
        Assert.Equal(4.2525,tax);
        Assert.Equal(4.2424,discounts);
        Assert.Equal(20.8676,finalPrice);
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