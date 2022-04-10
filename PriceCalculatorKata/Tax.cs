using PriceCalculatorKata.Interfaces;

namespace PriceCalculatorKata;

public class Tax:ITax
{
    private static readonly Tax s_tax = new Tax();
    private Tax() { }
    public static Tax GetTax()
    {
        return s_tax;
    }

    public int TaxValue { get; private set; }

    public void SetTax(string newValue)
    {
        if (int.TryParse(newValue, out var tax))
        {
            TaxValue = tax;
        }
    }
}