using System.Text.RegularExpressions;
namespace PriceCalculatorKata.Structures;

public struct Currency
{
    public string? CurrencyCode { get; private set; }
    public double Price { get; set; }

    public Currency(string currency,double price) : this()
    {
        SetCurrency(currency,price);
    }

    public void SetCurrency(string currency, double price)
    {
        if (currency.Length == 3 && Regex.IsMatch(currency, @"[A-Za-z]+$"))
            CurrencyCode = currency.ToUpper();
        else
            CurrencyCode = "USD";
        Price = new FormattedDouble(price).FormattedNumber;
    }
}