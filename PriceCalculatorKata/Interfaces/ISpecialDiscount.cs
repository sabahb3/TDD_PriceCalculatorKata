namespace PriceCalculatorKata.Interfaces;

public interface ISpecialDiscount
{
    public void Add(int upc, Discount discount);
    public int Count();
    public bool Contains(int upc,out Discount? discountValue);
    public void Remove(int upc);

}