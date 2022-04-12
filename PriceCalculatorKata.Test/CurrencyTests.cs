using Xunit;
using PriceCalculatorKata.Structures;
namespace PriceCalculatorKata.Test;

public class CurrencyTests
{
    [Theory]
    [InlineData("USD","USD")]
    [InlineData("GBp","GBP")]
    [InlineData("US","USD")]
    [InlineData("US22","USD")]
    public void ShouldReturnCurrencyCode(string input, string output)
    {
        // Arrange
        Currency currency = new Currency(input,20.25);
        
        // Act
        var currencyCode = currency.CurrencyCode;
        
        // Assert
        Assert.Equal(output,currencyCode);
    }
}