using PriceCalculatorKata.Interfaces;

namespace PriceCalculatorKata;

public class RelativeDiscount :Discount
{
    private static readonly RelativeDiscount s_universalDiscount = new();

    private RelativeDiscount()
    {
        
    }

    public static RelativeDiscount GetDiscountInstance()
    {
        return s_universalDiscount;
    }

    
}