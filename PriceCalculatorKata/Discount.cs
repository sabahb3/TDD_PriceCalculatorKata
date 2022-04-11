using PriceCalculatorKata.Interfaces;

namespace PriceCalculatorKata;

public class Discount : IDiscount
{
    public int DiscountValue { get; private set; }
    public object Precedence { get; set; }

    public void SetDiscount(string discount)
    {
        if (int.TryParse(discount, out var newDiscount))
        {
            DiscountValue = newDiscount;
        }
    }
}