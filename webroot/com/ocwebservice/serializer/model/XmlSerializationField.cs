using System;
using System.Xml.Serialization;

namespace OwensCorning.Serialization.Model
{
    [Serializable]
    [XmlRootAttribute(ElementName = "field")]
    public class XmlSerializationField
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }

        public override string ToString()
        {
            return "     Name: " + Name + Environment.NewLine + "     Value: " + Value;
        }
    }
}
