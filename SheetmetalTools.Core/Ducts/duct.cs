using SheetmetalTools.Air;
using SheetmetalTools.Formulas;
using SheetmetalTools.Materials;
using SheetmetalTools.Shapes;

namespace SheetmetalTools.Ducts;

public class Duct
{
    public Duct(Shape shape, double length, Airflow? airflow = null, Material? material = null)
    {
        ArgumentNullException.ThrowIfNull(shape);
        this.Shape = shape;
        this.Length = length;
        this.Airflow = airflow;
        this.Material = material;
    }
    
    public Shape Shape { get; }
    public Airflow? Airflow { get; set; }
    public Material? Material { get; set; }
    public DuctLeakTest? LeakTest { get; set; }
    public double Length { get; }

    public static Duct CreateRect(double width, double height, double length, Airflow? airflow = null, Material? material = null)
    {
            var formula = new Formula();
            double validLength = formula.ValidateNumber(length, nameof(length));
                
        return new Duct(new Quadrilateral(width, height), validLength, airflow, material);
    }
    
    public static Duct CreateRound(double diameter, double length, Airflow? airflow = null, Material? material = null)
    {
        var formula = new Formula();
        double validLength = formula.ValidateNumber(length, nameof(length));
        
        return new Duct(new Circle(diameter), validLength, airflow, material);
    }
    
    public static Duct CreateOval(double majorAxis, double minorAxis, double length, Airflow? airflow = null, Material? material = null)
    {
        var formula = new Formula();
        double validLength = formula.ValidateNumber(length, nameof(length));
        
        return new Duct(new FlatOval(majorAxis, minorAxis), validLength, airflow, material);
    }
    
    public string Info()
    {
        return $"Shape:        {Shape.Name}\n" +
               $"Size:         {Shape.Size}\n" +
               $"Length:       {Length}\"";
    }
}