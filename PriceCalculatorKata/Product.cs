using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Structures;

namespace PriceCalculatorKata;

public class Product: IProduct
{
    private ITax _tax;
    private IDiscount _universalDiscount;
    private ISpecialDiscount _upcDiscount;
    private double _price;
    public Product(int upc, string name, double price, ITax tax,IDiscount universalDiscount, ISpecialDiscount upcDiscount)
    {
        UPC = upc;
        Name = name ?? String.Empty;
        Price = price;
        _tax = tax;
        _universalDiscount = universalDiscount;
        _upcDiscount = upcDiscount;
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
            var upcDiscount = 0d;
            if (_upcDiscount.Contains(UPC,out var specialDiscount))
            {
                var upcDiscountRatio = new FormattedDouble(specialDiscount!.DiscountValue / 100.0).FormattedNumber;
                upcDiscount = new FormattedDouble(Price * upcDiscountRatio).FormattedNumber;
            }
            var universalDiscountRatio = new FormattedDouble(_universalDiscount.DiscountValue / 100.0).FormattedNumber;
            var universalDiscount= new FormattedDouble(Price * universalDiscountRatio).FormattedNumber;
            return upcDiscount + universalDiscount;
        }
    }
}