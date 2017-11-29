using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;

using OwensCorning.Utility.Notification;

using OwensCorning.Serialization.Model;

namespace OwensCorning.Serialization.Service
{
    public class XmlSerializationService
    {
        private static XmlSerializationService self = new XmlSerializationService();

        private XmlSerializationService() { }

        public static XmlSerializationService Instance
        {
            get { return self; }
        }

        public XmlSerializationUser Deserialize(string xml)
        {
            StringReader reader = new StringReader(xml);
            XmlTextReader xmlReader = new XmlTextReader(reader);

            XmlSerializer deSerializer = new XmlSerializer(typeof(XmlSerializationUser));
            XmlSerializationUser deserialized = (XmlSerializationUser)deSerializer.Deserialize(xmlReader);

            reader.Close();
            xmlReader.Close();

            return deserialized;
        }

        public string Serialize(XmlSerializationUser user)
        {
            StringBuilder sb = new StringBuilder();
            XmlSerializer s = new XmlSerializer(typeof(XmlSerializationUser));

            // To NOT write the xmldoc statement
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.OmitXmlDeclaration = true;

            StringWriter stringWriter = new StringWriter();

            // To avoid xmlns on every node
            XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
            xmlnsEmpty.Add("", "");


            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, writerSettings))
            {
                s.Serialize(xmlWriter, user, xmlnsEmpty);
            }

            return stringWriter.ToString();
        }

        public string Transform(string xmlPath, string xsltPath)
        {
            XPathDocument document = new XPathDocument(xmlPath);
            XslCompiledTransform transform = new XslCompiledTransform();
            XmlWriterSettings writerSettings = new XmlWriterSettings();

            writerSettings.OmitXmlDeclaration = true;
            transform.Load(xsltPath);

            StringWriter stringWriter = new StringWriter();
            XmlWriter xmlWriter = XmlWriter.Create(stringWriter, writerSettings);

            transform.Transform(document, null, stringWriter);

            return stringWriter.ToString();
        }

        public void AddToExactTarget(XmlSerializationUser user)
        {
            ExactTargetUser etUser = new ExactTargetUser()
            {
                FirstName = GetField(user.BaseFields, "First Name"),
                LastName = GetField(user.BaseFields, "Last Name"),
                Email = GetField(user.BaseFields, "Email")
            };

            user.Lists.ListItems.ForEach(a => etUser.Lists.Add(new ExactTargetList() { Id = a.ID, Name = a.Name }));
            ExactTargetService.Instance.Register(etUser);

            ExactTargetService.Instance.AddDataExtensionRow(user.Extensions.Name,
                (from extension in user.Extensions.Fields
                 select new { Name = extension.Name, Value = extension.Value })
                .ToDictionary(a => a.Name, a => a.Value as object));
                 
        }

        private string GetField(XmlSerializationFieldList list, string fieldName)
        {
            XmlSerializationField serialzationField = list.Fields.Where(field => field.Name == fieldName).FirstOrDefault();
            return serialzationField == null ? "" : serialzationField.Value;
        }
    }
}
