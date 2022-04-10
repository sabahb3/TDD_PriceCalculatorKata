namespace PriceCalculatorKata;

public class SpecialDiscounts
{
    private Dictionary<int, Discount> _specialDiscount;

    public SpecialDiscounts()
    {
        _specialDiscount = new Dictionary<int, Discount>();
    }

    public void Add(int upc, Discount discount)
    {
        if (_specialDiscount.ContainsKey(upc))
        {
            _specialDiscount[upc] = discount;
        }
        else
        {
            _specialDiscount.Add(upc,discount);
        }
    }

    public int Count()
    {
       return _specialDiscount.Count;
    }

    public bool Contains(int upc)
    {
        return _specialDiscount.ContainsKey(upc);
    }
}