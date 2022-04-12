using System.Text.RegularExpressions;
namespace PriceCalculatorKata.Structures;

public struct Currency
{
    public string? CurrencyCode { get; private set; }

    public Currency(string currency) : this()
    {
        SetCurrency(currency);
    }

    public void SetCurrency(string currency)
    {
        if (currency.Length == 3 && Regex.IsMatch(currency, @"[A-Za-z]+$"))
            CurrencyCode = currency.ToUpper();
        else
            CurrencyCode = "USD";
    }
}