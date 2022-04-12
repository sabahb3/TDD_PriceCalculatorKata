using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Structures;
using PriceCalculatorKata.Enumerations;

namespace PriceCalculatorKata;

public class Product: IProduct
{

    private Calculations _calculations;
    private double _price;
    private List<IExpenses> _expenses;
    public Product(int upc, string name, Currency currency, Calculations calculations,List<IExpenses> expenses)
    {
        UPC = upc;
        Name = name ?? String.Empty;
        _calculations = calculations;
        _expenses = expenses;
        ProductCurrency = currency;
    }
    public int UPC { get; private set; }
    public string Name { get; set; }=String.Empty;
    public Currency ProductCurrency { get; private set; }

    public double Price
    {
        get
        {
            return ProductCurrency.Price;
        }
        set
        {
            SetCurrency(value,ProductCurrency.CurrencyCode);
            Price = ProductCurrency.Price;
        }
    }

    public string CurrencyCode
    {
        get
        {
            return ProductCurrency.CurrencyCode!;
        }
        set
        {
            SetCurrency(Price,value);
            CurrencyCode = ProductCurrency.CurrencyCode!;
        }
    }
    public void SetCurrency(double price, string code)
    {
        var p = new FormattedDouble(price).FormattedNumber;
        ProductCurrency.SetCurrency(code,p);
    }
    public double Tax
    {
        get
        {
            return _calculations.CalculateTax(Price,UPC,_calculations.CombinedDiscount);
        }
    }

    public double FinalPrice
    {
        get { return _calculations.CalculateFinalPrice(Price,UPC,_expenses,_calculations.CombinedDiscount); }
    }

    public double Discount
    {
        get
        {
            return _calculations.CalculateTotalDiscount(Price,UPC,_calculations.CombinedDiscount);
        }
    }

    public List<IExpenses> Expenses => _expenses;
}