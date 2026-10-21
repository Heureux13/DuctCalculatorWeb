using SheetmetalTools.Air;
using SheetmetalTools.Formulas;
using SheetmetalTools.Materials;
using SheetmetalTools.Shapes;

namespace SheetmetalTools.Ducts;

public class FrictionLoss
{
    public FrictionLoss(Duct duct)
    {
        ArgumentNullException.ThrowIfNull(duct);

        Duct = duct;
        Airflow = duct.Airflow
                  ?? throw new ArgumentException("Duct must have airflow.", nameof(duct));
        Material = duct.Material
                   ?? throw new ArgumentException("Duct must have material.", nameof(duct));
    }
    
    private Duct Duct { get; }
    private readonly Formula _formula = new Formula();
    private Shape Shape => Duct.Shape;
    private Airflow Airflow { get; }
    private Material Material { get; }
    private double Area => Duct.Shape.Area;
    private double Perimeter => Duct.Shape.Perimeter;
    private double Velocity => Airflow.VelocityFor(Area);
    private double Density => Airflow.AirType.Density;
    private double DensityLb => Airflow.AirType.DensityLbPerFt3;
    private double TemperatureK=> Airflow.AirType.TemperatureK;
    private double Roughness => Material.Roughness * 12;
    public double HydraulicDiameter => _formula.HydraulicDiameter(Area * 144, Perimeter);
    public double DynamicViscosity => _formula.DynamicViscosity(TemperatureK);
    public double KinematicViscosity => _formula.KinematicViscosityFt2PerS(DynamicViscosity,Density);
    public double ReynoldsNumber => _formula.ReynoldsNumber(Velocity, HydraulicDiameter,KinematicViscosity);
    public double FrictionFactor => _formula.DarcyFrictionFactor(ReynoldsNumber, Roughness, HydraulicDiameter);
    public double FrictionLossPer100Ft => _formula.FrictionLoss(FrictionFactor, HydraulicDiameter, DensityLb, Velocity);

    public string Info()
    {
        return $"Duct Shape:            {Duct.Shape.Name}\n" +
               $"CFM:                   {Duct.Airflow?.Cfm}\n" +
               $"Velocity:              {Duct.Airflow?.VelocityFor(Duct.Shape.Area):0.##}\n" +
               $"Hydraulic Diameter:    {HydraulicDiameter:0.0000}\n" +
               $"Dynamic Viscosity:     {DynamicViscosity:0.########}\n" +
               $"Kinematic Viscosity:   {KinematicViscosity:0.########}\n" +
               $"Reynolds Number:       {ReynoldsNumber:0.##}\n" +
               $"Friction Factor:       {FrictionFactor:0.0000}\n" +
               $"Friction Loss / 100FT: {FrictionLossPer100Ft:0.0000}";
    }

    public double GetFrictionLoss()
    {
        return FrictionLossPer100Ft;
    }



}