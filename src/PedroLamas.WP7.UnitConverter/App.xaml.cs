namespace PedroLamas.WP7.UnitConverter
{
    using System.Windows;

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            UnhandledException += (s, e) =>
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            };
        }
    }
}