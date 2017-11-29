using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using OwensCorning.Utility.Data;
using OwensCorning.Utility.Logging;

namespace OwensCorning.ContactService.Data
{
    public class ContactFormDao : AbstractDAO
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
                                               select BuildContactForm(contact, repository.GetDmaXmlFromFormData(contact.xml_form_data))).Take(100).ToList();
                return contacts;
            }
        }



        public static IList<ContactForm> GetLast100Contacts(IEnumerable<string> formNames)
        {
            // Just for convinience lets show last 100 contacts
            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {
                //
                IList<ContactForm> contacts = (from contact in repository.oc_contactlists
                                               where formNames.Contains(contact.source_form_name)
                                               orderby contact.date_created descending
                                               select BuildContactForm(contact, repository.GetDmaXmlFromFormData(contact.xml_form_data))).Take(100).ToList();
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
                                    select BuildContactForm(contact, repository.GetDmaXmlFromFormData(contact.xml_form_data)));
                return contactForms.ToList();
            }
        }

        public static IList<ContactForm> GetContactsByCriteria(IEnumerable<string> formNames, DateTime startDate, DateTime endDate)
        {
            return GetContactsByCriteria(null, formNames, startDate, endDate);
        }

        public static IList<ContactForm> GetContactsByCriteria(string businessArea, IEnumerable<string> formNames, DateTime startDate, DateTime endDate)
        {
            // Just for convinience lets show last 100 contacts
            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {
                var contactForms = (from contact in repository.oc_contactlists
                                    join map in repository.ContactListNameMaps
                                        on contact.source_form_name equals map.SourceFormName
                                    where (formNames.Where(x => x != string.Empty).Count() == 0 || formNames.Contains(map.Name))
                                    && (businessArea == null || businessArea == string.Empty || businessArea == map.BusinessName)
                                    &&
                                    (DateTime.MinValue == startDate || contact.date_created >= startDate) &&
                                    (DateTime.MaxValue == endDate || contact.date_created <= endDate)
                                    orderby contact.date_created descending
                                    select BuildContactForm(contact, repository.GetDmaXmlFromFormData(contact.xml_form_data)));
                return contactForms.ToList();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<ContactFormType> GetContactFormTypes()
        {
            return GetContactFormTypes(string.Empty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<ContactFormType> GetContactFormTypes(string businessArea)
        {
            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {
                var formTypes = (from contact in repository.oc_contactlists
                                 join map in repository.ContactListNameMaps
                                    on contact.source_form_name equals map.SourceFormName into tempMappings
                                 from mapping in tempMappings.DefaultIfEmpty() // allow nulls in the join. . .
                                 where string.IsNullOrEmpty(businessArea) ? true : // Only obtain items from the business area, if one is present. 
                                 mapping != null ? mapping.BusinessName == businessArea : // Attempt to check from the business name first. . .
                                 contact.business_area == businessArea
                                 select new ContactFormType() {
                                         Active = mapping != null ? mapping.Active : false,
                                         SourceFormName = mapping == null ? contact.source_form_name : mapping.Name,
										 Name = mapping.Name,
										 ColumnMap = mapping.ColumnMap,
										 BusinessName = mapping.BusinessName
                                     } ).Distinct().OrderByDescending(x => x.Active);
                return formTypes.ToList();
            }
        }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static ContactFormType GetContactFormType(string sourceFormName)
		{
			using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
			{
				var formTypes = (from mapping in repository.ContactListNameMaps
								 where string.Equals(mapping.SourceFormName, sourceFormName)
								 select new ContactFormType()
								 {
									 Active = mapping.Active,
									 SourceFormName = mapping.SourceFormName,
									 Name = mapping.Name,
									 ColumnMap = mapping.ColumnMap,
									 BusinessName = mapping.BusinessName
								 });
				return formTypes.SingleOrDefault();
			}
		}

        internal static IList<ContactFormBusinessArea> GetContactFormBusinessAreas()
        {
            using (ContactInformationDataContext repository = ContactDataSource.ContactDataContext())
            {

                var businessAreas = (from mapping in repository.ContactListNameMaps
                                     where mapping.BusinessName != string.Empty
                                     select mapping.BusinessName).Distinct();

                List<ContactFormBusinessArea> formBusinessAreaList = new List<ContactFormBusinessArea>();
                foreach (var businessArea in businessAreas)
                    formBusinessAreaList.Add(new ContactFormBusinessArea { BusinessArea = businessArea });

                return formBusinessAreaList;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "used in linq to sql")]
        private static ContactForm BuildContactForm(oc_contactlist contact)
        {
            return BuildContactForm(contact, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification="used in linq to sql")]
        private static ContactForm BuildContactForm(oc_contactlist contact, string dmaTable)
        {
            return new ContactForm
                                          {
                                              Id = contact.pk_id,
                                              DateCreated = contact.date_created,
                                              Name = contact.contact_fullname,
                                              Company = contact.company_name,
                                              Zip = dmaTable,
                                              Phone = contact.contact_phone,
                                              Email = contact.contact_email,
                                              ContactTypes = contact.contact_type,
                                              Language = contact.language,
                                              BusinessArea = contact.business_area,
                                              FormData = UpdateFormData(contact.xml_form_data, dmaTable),

                                              SourceFormName = contact.source_form_name,
                                              SourcePath = contact.source_form_path,
                                              ExternalKey = contact.external_key,
                                              ExternalDate = contact.external_date ?? DateTime.MinValue


                                          };
        }

        private static string UpdateFormData(string formData, string dmaTable)
        {
            Dictionary<string, XmlNode> dmaDictionary = new Dictionary<string, XmlNode>();
            XmlDocument xmlDmaTable = new XmlDocument();
            XmlDocument xmlFormData = new XmlDocument();
            XmlNodeList zipNodes;
            XmlNodeList dmaNodes;

            if (!string.IsNullOrEmpty(formData))
            {
                xmlFormData.LoadXml(CleanXmlData(formData));
                xmlDmaTable.LoadXml(CleanXmlData(dmaTable));

                zipNodes = xmlFormData.SelectNodes("//Zip");
                dmaNodes = xmlDmaTable.SelectNodes("//zipMatch");

                foreach (XmlNode node in dmaNodes)
                {
                    if (!dmaDictionary.ContainsKey(node.FirstChild.InnerText))
                    {
                        dmaDictionary.Add(node.FirstChild.InnerText, node.ChildNodes[1]);
                    }
                }

                foreach(XmlNode node in zipNodes)
                {
                    if (dmaDictionary.ContainsKey(node.InnerText))
                    {
                        XmlNode dmaNode = dmaDictionary[node.InnerText];
						if (dmaNode != null)
						{
							XmlElement newDmaNode = node.OwnerDocument.CreateElement(dmaNode.Name);
							newDmaNode.InnerText = dmaNode.InnerText;
							node.ParentNode.InsertAfter(newDmaNode, node);
						}
                    }
					else
					{
						XmlElement newDmaNode = node.OwnerDocument.CreateElement("dma");
						node.ParentNode.InsertAfter(newDmaNode, node);
					}
                }
            }

            return xmlFormData.InnerXml;
        }

        private static string CleanXmlData(string formData)
        {
			TextReader tr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(@"~/xmlreplacements.txt"));

			while (tr.Peek() > -1)
			{
				string replacement = tr.ReadLine();

				if (!String.IsNullOrEmpty(replacement) && replacement.Contains(','))
				{
					string[] splitStr = replacement.Split(',');

					formData = formData.Replace(splitStr[0], splitStr[1]);
				}
			}

			formData = Regex.Replace(formData, @"[^\u0000-\u007F]", string.Empty);
			formData = Regex.Replace(formData, @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]", string.Empty);

			return formData;
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
