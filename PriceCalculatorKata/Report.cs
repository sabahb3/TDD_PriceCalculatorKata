using System.Globalization;
using PriceCalculatorKata.Enumerations;
using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Structures;

namespace PriceCalculatorKata;

public class Report
{
    private IProduct _product;
    private ISpecialDiscount _upcDiscounts;
    public Report(IProduct product, ISpecialDiscount upcDiscounts)
    {
        _product = product;
        _upcDiscounts = upcDiscounts;
    }

    public string DisplayProductReport()
    {
        int noDiscount = 0;
        var currencyCode = _product.CurrencyCode.CurrencyCode;
        var finalPrice = _product.FinalPrice.ToString("#0.00", CultureInfo.InvariantCulture);
        var discount = _product.Discount.ToString("#0.00", CultureInfo.InvariantCulture);
        var tax = _product.Tax.ToString("#0.00",CultureInfo.InvariantCulture);
        string message=$"Cost = {_product.Price} {currencyCode}\n ";
        message += $"Tax = {tax} {currencyCode}\n ";
        if(_product.Discount!=0) message += $"Discounts = {discount} {currencyCode}\n ";
        message += ReportingExpenses();
        message += $"TOTAL = {finalPrice} {currencyCode}\n ";
        if (_product.Discount == noDiscount)
        {
            message += "no discounts";
            return message;
        }
        if (_upcDiscounts.Contains(_product.UPC, out var upcDiscount))
        {
            message += $"{discount} {currencyCode} total discount";
            return message;
        }
        message += $"{discount} {currencyCode} discount";
        return message;
    }

    private string ReportingExpenses()
    {
        if(_product.Expenses==null)return String.Empty;
        string message=string.Empty;
        var expenses = _product.Expenses.Select(e => e.Type == PriceType.Absolute 
            ? $"{e.Description} = {e.Amount.ToString("#0.00", CultureInfo.InvariantCulture)} {_product.CurrencyCode.CurrencyCode}"
            : 
            $"{e.Description} = {new FormattedDouble(_product.Price * e.Amount).FormattedNumber.ToString("#0.00", CultureInfo.InvariantCulture)} {_product.CurrencyCode.CurrencyCode}");
        message+=String.Join("\n ",expenses)+"\n ";
        return message;
    }
}