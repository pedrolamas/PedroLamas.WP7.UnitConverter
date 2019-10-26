using System.Xml.Serialization;
using System.Xml;

namespace PedroLamas.WP7.UnitConverter.Models
{
    [XmlRoot("data")]
    public class UnitsData : IUnitsData
    {
        #region Properties

        [XmlElement("type", typeof(UnitType))]
        public IUnitType[] UnitTypes { get; set; }

        #endregion

        public static UnitsData Deserialize()
        {
            var xmlSerializer = new XmlSerializer(typeof(UnitsData));

            using (var inputStream = XmlReader.Create("Data.xml"))
            {
                return (UnitsData)xmlSerializer.Deserialize(inputStream);
            }
        }
    }
}