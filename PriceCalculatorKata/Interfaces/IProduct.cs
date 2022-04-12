using PriceCalculatorKata.Structures;
namespace PriceCalculatorKata.Interfaces;

public interface IProduct
{
    public double Price { get; }
    public int UPC { get; }
    public string Name { get; set; }
    public double Tax { get; }
    public double FinalPrice { get; }
    public double Discount { get; }
    public List<IExpenses> Expenses { get; }
    public string CurrencyCode { get; }

}