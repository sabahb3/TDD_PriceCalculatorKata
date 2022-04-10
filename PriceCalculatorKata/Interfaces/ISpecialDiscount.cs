namespace PriceCalculatorKata.Interfaces;

public interface ISpecialDiscount
{
    public void Add(int upc, Discount discount);
    public int Count();
    public bool Contains(int upc);
    public void Remove(int upc);

}