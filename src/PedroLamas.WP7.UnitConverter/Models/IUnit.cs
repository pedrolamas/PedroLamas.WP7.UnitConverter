namespace PedroLamas.WP7.UnitConverter.Models
{
    public interface IUnit
    {
        string Name { get; }

        double RelativeFactor { get; }

        double RelativeDisplacement { get; }

        string Symbol { get; }
    }
}