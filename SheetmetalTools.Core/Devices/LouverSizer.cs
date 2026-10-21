using SheetmetalTools.Air;
using SheetmetalTools.Formulas;

namespace SheetmetalTools.Devices;

public class LouverSizer
{
    private LouverSizer(double velocity, double freeAir, Airflow airflow)
    {
        ArgumentNullException.ThrowIfNull(airflow);

        var converter = new Converter();

        Airflow = airflow;
        Velocity = velocity;
        FreeAir = freeAir;
        FreeAirFraction = converter.NumberToPercentige(freeAir);

        RequiredFreeArea = airflow.AreaFor(velocity);
    }

    private Airflow Airflow { get; }
    private double Velocity { get; }
    private double FreeAir { get; }
    private double FreeAirFraction { get; }
    private double RequiredFreeArea { get; }
    private double RequiredFaceArea => RequiredFreeArea / FreeAirFraction;
    private double FaceAreaInSquareInches => RequiredFaceArea * 144;

    public static Louver CreateSquare(double cfm, double velocity, double freeAir)
    {
        var airflow = new Airflow(cfm);
        var sizer = new LouverSizer(velocity, freeAir, airflow);
        return sizer.SizeSquare();
    }

    public static Louver CreateRect(double width, double cfm, double velocity, double freeAir)
    {
        var airflow = new Airflow(cfm);
        var sizer = new LouverSizer(velocity, freeAir, airflow);

        return sizer.SizeRect(width);
    }

    public static Louver CreateRound(double cfm, double velocity, double freeAir)
    {
        var airflow = new Airflow(cfm);
        var sizer = new LouverSizer(velocity, freeAir, airflow);

        return sizer.SizeRound();
    }

    public static Louver CreateOval(double cfm, double velocity, double freeAir)
    {
        var airflow = new Airflow(cfm);
        var sizer = new LouverSizer(velocity, freeAir, airflow);

        return sizer.SizeOval();
    }
    
    public static Louver CreateOval(double majorAxis, double cfm, double velocity, double freeAir)
    {
        var airflow = new Airflow(cfm);
        var sizer = new LouverSizer(velocity, freeAir, airflow);

        return sizer.SizeOval(majorAxis);
    }

    private Louver SizeSquare()
    {
        var side = Math.Sqrt(FaceAreaInSquareInches);
        return Louver.CreateRect(side, side, FreeAir, Airflow);
    }
    
    private Louver SizeRect(double width)
    {
        var formula = new Formula();
        formula.ValidateNumber(width, nameof(width));

        var height = FaceAreaInSquareInches / width;

        return Louver.CreateRect(width, height, FreeAir, Airflow);
    }

    private Louver SizeRound()
    {
        var diameter = 2 * Math.Sqrt(FaceAreaInSquareInches / Math.PI);
        return Louver.CreateRound(diameter, FreeAir, Airflow);
    }

    private Louver SizeOval()
    {
        var minorAxis = Math.Sqrt(FaceAreaInSquareInches / (1 + Math.PI / 4));
        var majorAxis = minorAxis * 2;

        return Louver.CreateOval(majorAxis, minorAxis, FreeAir, Airflow);
    }
    
    private Louver SizeOval(double majorAxis)
    {
        var formula = new Formula();
        formula.ValidateNumber(majorAxis, nameof(majorAxis));

        var area = FaceAreaInSquareInches;

        if (area > Math.PI * majorAxis * majorAxis / 4)
            throw new ArgumentOutOfRangeException(
                nameof(majorAxis),
                "Major axis is too small for the required area.");

        var factor = 1 - Math.PI / 4;
        var minorAxis = 2 * area /
                        (majorAxis + Math.Sqrt(majorAxis * majorAxis - 4 * factor * area));

        return Louver.CreateOval(majorAxis, minorAxis, FreeAir, Airflow);
    }
}