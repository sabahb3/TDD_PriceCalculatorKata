using PriceCalculatorKata.Enumerations;
using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Structures;

namespace PriceCalculatorKata;

public class Cap : ICap
{
    public Cap(double amount, PriceType type)
    {
        Type = type;
        if (type==PriceType.Absolute)
        {
            Amount = amount;
        }
        else
        {
            Amount = new FormattedDouble(amount / 100).FormattedNumber;
        }
    }


    public double Amount { get; set; }
    public PriceType Type { get; set; }

    public double GetCapAmount(double price)
    {
        if (Type==PriceType.Absolute)
        {
            return Amount;
        }
        else
        {
            return new FormattedDouble(price * Amount).FormattedNumber;
        }
    }
    
}