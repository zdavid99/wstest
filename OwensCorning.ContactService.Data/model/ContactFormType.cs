using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OwensCorning.ContactService.Data
{

	public class ContactFormType 
	{
        private string _sourceFormName;
   
        public ContactFormType()
		{

        }

        public string SourceFormName
        {
            get { return _sourceFormName; }
            set { _sourceFormName = value; }
        }

    }
}
