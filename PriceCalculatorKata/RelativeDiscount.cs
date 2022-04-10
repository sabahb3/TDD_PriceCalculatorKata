using PriceCalculatorKata.Interfaces;

namespace PriceCalculatorKata;

public class RelativeDiscount :IDiscount
{
    private static readonly RelativeDiscount s_universalDiscount = new();

    private RelativeDiscount()
    {
        
    }

    public static RelativeDiscount GetDiscountInstance()
    {
        return s_universalDiscount;
    }
    public int DiscountValue { get; private set; }
   
    public void SetDiscount(string discount)
    {
        if (int.TryParse(discount, out var newDiscount))
        {
            DiscountValue = newDiscount;
        }
    }
    
}