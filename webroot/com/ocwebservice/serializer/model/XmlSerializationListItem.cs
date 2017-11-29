using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OwensCorning.Serialization.Model
{
    [Serializable]
    [XmlRootAttribute(ElementName = "list")]
    public class XmlSerializationListItem
    {
        [XmlAttribute(AttributeName = "id")]
        public string ID { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return new StringBuilder().Append("     Name: ").AppendLine(Name).ToString();
        }
    }
}
