namespace SheetmetalTools.Formulas;

public class Converter
{
    public double NumberToPercentige(double number)
    {
        if (number < 1 || number > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(number), "Number must be between 1 and 100");
        }

        return number / 100.0;
    }

    public double SqrInToWidthHeight(double number)
    {
        return Math.Sqrt(number);
    }
}