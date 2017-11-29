using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OwensCorning.ContactService.Data
{
	[Serializable]
    [XmlRoot("ContactForm")]
	public class ContactForm 
	{
		private int id;
        private DateTime _dateCreated;
        private string name;
        private string company;
        private string zip;
        private string phone;
        private string email;

        private string _formData;
        private string _businessArea;
        private string _sourceFormName;
        private string _sourcePath;
        private string _externalKey;
        private string _contactTypes;
        private DateTime _externalDate;
        private string _language;

		public ContactForm()
		{
			Id = -1;
			Name = "";
            Company = "";
            Zip = "";
			Phone = "";
			Email = "";
            ContactTypes = "";
            _formData = "";
		}

//        [XmlElement("id")]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

        public DateTime DateCreated
        {
            get { return _dateCreated; }
            set { _dateCreated = value; }
        }

//        [XmlElement("name")]
		public String Name
		{
			get { return name; }
			set { name = value; }
		}

//        [XmlElement("company")]
		public String Company
		{
			get { return company; }
			set { company = value; }
        }

//        [XmlElement("contacttype")]
        public String ContactTypes
        {
            get { return _contactTypes; }
            set { _contactTypes = value; }
        }

//        [XmlElement("zip")]
        public String Zip
        {
            get { return zip; }
            set { zip = value; }
        }

//        [XmlElement("phone")]
		public String Phone
		{
			get { return phone; }
			set { phone = value; }
		}

//        [XmlElement("email")]
		public String Email
		{
			get { return email; }
			set { email = value; }
		}


//        [XmlElement("businessarea")]
        public string BusinessArea
        {
            get { return _businessArea; }
            set { _businessArea = value; }
        }


//        [XmlElement("sourceformname")]
        public string SourceFormName
        {
            get { return _sourceFormName; }
            set { _sourceFormName = value; }
        }

//        [XmlElement("sourcepath")]
        public string SourcePath
        {
            get { return _sourcePath; }
            set { _sourcePath = value; }
        }

//        [XmlElement("externalkey")]
        public string ExternalKey
        {
            get { return _externalKey; }
            set { _externalKey = value; }
        }

//        [XmlElement("externaldate")]
        public DateTime ExternalDate
        {
            get { return _externalDate; }
            set { _externalDate = value; }
        }

//        [XmlElement("language")]
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        //[XmlIgnoreAttribute()]
        public string FormData
        {
            get { return _formData; }
            set { _formData = value; }
        }
    }
}
