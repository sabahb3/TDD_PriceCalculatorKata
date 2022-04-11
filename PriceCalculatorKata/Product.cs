using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Structures;

namespace PriceCalculatorKata;

public class Product: IProduct
{

    private Calculations _calculations;
    private double _price;
    public Product(int upc, string name, double price, Calculations calculations)
    {
        UPC = upc;
        Name = name ?? String.Empty;
        Price = price;
        _calculations = calculations;
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
    
    public double Tax
    {
        get
        {
            return _calculations.CalculateTax(Price);
        }
    }

    public double FinalPrice
    {
        get { return _calculations.CalculateFinalPrice(Price,UPC); }
    }

    public double Discount
    {
        get
        {
            return _calculations.CalculateTotalDiscount(Price,UPC);
        }
    }
}