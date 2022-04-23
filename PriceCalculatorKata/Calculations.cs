using PriceCalculatorKata.Enumerations;
using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Structures;

namespace PriceCalculatorKata;

public static class Calculations 
{
    public static double CalculateFinalPrice(this IProduct product, IAccounting accounting)
    {
        return new FormattedDouble(
            product.Price + CalculateTax(product,accounting) 
            - CalculateTotalDiscount(product,accounting) + 
            CalculateExpenses(product)).FormattedNumber;
    }
    public static  double CalculateTax(this IProduct product,IAccounting accounting)
    {
        DiscountPrecedence upcPrecedence=DiscountPrecedence.AfterTax;
        if (accounting.UpcDiscount(product.UPC, out var discount))
            upcPrecedence = discount!.Precedence;

        if (upcPrecedence == accounting.UniversalDiscountPrecedence)
        {
            if (upcPrecedence == DiscountPrecedence.AfterTax)
                return CalculateTaxBeforeAllDiscount(product.Price,accounting.Tax);
            else
            {
                return CalculateTaxAfterAllDiscount(product,accounting);
            }
        }
        else
            return CalculateTaxAfterOneDiscount(product.Price,product.UPC,accounting);
    }
    public static double CalculateTaxBeforeAllDiscount(double price,int tax)
    {
        var taxRatio = new FormattedDouble(tax/ 100.0).FormattedNumber;
        return new FormattedDouble(price * taxRatio).FormattedNumber;
    }
    private static double CalculateTaxAfterAllDiscount(IProduct product, IAccounting accounting)
    {
        var discounts = CalculateTotalDiscount(product,accounting);
        var remaining = product.Price - discounts;
        var taxRatio = new FormattedDouble(accounting.Tax / 100.0).FormattedNumber;
        return new FormattedDouble(remaining * taxRatio).FormattedNumber;
    }
    private static double CalculateTaxAfterOneDiscount(double price, int upc,IAccounting accounting)
    {
        double discount;
        if (accounting.UniversalDiscountPrecedence == DiscountPrecedence.BeforeTax)
        {
            discount = CalculateForOneDiscount(price,accounting.UniversalDiscount);
        }
        else
        {
          discount = CalculateUpcDiscount(price, upc,accounting);
        }

        var cap = accounting.CapAmount(price);
        if (discount > cap) discount = cap;
        var remaining = price - discount;
        var taxRatio = new FormattedDouble(accounting.Tax / 100.0).FormattedNumber;
        return new FormattedDouble(remaining * taxRatio).FormattedNumber;
    }
    public static double CalculateTotalDiscount(this IProduct product, IAccounting accounting)
    {
        var actualDiscount = CalculateActualTotalDiscount(product,accounting);
        var cap = accounting.CapAmount(product.Price);
        var discount = actualDiscount > cap ? cap : actualDiscount;
        return discount;
    }
    private static double CalculateActualTotalDiscount(IProduct product, IAccounting accounting)
    {
        DiscountPrecedence upcPrecedence=DiscountPrecedence.AfterTax;
        Discount discount;
        if (accounting.UpcDiscount(product.UPC,out discount))
            upcPrecedence = discount!.Precedence;
        if(upcPrecedence==accounting.UniversalDiscountPrecedence)
            return CombiningDiscounts(product,accounting);
        else
        {
            switch (upcPrecedence)
            {
                case DiscountPrecedence.BeforeTax:
                    return CalculateOneDiscountBefore(product.Price, discount!.DiscountValue, accounting.UniversalDiscount);
                default:
                    return CalculateOneDiscountBefore(product.Price, accounting.UniversalDiscount, discount!.DiscountValue);
            }
        }
    }
    private static double CombiningDiscounts(IProduct product, IAccounting accounting)
    {
        if (accounting.CombinedDiscount == CombinedDiscount.Additive)
        {
            return CalculateUpcDiscount(product.Price,product.UPC,accounting) + CalculateForOneDiscount(product.Price,accounting.UniversalDiscount);
        }
        var universalDiscount= CalculateForOneDiscount(product.Price,accounting.UniversalDiscount);
        var remaining = product.Price - universalDiscount;
        return universalDiscount + CalculateUpcDiscount(remaining, product.UPC,accounting);
    }
    private static double CalculateOneDiscountBefore(double price, int firstDiscount, int secondDiscount)
    {
        var discounts= CalculateForOneDiscount(price, firstDiscount);
        var remaining = price - discounts;
        return discounts + CalculateForOneDiscount(remaining, secondDiscount);

    }
    private  static double CalculateForOneDiscount(double price, int discountValue)
    {
        var discountRatio = new FormattedDouble( discountValue / 100.0).FormattedNumber;
        return new FormattedDouble(price * discountRatio).FormattedNumber;
    }
    public static double CalculateUpcDiscount(double price, int upc,IAccounting accounting)
    {
        double upcDiscount=0d;
        if (accounting.UpcDiscount(upc,out var discount))
        {
            var upcDiscountRatio = new FormattedDouble(discount!.DiscountValue / 100.0).FormattedNumber;
            upcDiscount = new FormattedDouble(price * upcDiscountRatio).FormattedNumber;
        }
        return upcDiscount;
    }
    public static double UpcDiscount(this IProduct product,IAccounting accounting)
    {
        double upcDiscount=0d;
        if (accounting.UpcDiscount(product.UPC,out var discount))
        {
            var upcDiscountRatio = new FormattedDouble(discount!.DiscountValue / 100.0).FormattedNumber;
            upcDiscount = new FormattedDouble(product.Price * upcDiscountRatio).FormattedNumber;
        }
        return upcDiscount;
    }
    public static double CalculateExpenses(this IProduct product)
    {
        var expenseCost = product.Expenses.Where(e => e.Type == PriceType.Percentage)
            .Select(e => new FormattedDouble(product.Price * e.Amount).FormattedNumber).Sum();
        expenseCost += product.Expenses.Where(e => e.Type == PriceType.Absolute)
            .Select(e => new FormattedDouble(e.Amount).FormattedNumber).Sum();
        return new FormattedDouble(expenseCost).FormattedNumber;
    }
}