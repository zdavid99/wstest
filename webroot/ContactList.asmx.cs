using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using OwensCorning.ContactService.Data;
using System.ServiceModel;

namespace com.ocwebservice
{
    /// <summary>
    /// Owens Corning Web Service to collect contact info (with possible extended parameters)
    /// </summary>
    [WebService(Namespace = "http://ws.owenscorning.com/")]
    [ServiceContract(Namespace="http://ws.owenscorning.com/")]
//    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class ContactList : System.Web.Services.WebService
    {

        /// <summary>
        /// Save the contact form as defined by OCContactForm class, which has
        /// standard fields such as fullName and phone number, but also includes
        /// user defined fields serialized into the FormData xml text field
        /// </summary>
        /// <param name="ocForm">Contact form that conforms to the IContactForm interace. In addition to basic fields such as company, name it should havea FormData field which returns the form XML serialized for all fields (basic and extedned)</param>
        /// <returns>status indicator if saved or not</returns>
        [WebMethod]
        [OperationContract]
        public Boolean SaveContactForm( ContactForm ocForm )
        {
            try {
                return ContactFormDao.Save(ocForm) >= 0;
            } catch (ApplicationException) {
                return false;
            }
        }

        /// <summary>
        /// Basic SaveContactForm allows to you save basic info, without the extended/other fields.
        /// The basic function will also serialize the standard fields into XML.
        /// </summary>
        /// <param name="business_area"></param>
        /// <param name="source_form_name"></param>
        /// <param name="source_path"></param>
        /// <param name="external_key"></param>
        /// <param name="external_date"></param>
        /// <param name="contact_type"></param>
        /// <param name="contact_fullname"></param>
        /// <param name="contact_email"></param>
        /// <param name="contact_phone"></param>
        /// <param name="company_name"></param>
        /// <param name="language"></param>
        /// <returns>status indicator if saved or not</returns>
        [WebMethod]
        [OperationContract]
        public Boolean SaveContactForm_Basic
            (
                string business_area,
                string source_form_name,
                string source_path,
                string external_key,
                string external_date,
                string contact_type,
                string contact_fullname,
                string contact_email,
                string contact_phone,
                string company_name,
                string language
            )
        {
            ContactForm ocForm = new ContactForm();
            DateTime extDate;   // temp to parse external_date

            ocForm.BusinessArea=  business_area;
            ocForm.SourceFormName = source_form_name;
            ocForm.SourcePath = source_path;
            ocForm.ExternalKey = external_key;

            if( DateTime.TryParse(external_date, out extDate) )
                ocForm.ExternalDate =  extDate;
            else 
                ocForm.ExternalDate =  (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

            ocForm.ContactTypes = contact_type;
            ocForm.Name = contact_fullname;
            ocForm.Email = contact_email;
            ocForm.Phone = contact_phone;
            ocForm.Company = company_name;
            ocForm.Language = language;

            ocForm.FormData = XMLSerializeBasicContactForm(ocForm);

            try {
                return ContactFormDao.Save(ocForm) >= 0;
            } catch (ApplicationException) {
                return false;
            }
        }

        /// <summary>
        /// Serializes the ContactForm paramter and returns a XML string
        /// </summary>
        /// <param name="ocForm"></param>
        /// <returns>xml string of ocForm</returns>
        public string XMLSerializeBasicContactForm( ContactForm ocForm)
        {
            StringBuilder sb = new StringBuilder();

            // Create Serializer
            XmlSerializer s = new XmlSerializer(typeof(ContactForm));

            // To NOT write the xmldoc statement
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.OmitXmlDeclaration = true;

            StringWriter stringWriter = new StringWriter();
            
            // To avoid xmlns on every node
            XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
            xmlnsEmpty.Add("", "");


            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, writerSettings)) {
                // Serialize all the ContactForm properties that are serializable (most have XmlElement attribute) XML StringBuilder 
                s.Serialize(xmlWriter, ocForm, xmlnsEmpty);
            }

            return stringWriter.ToString();
        }

        /// <summary>
        /// Just a stub that other websites can call to see if this service even exists
        /// </summary>
        /// <returns>boolean - and only thing checking is if running - so always true</returns>
        [WebMethod]
        [OperationContract]
        public bool GetStatus()
        {
            return true;
        }
    }
}
