using PriceCalculatorKata.Interfaces;
using PriceCalculatorKata.Enumerations;
using PriceCalculatorKata.Structures;
namespace PriceCalculatorKata;

public class Expense:IExpenses
{
    public Expense(string description, double amount, PriceType type)
    {
        Description = description;
        Type = type;
        if (type == PriceType.Absolute)
        {
            Amount = new FormattedDouble(amount).FormattedNumber;
        }
        else
        {
            var percentage = amount / 100.0;
            Amount = new FormattedDouble(percentage).FormattedNumber;
        }
    }

    public string Description { get; set; }
    public double Amount { get; private set; }
    public PriceType Type { get; set; }
}