using PriceCalculatorKata.Enumerations;
using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Structures;

namespace PriceCalculatorKata;

public class Product: IProduct
{

    private Calculations _calculations;
    private double _price;
    private List<IExpenses> _expenses;
    public Product(int upc, string name, double price, Calculations calculations,List<IExpenses> expenses)
    {
        UPC = upc;
        Name = name ?? String.Empty;
        Price = price;
        _calculations = calculations;
        _expenses = expenses;
    }

    public double Price
    {
        get
        {
            return _price;
        }
        private set
        {
            _price = new FormattedDouble(value).FormattedNumber;
        }
    }

    public int UPC { get; private set; }
    public string Name { get; set; }=String.Empty;

    public CombinedDiscount CombinedDiscount { get; set; }
    
    public double Tax
    {
        get
        {
            return _calculations.CalculateTax(Price,UPC,CombinedDiscount);
        }
    }

    public double FinalPrice
    {
        get { return _calculations.CalculateFinalPrice(Price,UPC,_expenses,CombinedDiscount); }
    }

    public double Discount
    {
        get
        {
            return _calculations.CalculateTotalDiscount(Price,UPC,CombinedDiscount);
        }
    }
}