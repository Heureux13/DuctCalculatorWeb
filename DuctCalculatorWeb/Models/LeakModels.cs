using SheetmetalTools.Ducts;

namespace DuctCalculatorWeb.Models;

public class LeakDuct
{
    public string Type { get; set; } = "RND";
    private double? _diameter, _width, _height, _length, _testPressure = 1;

    public double? Diameter { get => _diameter; set => _diameter = Limit(value, 50); }
    public double? Width { get => _width; set => _width = Limit(value, 50); }
    public double? Height { get => _height; set => _height = Limit(value, 50); }
    public double? Length { get => _length; set => _length = Limit(value, 50); }
    public string LeakClass { get; set; } = "A";
    public string? Photo { get; set; }
    public double? TestPressure { get => _testPressure; set => _testPressure = Limit(value, 10); }

    // Empty or zero becomes blank; anything above the cap is capped.
    private static double? Limit(double? value, double max) =>
        value is null or <= 0 ? null : Math.Min(value.Value, max);

    public bool IsComplete => Build() is not null;

    public double Sfa => Build() is { } duct ? duct.Shape.Perimeter / 12 : 0;

    public double Sfd => Sfa * (Length ?? 0);

    public double AllowableLeakage =>
        Build() is { } duct ? new DuctLeakTest(duct, LeakClass, TestPressure!.Value).AllowableLeakage : 0;

    // The library rejects zero or missing sizes, so incomplete rows count as 0.
    private Duct? Build()
    {
        if (Length is not { } length || TestPressure is null)
        {
            return null;
        }

        return Type switch
        {
            "RND" when Diameter is { } d => Duct.CreateRound(d, length * 12),
            "SQR" when Width is { } w && Height is { } h => Duct.CreateRect(w, h, length * 12),
            "OVL" when Width is { } w && Height is { } h => Duct.CreateOval(w, h, length * 12),
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
    public string Name { get; set; } = "S01";
    public List<LeakDuct> Ducts { get; set; } = new() { new LeakDuct() };
    public double Total => Ducts.Sum(d => d.AllowableLeakage);
}

public class LeakRun
{
    public string Name { get; set; } = "R01";
    public List<LeakSection> Sections { get; set; } = new() { new LeakSection() };
    public double Total => Sections.Sum(s => s.Total);
}

public class LeakBranch
{
    public string Name { get; set; } = "B01";
    public List<LeakRun> Runs { get; set; } = new() { new LeakRun() };
    public double Total => Runs.Sum(r => r.Total);
}
