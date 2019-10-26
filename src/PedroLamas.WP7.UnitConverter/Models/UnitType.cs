using System.Xml.Serialization;

namespace PedroLamas.WP7.UnitConverter.Models
{
    public class UnitType : IUnitType
    {
        #region Properties

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("unit", typeof(Unit))]
        public IUnit[] Units { get; set; }

        #endregion
    }
}