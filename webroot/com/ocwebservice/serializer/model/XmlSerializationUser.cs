using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OwensCorning.Serialization.Model
{
    [Serializable]
    [XmlRootAttribute(ElementName = "user")]
    public class XmlSerializationUser
    {
        [XmlElement(ElementName = "basefields")]
        public XmlSerializationFieldList BaseFields { get; set; }

        [XmlElement(ElementName = "addlists")]
        public XmlSerializationList Lists { get; set; }

        [XmlElement(ElementName = "addextension")]
        public XmlSerializationFieldList Extensions { get; set; }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb
                .Append(BaseFields.Name).AppendLine(" Base Fields: ")
                .AppendLine(BaseFields.ToString())
                .AppendLine("Lists: ")
                .AppendLine(Lists == null ? "" : Lists.ToString())
                .Append(Extensions.Name).AppendLine(" Extension Fields: ")
                .AppendLine(Extensions.ToString())
                ;

            return sb.ToString();
        }
    }
}
