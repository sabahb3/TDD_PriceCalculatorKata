using PriceCalculatorKata.Enumerations;
using Xunit;

namespace PriceCalculatorKata.Test;

public class CapTests
{
    [Fact]
    public void ShouldGetCapAmountWhenTypeAbsolute()
    {
        // Arrange
        Cap cap = new Cap(4, PriceType.Absolute);
        
        // Act
        var amount = cap.GetCapAmount(20.25);
        
        // Assert
        Assert.Equal(4,amount);
    }
}