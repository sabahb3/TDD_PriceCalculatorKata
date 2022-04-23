using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Structures;

namespace PriceCalculatorKata;

public class Product : IProduct
{
    private List<IExpenses> _expenses;

    public Product(int upc, string name, Currency currency, List<IExpenses> expenses)
    {
        UPC = upc;
        Name = name ?? string.Empty;
        _expenses = expenses;
        ProductCurrency = currency;
    }

    public int UPC { get; private set; }
    public string Name { get; set; } = string.Empty;
    public Currency ProductCurrency { get; private set; }

    public double Price
    {
        get => ProductCurrency.Price;
        set
        {
            SetCurrency(value, ProductCurrency.CurrencyCode);
            Price = ProductCurrency.Price;
        }
    }

    public string CurrencyCode
    {
        get => ProductCurrency.CurrencyCode!;
        set
        {
            SetCurrency(Price, value);
            CurrencyCode = ProductCurrency.CurrencyCode!;
        }
    }

    public void SetCurrency(double price, string code)
    {
        var p = new FormattedDouble(price).FormattedNumber;
        ProductCurrency.SetCurrency(code, p);
    }

    public List<IExpenses> Expenses => _expenses;
}