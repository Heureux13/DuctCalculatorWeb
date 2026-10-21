using SheetmetalTools.Formulas;

namespace SheetmetalTools.Materials;

public class Material
{
    private Formula _formula = new Formula();
    public string Name { get; }
    public double Density { get; }
    public double Roughness { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Name of the material.</param>
    /// <param name="density">Weight in lbs / ft³.</param>
    /// <param name="roughness">friction of metal in relation to air in feet.</param>
    private Material(string name, double density, double roughness)
    {
        this.Name = name;
        this.Density = _formula.ValidateNumber(density, nameof(density));
        this.Roughness = _formula.ValidateNumber(roughness, nameof(roughness));
    }
    
    public static Material Galvanized     { get; } = new Material("Galvanized", 490, 0.0003);
    public static Material Aluminium      { get; } = new Material("Aluminium", 169, 0.00015);
    public static Material StainlessSteel { get; } = new Material("Stainless Steel", 499, 0.000002);
    public static Material CarbonSteel    { get; } = new Material("Carbon Steel", 490, 0.000295);
    public static Material Liner          { get; } = new Material("Liner", 1.44, 0.003);
    public static Material LinerRigid     { get; } = new Material("Liner Hardboard", 3.0, 0.003);

    public string Info()
    {
        return $"Name:         {Name}\n" +
               $"Density:      {Density} lbs/ft³\n" +
               $"Roughness:    {Roughness}/ft";
    }
}