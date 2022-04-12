using PriceCalculatorKata.Enumerations;
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

    public virtual double CalculateFinalPrice(double price, int upc, List<IExpenses> expenses,
        CombinedDiscount combinedDiscount)
    {
        return new FormattedDouble(
            price + CalculateTax(price,upc,combinedDiscount) - CalculateTotalDiscount(price, upc,combinedDiscount)+CalculateExpenses(expenses,price)).FormattedNumber;
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
        double remaining;
        if (_universalDiscount.Precedence == DiscountPrecedence.BeforeTax)
        {
            remaining = price - CalculateUniversalDiscount(price);
        }
        else
        {
            remaining = price - CalculateUPCDiscount(price, upc);
        }
        var taxRatio = new FormattedDouble(_tax.TaxValue / 100.0).FormattedNumber;
        return new FormattedDouble(remaining * taxRatio).FormattedNumber;
    }

    public virtual double CalculateTotalDiscount(double price, int upc, CombinedDiscount combinedDiscount)
    {
        DiscountPrecedence upcPrecedence=DiscountPrecedence.AfterTax;
        Discount discount;
        if (_upcDiscount.Contains(upc, out  discount))
            upcPrecedence = discount!.Precedence;
        if(upcPrecedence==_universalDiscount.Precedence)
             return CalculateUPCDiscount(price,upc) + CalculateUniversalDiscount(price);
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