using PriceCalculatorKata.Structures;
namespace PriceCalculatorKata.Interfaces;

public interface IProduct
{
    public double Price { get; }
    public int UPC { get; }
    public string Name { get; set; }
    public List<IExpenses> Expenses { get; }
    public Currency ProductCurrency { get; }
    public string CurrencyCode { get; }
}