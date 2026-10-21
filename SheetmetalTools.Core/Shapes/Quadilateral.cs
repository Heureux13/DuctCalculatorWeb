using SheetmetalTools.Formulas;

namespace SheetmetalTools.Shapes;

internal class Quadrilateral : Shape
{
    private Formula _formula = new Formula();
    public override string Name => "Quadrilateral";
    
    public Quadrilateral(double widthIn, double heightIn)
    {
        this.WidthIn = _formula.ValidateNumber(widthIn, nameof(widthIn));
        this.HeightIn =_formula.ValidateNumber(heightIn, nameof(heightIn));
    }
    
    public  double WidthIn {get; private set; }
    public  double HeightIn {get; private set;}

    public override double Perimeter => WidthIn * 2 + HeightIn * 2;

    public override double Area => WidthIn * HeightIn / 144;

    public override string Size => $"{WidthIn:0.##}\"x{HeightIn:0.##}\"";
}