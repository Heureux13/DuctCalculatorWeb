using SheetmetalTools.Formulas;

namespace SheetmetalTools.Air ;

public class Airflow
{
    private Formula _formula = new Formula();
    public Air AirType { get;}

    public double Cfm { get; }
    
    public Airflow(double cfm, Air? air = null)
    {
        this.Cfm = _formula.ValidateNumber(cfm, nameof(cfm));
        this.AirType = air ?? Air.StandardAir;
    }

    public double VelocityFor(double area) => Cfm / _formula.ValidateNumber(area,nameof(area));
    public double AreaFor(double velocity) => Cfm / _formula.ValidateNumber(velocity, nameof(velocity));

    public string Info()
    {
        return $"CFM:          {Cfm}\n" +
               $"Air Type:     {AirType.Name}";
    }
}