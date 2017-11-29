using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using com.ocwebservice.model;
using log4net;
using OwensCorning.ContactService.Data;

namespace com.ocwebservice.dao
{
    public class ContactFormDAO : AbstractDAO
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static ContactFormDAO self = new ContactFormDAO();

		private ContactFormDAO()
		{
		}

		public static ContactFormDAO Instance
		{
			get { return self; }
		}

		public int Save(ContactForm form)
		{
            // Check required fields
            if( form.BusinessArea == string.Empty
                || form.SourceFormName == string.Empty
                || form.SourcePath == string.Empty
                || form.ExternalKey == string.Empty
                || form.ContactTypes == string.Empty
                || form.Name == string.Empty
                || form.Email == string.Empty
                || form.Phone == string.Empty
                || form.Company == string.Empty
                || form.Language == string.Empty
                ) {
                return -1;
            }

            // Save the form
			form.Id = InsertItem(form);
			return form.Id;
		}

        private int InsertItem(ContactForm form)
        {
            ValidateForm(form);
            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {
                oc_contactlist contact = new oc_contactlist
                   {
                       business_area = form.BusinessArea,
                       source_form_name = form.SourceFormName,
                       source_form_path = form.SourcePath,
                       external_key = form.ExternalKey,
                       external_date = form.ExternalDate,
                       contact_type = form.ContactTypes,
                       contact_fullname = form.Name,
                       contact_email = form.Email,
                       contact_phone = form.Phone,
                       company_name = form.Company,
                       language = form.Language,
                       xml_form_data = form.FormData,
                       date_created = DateTime.Now

                   };
                repository.oc_contactlists.InsertOnSubmit(contact);
                repository.SubmitChanges();
                return contact.pk_id;
            }

        }

        private void ValidateForm(ContactForm form)
        {
            //TODO:
            //throw new NotImplementedException();
        }


        public IList<ContactForm> GetAllContacts()
        {

            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {
                IList<ContactForm> contacts = GetAllContactFormsFromRepository(repository);
                return contacts;
            }
        }

        private IList<ContactForm> GetAllContactFormsFromRepository(ContactInformationDataContext repository)
        {
            IList<ContactForm> contacts = (from contact in repository.oc_contactlists
                                           select BuildContactForm(contact)).ToList();
            return contacts;
        }

        

        public IList<ContactForm> GetLast100Contacts()
        {
            // Just for convinience lets show last 100 contacts
            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {
                //
                IList<ContactForm> contacts = (from contact in repository.oc_contactlists
                                               select BuildContactForm(contact)).Take(100).ToList();
                return contacts;
            }
        }


        public IList<ContactForm> GetContactsByCriteria(string formName, DateTime startingFrom)
        {
            // Just for convinience lets show last 100 contacts
            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {
                //
                bool hasWhere = formName != string.Empty || startingFrom != DateTime.MinValue;
                IList<ContactForm> contactForms = (from contact in repository.oc_contactlists
                                                   where (String.IsNullOrEmpty(formName) || contact.source_form_name == formName)
                                                   &&
                                                   (DateTime.MinValue == startingFrom || contact.date_created > startingFrom)
                                                   orderby contact.date_created descending
                                                   select BuildContactForm(contact)).ToList();
                return contactForms;
            }
        }

        public IList<ContactFormType> GetContactFormTypes()
        {
            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {

                var formTypes = (from contact in repository.oc_contactlists
                                 select contact.source_form_name).Distinct();
                List<ContactFormType> formTypeList = new List<ContactFormType>();
                foreach (var formType in formTypes)
                {
                    formTypeList.Add(new ContactFormType { SourceFormName = formType });
                }
                return formTypeList;
            }
        }


		public ContactForm GetContactFormById(int formId)
		{
            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {
                ContactForm form = (from contact in repository.oc_contactlists
                                    where contact.pk_id == formId
                                    select BuildContactForm(contact)).FirstOrDefault();
                return form;
            }
		}

        private ContactForm BuildContactForm(oc_contactlist contact)
        {
            return new ContactForm
                                          {
                                              Id = contact.pk_id,
                                              DateCreated = contact.date_created,
                                              Name = contact.contact_fullname,
                                              Company = contact.company_name,
                                              //Zip = rdr.IsDBNull(rdr.GetOrdinal("zip")) ? "" : rdr.GetString(rdr.GetOrdinal("zip,
                                              Phone = contact.contact_phone,
                                              Email = contact.contact_email,
                                              ContactTypes = contact.contact_type,
                                              Language = contact.language,
                                              BusinessArea = contact.business_area,
                                              FormData = contact.xml_form_data,

                                              SourceFormName = contact.source_form_name,
                                              SourcePath = contact.source_form_path,
                                              ExternalKey = contact.external_key,
                                              ExternalDate = contact.external_date ?? DateTime.MinValue


                                          };
        }


        

        #region IStatusMonitoringDAO Members

        public override int RecordCountTest
        {
            get
            {
                int count = 0;
                try
                {
                    count = CountTable(Database.OwensCorning, "oc_contactlist");
				}
                catch (Exception ex)
                {
					log.Fatal("Monitoring: " + this.Name + " failed RecordCountTest.", ex);
                }
                return count;
            }
        }

        public override string Name
        {
            get { return this.GetType().ToString(); }
        }

        public override bool IsPass
        {
            get { return (RecordCountTest > 0); }
        }

        #endregion        

    }
}
