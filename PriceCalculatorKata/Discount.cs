using PriceCalculatorKata.Interfaces;
using  PriceCalculatorKata.Enumerations;

namespace PriceCalculatorKata;

public class Discount : IDiscount
{
    public int DiscountValue { get; private set; }
    public DiscountPrecedence Precedence { get; set; }

    public void SetDiscount(string discount)
    {
        if (int.TryParse(discount, out var newDiscount))
        {
            DiscountValue = newDiscount;
        }
    }
}