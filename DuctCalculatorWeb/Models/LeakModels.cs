using SheetmetalTools.Ducts;

namespace DuctCalculatorWeb.Models;

public class LeakDuct
{
    public string Type { get; set; } = "RND";
    private double _diameter, _width, _height, _length, _testPressure = 1;

    public double Diameter { get => _diameter; set => _diameter = Math.Clamp(value, 0, 50); }
    public double Width { get => _width; set => _width = Math.Clamp(value, 0, 50); }
    public double Height { get => _height; set => _height = Math.Clamp(value, 0, 50); }
    public double Length { get => _length; set => _length = Math.Clamp(value, 0, 50); }
    public string LeakClass { get; set; } = "A";
    public double TestPressure { get => _testPressure; set => _testPressure = Math.Clamp(value, 0, 10); }

    public double Sfa => Build() is { } duct ? duct.Shape.Perimeter / 12 : 0;

    public double Sfd => Sfa * Length;

    public double AllowableLeakage =>
        Build() is { } duct ? new DuctLeakTest(duct, LeakClass, TestPressure).AllowableLeakage : 0;

    // The library rejects zero or negative sizes, so incomplete rows count as 0.
    private Duct? Build()
    {
        if (Length <= 0 || TestPressure <= 0)
        {
            return null;
        }

        return Type switch
        {
            "RND" when Diameter > 0 => Duct.CreateRound(Diameter, Length * 12),
            "SQR" when Width > 0 && Height > 0 => Duct.CreateRect(Width, Height, Length * 12),
            "OVL" when Width > 0 && Height > 0 => Duct.CreateOval(Width, Height, Length * 12),
            _ => null
        };
    }
}

public class LeakSave
{
    public string JobName { get; set; } = "";
    public string JobNumber { get; set; } = "";
    public string System { get; set; } = "";
    public string Unit { get; set; } = "";
    public DateTime? Date { get; set; }
    public string Witness { get; set; } = "";
    public List<LeakBranch> Branches { get; set; } = new();
}

public class LeakSection
{
    public string Name { get; set; } = "";
    public List<LeakDuct> Ducts { get; set; } = new() { new LeakDuct() };
    public double Total => Ducts.Sum(d => d.AllowableLeakage);
}

public class LeakRun
{
    public string Name { get; set; } = "";
    public List<LeakSection> Sections { get; set; } = new() { new LeakSection() };
    public double Total => Sections.Sum(s => s.Total);
}

public class LeakBranch
{
    public string Name { get; set; } = "";
    public List<LeakRun> Runs { get; set; } = new() { new LeakRun() };
    public double Total => Runs.Sum(r => r.Total);
}
