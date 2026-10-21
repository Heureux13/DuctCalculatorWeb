namespace SheetmetalTools.Shapes;

public abstract class Shape
{
    public abstract string Name {get;}
    public abstract string Size { get; }
    public abstract double Area {get;}
    public abstract double Perimeter {get;}
    
}