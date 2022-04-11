namespace PriceCalculatorKata.Interfaces;

public interface IDiscount
{
    int DiscountValue { get; }
    object Precedence { get; set; }
    void SetDiscount(string newValue);
}