using  PriceCalculatorKata.Enumerations;

namespace PriceCalculatorKata.Interfaces;

public interface IDiscount
{
    int DiscountValue { get; }
    DiscountPrecedence Precedence { get; set; }
    void SetDiscount(string newValue);
}