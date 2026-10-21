using SheetmetalTools.Formulas;

namespace SheetmetalTools.Shapes;

internal class FlatOval : Shape
{
    private Formula _formula = new Formula();
    
    public override string Name => "Flat Oval";
    
    public FlatOval(double majorAxis, double minorAxis)
    {
        this.MajorAxis = _formula.ValidateNumber(majorAxis, nameof(majorAxis));
        this.MinorAxis = _formula.ValidateNumber(minorAxis, nameof(minorAxis));
    }
    
    public double MajorAxis {get; private set; }
    public double MinorAxis {get; private set; }

    public override double Perimeter => (2 * (MajorAxis - MinorAxis)) + (Math.PI * MinorAxis);
    

    public override double Area => 
        (MinorAxis * (MajorAxis - MinorAxis) + (Math.PI * Math.Pow(MinorAxis, 2) / 4))/ 144;

    public override string Size => $"{MajorAxis:0.##}\"x{MinorAxis:0.##}\"";
}