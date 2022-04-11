using System.Globalization;
using PriceCalculatorKata.Interfaces;

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
        var finalPrice = _product.FinalPrice.ToString("#.00", CultureInfo.InvariantCulture);
        var discount = _product.Discount.ToString("#.00", CultureInfo.InvariantCulture);
        string message=$"price ${finalPrice} \n";
        if(_product.Discount==noDiscount)
            return message;
        if (_upcDiscounts.Contains(_product.UPC, out var upcDiscount))
        {
            message += $"total discount amount ${discount}";
            return message;
        }
        message += $"discount amount ${discount}";
        return message;
    }
}