using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Structures;

namespace PriceCalculatorKata;

public class Product: IProduct
{
    private ITax _tax;
    private IDiscount _universalDiscount;
    private double _price;
    public Product(int upc, string name, double price, ITax tax,IDiscount universalDiscount)
    {
        UPC = upc;
        Name = name ?? String.Empty;
        Price = price;
        _tax = tax;
        _universalDiscount = universalDiscount;
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
            var tax = new FormattedDouble(_tax.TaxValue / 100.0).FormattedNumber;
            return new FormattedDouble(Price * tax).FormattedNumber;
        }
    }

    public double FinalPrice
    {
        get { return new FormattedDouble(Price + Tax - Discount).FormattedNumber; }
    }

    public double Discount
    {
        get
        {
            var discount = new FormattedDouble(_universalDiscount.DiscountValue / 100.0).FormattedNumber;
            return new FormattedDouble(Price * discount).FormattedNumber;
        }
    }
}