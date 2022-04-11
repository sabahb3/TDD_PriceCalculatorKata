using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Structures;

namespace PriceCalculatorKata;

public class Calculations
{
    private ITax _tax;
    private IDiscount _universalDiscount;
    private ISpecialDiscount _upcDiscount;

    public Calculations(ITax tax, IDiscount universalDiscount, ISpecialDiscount upcDiscount)
    {
        _tax = tax;
        _universalDiscount = universalDiscount;
        _upcDiscount = upcDiscount;
    }

    public virtual double CalculateFinalPrice(double price, int upc)
    {
        return price + CalculateTax(price) - CalculateTotalDiscount(price, upc);
    }
    public virtual double CalculateTax(double price)
    {
        var taxRatio = new FormattedDouble(_tax.TaxValue / 100.0).FormattedNumber;
        return new FormattedDouble(price * taxRatio).FormattedNumber;
    }

    public virtual double CalculateTotalDiscount(double price, int upc)
    {
        return CalculateUPCDiscount(price,upc) + CalculateUniversalDiscount(price);
    }
    
    public virtual double CalculateUPCDiscount(double price, int upc)
    {
        double upcDiscount=0d;
        if (_upcDiscount.Contains(upc, out var discount))
        {
            var upcDiscountRatio = new FormattedDouble(discount!.DiscountValue / 100.0).FormattedNumber;
            upcDiscount = new FormattedDouble(price * upcDiscountRatio).FormattedNumber;
        }

        return upcDiscount;
    }
    
    public virtual double CalculateUniversalDiscount(double price)
    {
        var universalDiscountRatio = new FormattedDouble(_universalDiscount.DiscountValue / 100.0).FormattedNumber;
        return new FormattedDouble(price * universalDiscountRatio).FormattedNumber;
    }
    
}