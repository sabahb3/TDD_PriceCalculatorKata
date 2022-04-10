using System.Globalization;
using PriceCalculatorKata.Interfaces;

namespace PriceCalculatorKata;

public class Report
{
    private IProduct _product;
    public Report(IProduct product)
    {
        _product = product;
    }

    public string DisplayProductReport()
    {
        int noDiscount = 0;
        var finalPrice = _product.FinalPrice.ToString("#.00", CultureInfo.InvariantCulture);
        var discount = _product.Discount.ToString("#.00", CultureInfo.InvariantCulture);
        string message=$"price ${finalPrice} \n";
        if(_product.Discount==noDiscount)
            return message;
        message += $"${discount} amount which was deduced";
        return message;
    }
}