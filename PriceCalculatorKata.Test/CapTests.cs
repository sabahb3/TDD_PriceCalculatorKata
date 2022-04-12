using PriceCalculatorKata.Enumerations;
using Xunit;

namespace PriceCalculatorKata.Test;

public class CapTests
{
    [Theory]
    [InlineData(4,PriceType.Absolute,4)]
    [InlineData(20,PriceType.Percentage,4.05)]
    [InlineData(30,PriceType.Percentage,6.08)]
    public void ShouldGetCapAmountWhenTypeAbsolute(double value, PriceType type, double exactAmount)
    {
        // Arrange
        Cap cap = new Cap(value, type);
        
        // Act
        var amount = cap.GetCapAmount(20.25);
        
        // Assert
        Assert.Equal(exactAmount,amount);
    }
}