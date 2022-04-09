namespace PriceCalculatorKata.Interfaces;

public interface ITax
{
    int TaxValue { get; }
    void SetTax(string newValue);
}