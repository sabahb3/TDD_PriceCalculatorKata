namespace PriceCalculatorKata.Interfaces;

public interface ICalculations
{
    public double CalculateFinalPrice(IProduct product, IAccounting accounting);
    public double CalculateTax(IProduct product, IAccounting accounting);
    public double CalculateTotalDiscount(IProduct product, IAccounting accounting);
    public double UpcDiscount(IProduct product, IAccounting accounting);
    public double CalculateExpenses(IProduct product);
}