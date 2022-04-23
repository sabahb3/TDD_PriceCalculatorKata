using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Enumerations;

namespace PriceCalculatorKata;


public class Accounting : IAccounting
{
    private ITax _tax;
    private IDiscount _universalDiscount;
    private ISpecialDiscount _upcDiscount;
    private ICap _cap;

    public Accounting(ITax tax, IDiscount universalDiscount, ISpecialDiscount upcDiscount, ICap cap, CombinedDiscount combinedDiscount)
    {
        _tax = tax;
        _universalDiscount = universalDiscount;
        _upcDiscount = upcDiscount;
        _cap = cap;
        CombinedDiscount = combinedDiscount;
    }

    public int Tax
    {
        get
        {
            return _tax.TaxValue;
        }
    }

    public int UniversalDiscount { get;}

    public DiscountPrecedence UniversalDiscountPrecedence
    {
        get
        {
            return _universalDiscount.Precedence;
        }
        set
        {
            _universalDiscount.Precedence = value;
        }
    }

    public PriceType CapType
    {
        get
        {
            return _cap.Type;
        }
    }
    
    public CombinedDiscount CombinedDiscount { get; set; }

    public bool UpcDiscount(int upc, out Discount discount)
    {
        return _upcDiscount.Contains(upc, out discount);
    }

    public void AddUpcDiscount(int upc, Discount discount)
    {
        _upcDiscount.Add(upc,discount);
    }

    public void RemoveUpcDiscount(int upc)
    {
        _upcDiscount.Remove(upc);
    }

    public void ChangeTax(string newValue)
    {
        _tax.SetTax(newValue);
    }

    public void ChangeUniversalDiscount(string newValue)
    {
        _universalDiscount.SetDiscount(newValue);
    }

    public double CapAmount(double price)
    {
        return _cap.GetCapAmount(price);
    }

    public void ChangeCapAmount(double newValue)
    {
        _cap.Amount = newValue;
    }

    public void ChangeCapPriceType(PriceType priceType)
    {
        _cap.Type = priceType;
    }


}