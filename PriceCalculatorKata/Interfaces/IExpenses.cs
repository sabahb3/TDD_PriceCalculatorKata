namespace PriceCalculatorKata.Interfaces;

public interface IExpenses
{
    object Description { get; set; }
    object Amount { get; set; }
    object PriceType { get; set; }
}