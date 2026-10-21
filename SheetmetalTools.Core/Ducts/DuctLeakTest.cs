using SheetmetalTools.Formulas;
using SheetmetalTools.Shapes;

namespace SheetmetalTools.Ducts;

public class DuctLeakTest
{
    public DuctLeakTest(Duct duct, string leakClass, double waterGauge)
    {
        ArgumentNullException.ThrowIfNull(duct);

        var intLeakClass = GetLeakClassInt(duct, leakClass);
        
        this.Duct = duct;
        this.StringLeakClass = leakClass;
        this.IntLeakClass = intLeakClass;
        this.WaterGauge = _formula.ValidateNumber(waterGauge, nameof(waterGauge));
    }

    private Formula _formula = new Formula();
    private Duct Duct { get; }
    public string StringLeakClass { get; }
    public int IntLeakClass { get; }
    public double WaterGauge { get; }
    private double SurfaceArea => Duct.Shape.Perimeter * Duct.Length / 144;

    private Dictionary<string, int> _rectLeakClasses = new Dictionary<string, int>
    {
        { "a", 4 },
        { "b", 8 },
        { "c", 16 },
    };

    private Dictionary<string, int> _roundLeakClasses = new Dictionary<string, int>
    {
        { "a", 2 },
        { "b", 4 },
        { "c", 8 },
    };

    public double AllowableLeakage => _formula.AllowableLeakage(IntLeakClass, SurfaceArea, WaterGauge);

    private int GetLeakClassInt(Duct duct, string leakClass)
    {
        var cleanLeakClass = _formula.Clean(leakClass);
        
        if (duct.Shape is Quadrilateral)
        {
            if (_rectLeakClasses.TryGetValue(cleanLeakClass, out var value))
                return value;

            throw new ArgumentException(
                "Seal class must be A, B, or C.", nameof(leakClass));
        }

        if (duct.Shape is Circle || duct.Shape is FlatOval)
        {
            if (_roundLeakClasses.TryGetValue(cleanLeakClass, out var value))
                return value;

            throw new ArgumentException(
                "Seal class must be A, B, or C.", nameof(leakClass));
        }
        
        throw new ArgumentException(
            "Unsupported duct shape.", nameof(duct));
    }
}