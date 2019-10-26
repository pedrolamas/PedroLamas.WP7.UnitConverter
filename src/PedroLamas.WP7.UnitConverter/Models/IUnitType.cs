namespace PedroLamas.WP7.UnitConverter.Models
{
    public interface IUnitType
    {
        string Name { get; }

        IUnit[] Units { get; }
    }
}