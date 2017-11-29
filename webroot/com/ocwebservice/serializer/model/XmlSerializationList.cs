using System;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OwensCorning.Serialization.Model
{
    [Serializable]
    //[XmlRootAttribute(ElementName = "addlists")]
    public class XmlSerializationList
    {
        public XmlSerializationList()
        {
            ListItems = new List<XmlSerializationListItem>();
        }

        [XmlElement("list")]
        public List<XmlSerializationListItem> ListItems { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (XmlSerializationListItem listItem in ListItems)
                sb.Append(listItem.ToString());

            return sb.ToString();
        }
    }
}
