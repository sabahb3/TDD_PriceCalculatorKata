namespace PriceCalculatorKata.Structures;

public struct FormattedDouble
{
    private readonly double _number;

    public FormattedDouble(double num)
    {
        _number = num;
    }

    public double FormattedNumber => Math.Round(_number, 4);
    public double DisplayedNumber => Math.Round(_number, 2);
}