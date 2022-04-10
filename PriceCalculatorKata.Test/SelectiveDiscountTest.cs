using Xunit;

namespace PriceCalculatorKata.Test;

public class SelectiveDiscountTest
{
    private SpecialDiscounts _specialDiscount;
    public SelectiveDiscountTest()
    {
        _specialDiscount = new SpecialDiscounts();
        
    }

    [Fact]
    public void ShouldAddASpecialDiscount()
    {
        // Arrange
        var upc = 12345;
        var discount = new Discount();
        discount.SetDiscount("7");

        // Act
        _specialDiscount.Add(upc, discount);
        var specialDiscountsCount = _specialDiscount.Count();
        var containsUpc = _specialDiscount.Contains(upc);
        
        // Assert
        Assert.Equal(1,specialDiscountsCount);
        Assert.True(containsUpc);
    }
    
}