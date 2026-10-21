using SheetmetalTools.Air;
using SheetmetalTools.Formulas;
using SheetmetalTools.Shapes;

namespace SheetmetalTools.Devices;

public class Louver
{
    public Louver(Shape shape, double freeAir, Airflow? airflow = null)
    {
        ArgumentNullException.ThrowIfNull(shape);
        this.Shape = shape;
        this.FreeAir = freeAir;
        this.Airflow = airflow;
        this.FreeAirFraction = _convert.NumberToPercentige(freeAir);
    }
    
    public static Louver CreateRect(double width, double height, double freeAir, Airflow? airflow = null)
    {
        return new Louver(new Quadrilateral(width, height), freeAir, airflow);
    }
    
    public static Louver CreateRound(double diameter, double freeAir, Airflow? airflow = null)
    {
        return new Louver(new Circle(diameter), freeAir, airflow);
    }
    
    public static Louver CreateOval(double majorAxis, double minorAxis, double freeAir, Airflow? airflow = null)
    {
        return new Louver(new FlatOval(majorAxis, minorAxis), freeAir, airflow);
    }
    
    public Airflow? Airflow { get; set; }
    private Converter _convert = new Converter();
    public Shape Shape { get; }
    public double? Width => Shape is Quadrilateral rect ? rect.WidthIn : null;
    public double? Height => Shape is Quadrilateral rect ? rect.HeightIn : null;
    public double? Diameter => Shape is Circle cir ? cir.Diameter : null;
    public double? MajorAxis => Shape is FlatOval oval ? oval.MajorAxis : null;
    public double? MinorAxis => Shape is FlatOval oval ? oval.MinorAxis : null;
    public double FreeAir { get; private set; }
    public string Size => Shape.Size;
    public double FreeAirFraction { get; private set; }
    public double? Velocity => Airflow?.VelocityFor(FreeArea);
    public string VelocityText => Velocity?.ToString("0.##") ?? "N/A";
    public double? Cfm => Airflow?.Cfm; 
    public string CfmText => Cfm?.ToString("0.##") ?? "N/A";

    public double Area => Shape.Area;

    public double FreeArea => Area * FreeAirFraction;

    public string Info()
    {
        var freeAir = FreeAirFraction * 100;
        var area = Math.Round(Area, 2);
        var freeArea = Math.Round(FreeArea, 2);
        
        return $"Shape:        {Shape.Name}\n" +
               $"Size:         {Size}\n" +
               $"Free Air:     {freeAir}%\n" +
               $"Area:         {area} sqft\n" +
               $"Free Area:    {freeArea} sqft\n" +
               $"CFM:          {CfmText}\n" +
               $"Velocity:     {VelocityText} FPM";
    }
}   

