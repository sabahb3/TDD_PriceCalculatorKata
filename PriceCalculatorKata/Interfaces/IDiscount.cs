namespace PriceCalculatorKata.Interfaces;

public interface IDiscount
{
    int DiscountValue { get; }
    void SetDiscount(string newValue);
}