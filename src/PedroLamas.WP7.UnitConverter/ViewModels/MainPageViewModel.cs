using PedroLamas.WP7.UnitConverter.Framework;
using PedroLamas.WP7.UnitConverter.Models;

namespace PedroLamas.WP7.UnitConverter.ViewModels
{
    [SurviveTombstone]
    public class MainPageViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private IUnitsData _unitsData;

        public MainPageViewModel()
        {
            _unitsData = UnitsData.Deserialize();

            foreach (var unitType in _unitsData.UnitTypes)
                Items.Add(new UnitsTabViewModel(unitType));

            ActivateItem(Items[0]);
        }

        public void About()
        {
            PedroLamas.WP7.UnitConverter.Controls.About.Show();
        }
    }
}