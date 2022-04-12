using PriceCalculatorKata.Enumerations;
using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Structures;

namespace PriceCalculatorKata;

public class Calculations
{
    private ITax _tax;
    private IDiscount _universalDiscount;
    private ISpecialDiscount _upcDiscount;
    private ICap _cap;

    public Calculations(ITax tax, IDiscount universalDiscount, ISpecialDiscount upcDiscount,ICap cap)
    {
        _tax = tax;
        _universalDiscount = universalDiscount;
        _upcDiscount = upcDiscount;
        _cap = cap;
    }

    public virtual CombinedDiscount CombinedDiscount { get; set; }

    public virtual double CalculateFinalPrice(double price, int upc, List<IExpenses> expenses,
        CombinedDiscount combinedDiscount)
    {
        return new FormattedDouble(
            price + CalculateTax(price, upc, combinedDiscount) - CalculateTotalDiscount(price, upc, combinedDiscount) + 
            CalculateExpenses(expenses, price)).FormattedNumber;
    }
    public virtual double CalculateTax(double price, int upc, CombinedDiscount combinedDiscount)
    {
        DiscountPrecedence upcPrecedence=DiscountPrecedence.AfterTax;
        if (_upcDiscount.Contains(upc, out var discount))
            upcPrecedence = discount!.Precedence;

        if (upcPrecedence == _universalDiscount.Precedence)
        {
            if (upcPrecedence == DiscountPrecedence.AfterTax)
                return CalculateTaxBeforeAllDiscount(price);
            else
            {
                return CalculateTaxAfterAllDiscount(price, upc,combinedDiscount);
            }
        }
        else
            return CalculateTaxAfterOneDiscount(price,upc);
    }
    
    public double CalculateTaxBeforeAllDiscount(double price)
    {
        var taxRatio = new FormattedDouble(_tax.TaxValue / 100.0).FormattedNumber;
        return new FormattedDouble(price * taxRatio).FormattedNumber;
    }
    
    private double CalculateTaxAfterAllDiscount(double price, int upc,CombinedDiscount combinedDiscount)
    {
        var discounts = CalculateTotalDiscount(price, upc,combinedDiscount);
        var remaining = price - discounts;
        var taxRatio = new FormattedDouble(_tax.TaxValue / 100.0).FormattedNumber;
        return new FormattedDouble(remaining * taxRatio).FormattedNumber;
    }
    
    private double CalculateTaxAfterOneDiscount(double price, int upc)
    {
        double discount;
        if (_universalDiscount.Precedence == DiscountPrecedence.BeforeTax)
        {
            discount = CalculateUniversalDiscount(price);
        }
        else
        {
          discount = CalculateUPCDiscount(price, upc);
        }

        var cap = _cap.GetCapAmount(price);
        if (discount > cap) discount = cap;
        var remaining = price - discount;
        var taxRatio = new FormattedDouble(_tax.TaxValue / 100.0).FormattedNumber;
        return new FormattedDouble(remaining * taxRatio).FormattedNumber;
    }
    public virtual double CalculateTotalDiscount(double price, int upc, CombinedDiscount combinedDiscount)
    {
        var actualDiscount = CalculateActualTotalDiscount(price, upc, combinedDiscount);
        var cap = _cap.GetCapAmount(price);
        var discount = actualDiscount > cap ? cap : actualDiscount;
        return discount;
    }

    private double CalculateActualTotalDiscount(double price, int upc, CombinedDiscount combinedDiscount)
    {
        DiscountPrecedence upcPrecedence=DiscountPrecedence.AfterTax;
        Discount discount;
        if (_upcDiscount.Contains(upc, out  discount))
            upcPrecedence = discount!.Precedence;
        if(upcPrecedence==_universalDiscount.Precedence)
            return CombiningDiscounts(price,upc,combinedDiscount);
        else
        {
            switch (upcPrecedence)
            {
                case DiscountPrecedence.BeforeTax:
                    return CalculateOneDiscountBefore(price, discount!.DiscountValue, _universalDiscount.DiscountValue);
                default:
                    return CalculateOneDiscountBefore(price, _universalDiscount.DiscountValue, discount!.DiscountValue);
            }
        }
    }
    private double CombiningDiscounts(double price, int upc, CombinedDiscount combinedDiscount)
    {
        if (combinedDiscount == CombinedDiscount.Additive)
        {
            return CalculateUPCDiscount(price,upc) + CalculateUniversalDiscount(price);
        }
        var universalDiscount= CalculateUniversalDiscount(price);
        var remaining = price - universalDiscount;
        return universalDiscount + CalculateUPCDiscount(remaining, upc);
    }
    private double CalculateOneDiscountBefore(double price, int firstDiscount, int secondDiscount)
    {
        var discounts= CalculateForOneDiscount(price, firstDiscount);
        var remaining = price - discounts;
        return discounts + CalculateForOneDiscount(remaining, secondDiscount);

    }
    private  double CalculateForOneDiscount(double price, int discountValue)
    {
        var discountRatio = new FormattedDouble( discountValue / 100.0).FormattedNumber;
        return new FormattedDouble(price * discountRatio).FormattedNumber;
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

    public double CalculateExpenses(List<IExpenses> expenses, double price)
    {
        var expenseCost = expenses.Where(e => e.Type == PriceType.Percentage)
            .Select(e => new FormattedDouble(price * e.Amount).FormattedNumber).Sum();
        expenseCost += expenses.Where(e => e.Type == PriceType.Absolute)
            .Select(e => new FormattedDouble(e.Amount).FormattedNumber).Sum();
        return new FormattedDouble(expenseCost).FormattedNumber;
    }
}