using SheetmetalTools.Formulas;

namespace SheetmetalTools.Air;

public class Air
{
    private Formula _formula = new Formula();
        
    private const double MolarMassOfDryAir = 0.0289644; //kg/mol
    private const double GasConstant = 8.314; // J/(mol*K)

    public string Name { get; }
    public double TemperatureC { get; }
    public double Pressurepa { get; }

    public double TemperatureK => TemperatureC + 273.15;

    // Dry-air density in kg/m³ (ideal gas law)
    public double Density =>
        (Pressurepa * MolarMassOfDryAir) / (GasConstant * TemperatureK);

    // Same density in lb/ft³, which fits CFM/fmp work (standard air = 0.075)
    public double DensityLbPerFt3 => Density * 0.062428;

    public Air(string name, double temperatureC, double pressurePa)
    {
        this.Name = name;
        this.TemperatureC = temperatureC;
        this.Pressurepa = _formula.ValidateNumber(pressurePa, nameof(pressurePa));
    }

    public static Air SupplyAir { get; } = new Air("Supply Air", 13, 101325);
    public static Air StandardAir{ get; } = new Air("Standard Air", 21.1, 101325);
    
}

