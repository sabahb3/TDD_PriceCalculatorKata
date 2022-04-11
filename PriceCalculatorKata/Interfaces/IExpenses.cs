using PriceCalculatorKata.Enumerations;
namespace PriceCalculatorKata.Interfaces;

public interface IExpenses
{
    string Description { get; set; }
    double Amount { get; }
    PriceType Type { get; set; }
}