using System.Globalization;
using PriceCalculatorKata.Enumerations;
using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Structures;

namespace PriceCalculatorKata;

public class Report
{

    public string DisplayProductReport(IProduct product, IAccounting accounting)
    {
        int noDiscount = 0;
        var currencyCode = product.CurrencyCode;
        var finalPrice = new FormattedDouble(product.CalculateFinalPrice(accounting)).DisplayedNumber.ToString("#0.00", CultureInfo.InvariantCulture);
        var calculatedDiscount = product.CalculateTotalDiscount(accounting);
        var discount = new FormattedDouble(calculatedDiscount).DisplayedNumber.ToString("#0.00", CultureInfo.InvariantCulture);
        var tax = new FormattedDouble(product.CalculateTax(accounting)).DisplayedNumber.ToString("#0.00",CultureInfo.InvariantCulture);
        string message=$"Cost = {new FormattedDouble(product.Price).DisplayedNumber.ToString("#0.00", CultureInfo.InvariantCulture)} {currencyCode}\n ";
        message += $"Tax = {tax} {currencyCode}\n ";
        if(calculatedDiscount!=0) message += $"Discounts = {discount} {currencyCode}\n ";
        message += ReportingExpenses(product);
        message += $"TOTAL = {finalPrice} {currencyCode}\n ";
        if (calculatedDiscount == noDiscount)
        {
            message += "no discounts";
            return message;
        }
        if (product.UpcDiscount(accounting)!=0)
        {
            message += $"{discount} {currencyCode} total discount";
            return message;
        }
        message += $"{discount} {currencyCode} discount";
        return message;
    }

    private string ReportingExpenses(IProduct product)
    {
        if(product.Expenses==null||!product.Expenses.Any())return String.Empty;
        string message=string.Empty;
        var expenses = product.Expenses.Select(e => e.Type == PriceType.Absolute 
            ? $"{e.Description} = {new FormattedDouble(e.Amount).DisplayedNumber.ToString("#0.00", CultureInfo.InvariantCulture)} {product.CurrencyCode}"
            : 
            $"{e.Description} = {new FormattedDouble(product.Price * e.Amount).DisplayedNumber.ToString("#0.00", CultureInfo.InvariantCulture)} {product.CurrencyCode}");
        message+=String.Join("\n ",expenses)+"\n ";
        return message;
    }
}