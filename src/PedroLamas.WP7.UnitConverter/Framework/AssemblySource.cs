namespace PedroLamas.WP7.UnitConverter.Framework
{
    using System.Reflection;

    /// <summary>
    /// A source of assemblies that are inspectable by the framework.
    /// </summary>
    public static class AssemblySource
    {
        /// <summary>
        /// The singleton instance of the AssemblySource used by the framework.
        /// </summary>
        public static readonly IObservableCollection<Assembly> Instance = new BindableCollection<Assembly>();
    }
}
