using SheetmetalTools.Formulas;

namespace SheetmetalTools.Shapes;

internal class Circle : Shape
{
    private readonly Formula _formula = new Formula();
    
    public override string Name => "Circle";
    
    public Circle(Double diameter)
    {
        this.Diameter = _formula.ValidateNumber(diameter, nameof(diameter));
    }
    
    public  double Diameter {get; private set; }

    public double Radius => Diameter / 2;

    public override double Perimeter => Math.PI * Diameter;

    public override double Area => Math.PI * Math.Pow(Radius, 2) / 144;

    public override string Size => $"{Diameter:0.##}\"";
}