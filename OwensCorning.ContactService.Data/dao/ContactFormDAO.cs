using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using OwensCorning.Utility.Logging;

namespace OwensCorning.ContactService.Data
{
    public class ContactFormDao : AbstractDao
    {
        private static ILogger log = LoggerFactory.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static ContactFormDao self = new ContactFormDao();

		private ContactFormDao()
		{
		}

		public static ContactFormDao Instance
		{
			get { return self; }
		}

		public static int Save(ContactForm form)
		{
            // Check required fields
            if( String.IsNullOrEmpty(form.SourceFormName)
                || String.IsNullOrEmpty(form.Name)
                || String.IsNullOrEmpty(form.Email)
                || String.IsNullOrEmpty(form.Phone)
                ) {
                return -1;
            }

            // Save the form
			form.Id = InsertItem(form);
			return form.Id;
		}

        private static int InsertItem(ContactForm form)
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

        private static void ValidateForm(ContactForm form)
        {
            //TODO: better validation
            if (form == null)
                throw new ArgumentNullException("form");

        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<ContactForm> GetAllContacts()
        {

            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {
                IList<ContactForm> contacts = GetAllContactFormsFromRepository(repository);
                return contacts;
            }
        }

        private static IList<ContactForm> GetAllContactFormsFromRepository(ContactInformationDataContext repository)
        {
            IList<ContactForm> contacts = (from contact in repository.oc_contactlists
                                           select BuildContactForm(contact)).ToList();
            return contacts;
        }

        

        public static IList<ContactForm> GetLast100Contacts()
        {
            // Just for convinience lets show last 100 contacts
            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {
                //
                IList<ContactForm> contacts = (from contact in repository.oc_contactlists
                                               orderby contact.date_created descending
                                               select BuildContactForm(contact)).Take(100).ToList();
                return contacts;
            }
        }


        public static IList<ContactForm> GetContactsByCriteria(string formName, DateTime startDate, DateTime endDate)
        {
            // Just for convinience lets show last 100 contacts
            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {
                
                var contactForms = (from contact in repository.oc_contactlists
                                                   where (String.IsNullOrEmpty(formName) || contact.source_form_name == formName)
                                                   &&
                                                   (DateTime.MinValue == startDate || contact.date_created >= startDate) &&
                                                   (DateTime.MaxValue == endDate || contact.date_created <= endDate)
                                                   orderby contact.date_created descending
                                                   select BuildContactForm(contact));
                return contactForms.ToList();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<ContactFormType> GetContactFormTypes()
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


		public static ContactForm GetContactFormById(int formId)
		{
            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {
                ContactForm form = (from contact in repository.oc_contactlists
                                    where contact.pk_id == formId
                                    select BuildContactForm(contact)).FirstOrDefault();
                return form;
            }
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification="used in linq to sql")]
        private static ContactForm BuildContactForm(oc_contactlist contact)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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
