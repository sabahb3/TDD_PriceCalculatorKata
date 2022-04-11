using PriceCalculatorKata.Interfaces;
namespace PriceCalculatorKata;

public class SpecialDiscounts: ISpecialDiscount
{
    private Dictionary<int, Discount> _specialDiscountContainer;
    private static SpecialDiscounts _specialDiscounts= new SpecialDiscounts();

    private SpecialDiscounts()
    {
        _specialDiscountContainer = new Dictionary<int, Discount>();
    }

    public static SpecialDiscounts GetInstance()
    {
        return _specialDiscounts;
    }

    public void Add(int upc, Discount discount)
    {
        if (_specialDiscountContainer.ContainsKey(upc))
        {
            _specialDiscountContainer[upc] = discount;
        }
        else
        {
            _specialDiscountContainer.Add(upc,discount);
        }
    }

    public int Count()
    {
       return _specialDiscountContainer.Count;
    }

    public bool Contains(int upc,out Discount? discountValue)
    {
        
        discountValue=_specialDiscountContainer.ContainsKey(upc)?_specialDiscountContainer[upc]:null;
        return _specialDiscountContainer.ContainsKey(upc);
    }

    public void Remove(int upc)
    {
        if (Contains(upc,out var discount)) _specialDiscountContainer.Remove(upc);
    }
    
}