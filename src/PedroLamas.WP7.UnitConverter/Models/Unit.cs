using System.Xml.Serialization;

namespace PedroLamas.WP7.UnitConverter.Models
{
    public class Unit : IUnit
    {
        #region Properties

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("factor")]
        public double RelativeFactor { get; set; }

        [XmlAttribute("displacement")]
        public double RelativeDisplacement { get; set; }

        [XmlAttribute("symbol")]
        public string Symbol { get; set; }

        #endregion
    }
}