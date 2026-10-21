using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using SheetmetalTools.Air;
using SheetmetalTools.Formulas;
using SheetmetalTools.Materials;

namespace SheetmetalTools.Ducts;

public class Duculator
{
    public Duculator(Duct duct, double velocity = 600)
    {
        ArgumentNullException.ThrowIfNull(duct);
        
        this.Length = duct.Length;
        this.TargetVelocity = _formula.ValidateNumber(velocity, nameof(velocity));
        
        Airflow = duct.Airflow
                  ?? throw new ArgumentException("Duct must have airflow.", nameof(duct));
        Material = duct.Material
                   ?? throw new ArgumentException("Duct must have material.", nameof(duct));
        TargetFriction = new FrictionLoss(duct).FrictionLossPer100Ft;
    }

    public Duculator(double cfm, double frictionLoss, double length, double velocity = 600, Material? material = null)
    {
        this.Airflow = new Airflow(cfm);
        this.TargetVelocity = _formula.ValidateNumber(velocity, nameof(velocity));
        this.Length = _formula.ValidateNumber(length, nameof(length));
        this.Material = material ?? Material.Galvanized;
        this.TargetFriction = _formula.ValidateNumber(frictionLoss, nameof(frictionLoss));
    }

    private readonly Formula _formula = new Formula();
    private Airflow Airflow { get; }
    private Material Material { get; }
    private double TargetFriction { get; }
    private double TargetVelocity { get; }
    private double Length { get; }
    public int DivisionCount { get; private set; }
    
    private Duct NewDuct(Func<double, Duct> createCandidate, double maxSize = 1000)
    {
        if (!double.IsFinite(maxSize) || maxSize < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxSize), "Maximum size must be finite and at least 1 inch.");
        }
        
        double low = 1;
        double high = maxSize;

        var bestCandidate = createCandidate(high);

        var maxLoss = new FrictionLoss(bestCandidate).GetFrictionLoss();
        var maxVelocity = Airflow.VelocityFor(bestCandidate.Shape.Area);

        if (maxLoss > TargetFriction || maxVelocity > TargetVelocity)
        {
            throw new InvalidOperationException(
                $"No size between 1 and {maxSize} inches meets both limits.");
        }
        
        for (var i = 0; i < 100 && high - low > 0.0001; i++)
        {
            // Set our middle value
            var middle = (low + high) / 2.0;
            
            DivisionCount++;

            var candidate = createCandidate(middle);

            var loss = new FrictionLoss(candidate).GetFrictionLoss();
            var velocity = Airflow.VelocityFor(candidate.Shape.Area);

            var exceedsLimit = loss > TargetFriction || velocity > TargetVelocity;

            if (exceedsLimit)
            {
                low = middle;
            }
            else
            {
                high = middle; 
            }
        }

        var roundedSize = Math.Ceiling(high);

        if (roundedSize > maxSize)
        {
            throw new InvalidOperationException(
                "Rounding up would exceed the maxium size.");
        }

        return createCandidate(roundedSize);
    }

    public Duct NewRec(double width)
    {
        var validWidth = _formula.ValidateNumber(width, nameof(width));

        return NewDuct(height =>
            Duct.CreateRect(
                validWidth, height, Length, Airflow, Material));
    }
    
    public Duct NewRec2to1()
    {
        return NewDuct(height  =>
            Duct.CreateRect(
                height * 2, height, Length, Airflow, Material));
    }
    
    public Duct NewSquare()
    {
        return NewDuct(side =>
            Duct.CreateRect(
                 side, side, Length, Airflow, Material));
    }
    
    public Duct NewRound()
    {
        return NewDuct(diameter =>
            Duct.CreateRound(
                diameter, Length, Airflow, Material));
    }
    
    public Duct NewOval(double majorAxis)
    {
        var validMajorAxis = _formula.ValidateNumber(majorAxis, nameof(majorAxis));

        return NewDuct(minorAxis =>
            Duct.CreateOval(
                validMajorAxis, minorAxis, Length, Airflow, Material), maxSize: validMajorAxis);
    }
    
    public Duct NewOval2to1()
    {
        return NewDuct(minorAxis =>
            Duct.CreateOval(
                minorAxis * 2, minorAxis, Length, Airflow, Material));
    }


    public string Info(Duct duct)
    {
        var friction = new FrictionLoss(duct);
        
        return $"Size:              {duct.Shape.Size}\n" +
               $"CFM:               {duct.Airflow?.Cfm}\n" +
               $"Velocity:          {duct.Airflow?.VelocityFor(duct.Shape.Area):0.##}\n" +
               $"Static Loss/100ft: {friction.FrictionLossPer100Ft:0.##} in w.g.\n" +
               $"Divisions          {DivisionCount}";
    }
}