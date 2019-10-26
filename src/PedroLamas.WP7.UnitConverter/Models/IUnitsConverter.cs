using System.ComponentModel;

namespace PedroLamas.WP7.UnitConverter.Models
{
    public interface IUnitsConverter : INotifyPropertyChanged
    {
        IUnitType[] UnitTypes { get; }

        IUnitType UnitType { get; set; }

        IUnit UnitA { get; set; }

        double AmountA { get; set; }

        IUnit UnitB { get; set; }

        double AmountB { get; set; }
    }
}