using System.Security.Cryptography;
using PriceCalculatorKata.Enumerations;

namespace PriceCalculatorKata.Interfaces;

public interface IAccounting
{
    public int Tax { get;}
    public int UniversalDiscount { get;}
    public DiscountPrecedence UniversalDiscountPrecedence { get; set; }
    public CombinedDiscount CombinedDiscount { get; set; }
    public PriceType CapType { get;}

    public bool UpcDiscount(int upc, out Discount discount);
    public void AddUpcDiscount(int upc, Discount discount);
    public void RemoveUpcDiscount(int upc);
    public void ChangeTax(string newValue);
    public void ChangeUniversalDiscount(string newValue);
    public double CapAmount(double price);
    public void ChangeCapAmount(double newValue);
    public void ChangeCapPriceType(PriceType priceType);

}