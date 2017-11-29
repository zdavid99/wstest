using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OwensCorning.Serialization.Model
{
    [Serializable]
    //[XmlRootAttribute(ElementName = "addextension")]
    public class XmlSerializationFieldList
    {
        public XmlSerializationFieldList()
        {
            Fields = new List<XmlSerializationField>();
        }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlIgnore]
        public bool NameSpecified { get { return !string.IsNullOrEmpty(Name); } }

        [XmlElement("field")]
        public List<XmlSerializationField> Fields;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (XmlSerializationField field in Fields)
                sb.AppendLine(field.ToString()).AppendLine(" ");

            return sb.ToString();
        }
    }
}
