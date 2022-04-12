using PriceCalculatorKata.Enumerations;

namespace PriceCalculatorKata.Interfaces;

public interface ICap
{
    public double Amount { set; }
    public PriceType Type { get; set; }
    public double GetCapAmount(double price);
}