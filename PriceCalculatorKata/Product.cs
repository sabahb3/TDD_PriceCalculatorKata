using PriceCalculatorKata.Interfaces;

namespace PriceCalculatorKata;

public class Product
{
    private ITax _tax;
    public Product(int upc, string name, double price, ITax tax)
    {
        UPC = upc;
        Name = name ?? String.Empty;
        Price = price;
        _tax = tax;
    }

    public  double Price { get; private set; }
    public int UPC { get; private set; }
    public string Name { get; set; }=String.Empty;

    public double Tax => Price * (_tax.TaxValue / 100.0);
}