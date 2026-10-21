using SheetmetalTools.Air;
using SheetmetalTools.Devices;
using SheetmetalTools.Ducts;
using SheetmetalTools.Formulas;
using SheetmetalTools.Materials;

namespace SheetmetalTools.RandomPrints;

public class RandomPrints00
{
    Converter conv = new Converter();

    private Airflow air = new Airflow(1800);
    private Material galvanized = Material.Galvanized;
    private List<Duct> ducts;
    private List<Louver> louvers;

    public RandomPrints00()
    {
        var rect = Duct.CreateRect(24, 12, 120, air, galvanized);
        var round = Duct.CreateRound(16, 120, air, galvanized);
        var oval = Duct.CreateOval(24, 12, 120, air, galvanized);
        
        ducts = [rect, round, oval];
        
        foreach (var duct in ducts)
        {
            duct.LeakTest = new DuctLeakTest(duct, "b", 2);
        }
        
        Louver l01 = Louver.CreateRect(24, 12, 50, air);
        Louver l02 = Louver.CreateRound(24, 50, air);
        Louver l03 = Louver.CreateOval(24, 12, 50, air);

        var l04 = LouverSizer.CreateRect(32, 5000, 800, 50);
        var l05 = LouverSizer.CreateSquare(5000, 800, 50);
        var l06 = LouverSizer.CreateRound(5000, 800, 50);
        var l07 = LouverSizer.CreateOval(5000, 800, 50);
        var l08 = LouverSizer.CreateOval(60, 5000, 800, 50);
        
        louvers = [l01, l02, l03, l04, l05, l06, l07, l08];

    }

    public void RunRandomLouvers00()
    {
        Console.WriteLine("\nSTART OF RANDOM RUN LOUVER 00\n");

        foreach (var louver in louvers)
        {
            Console.WriteLine($"{louver.Info()}\n");
        }
        
        Console.WriteLine("END OF RANDOM RUN LOUVER");
    }
    
    public void RunRandomDuct00()
    {
        Console.WriteLine("START OF RANDOM RUN DUCT 00\n");

        foreach (Duct duct in ducts)
        {
            Console.WriteLine($"{duct.Info()}\n");
        }
        
        Console.WriteLine("END OF RANDOM RUN DUCT");
    }
    
    public void RunRandomDuct01()
    {
        Console.WriteLine("START OF FRICTION LOSS TESTING\n");

        foreach (Duct duct in ducts)
        {
            FrictionLoss frictionLoss = new FrictionLoss(duct);
            
            Console.WriteLine($"{frictionLoss.Info()}\n");
        }
        
        Console.WriteLine("END OF FRICTION LOSS TESTING");
    }
    
    public void RunRandomDuct02()
    {
        Console.WriteLine("START OF DUCULATOR TESTING\n");

        var originalDuct = ducts[1];
        var calculator = new Duculator(originalDuct);
        var targerCalculator = new Duculator(2100, 0.08, 120, 800);

        List<Duct> replacements =
        [
            calculator.NewSquare(),
            calculator.NewRec2to1(),
            calculator.NewRound(),
            calculator.NewOval2to1()
        ];

        foreach (Duct replacement in replacements)
        {
            Console.WriteLine($"{calculator.Info(replacement)}\n");
        }

        var targetSquare = targerCalculator.NewSquare();
        Console.WriteLine(targerCalculator.Info(targetSquare));

        Console.WriteLine("END OF DUCULATOR TESTING");
    }
}
