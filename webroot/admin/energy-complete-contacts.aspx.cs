using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Linq;
using System.Data.Linq;


using com.ocwebservice.model;
using OwensCorning.ContactService.Data;

namespace com.ocwebservice.admin
{
    public partial class energy_complete_contacts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // reset and description status message
            wf_PreviewDesc.Text = string.Empty;
            wf_Status.Text = string.Empty;
            wf_Status.Visible = false;

            if (!IsPostBack)
                BindData();
        }

        private void BindData()
        {
            // lets prepopulate starting date to 7 days ago
            wf_StartingFrom.Text = DateTime.Now.AddDays(-7).ToShortDateString();



            wf_Contacts.DataSource = ContactFormDao.GetLast100Contacts(GetFormTypes());
            wf_Contacts.DataBind();

            wf_PreviewDesc.Text = "Viewing Last 100 Contacts";
            wf_PreviewDesc.Visible = true;

        }

        protected void wf_ViewData_Click(object sender, EventArgs e)
        {
            // show requested contact forms
            wf_Contacts.DataSource = GetRequestedContactForms();
            wf_Contacts.DataBind();
            if (wf_Contacts.DataSource == null) return; //

            if (wf_Contacts.Rows.Count <= 0)
            {
                wf_Status.Text = "No contacts found matching that criteria";
                //wf_Status.Visible = true;
            }
        }

        protected void wf_DownloadXML_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            IList<ContactForm> cforms = GetRequestedContactForms();
            if (cforms == null) return; //

            if (cforms.Count >= 0)
            {
                // Create XML Doc from contactForms - that Excel can open
                sb.Append("<ContactForms>");
                foreach (ContactForm cf in cforms)
                {
                    sb.AppendLine();
                    sb.Append(cf.FormData);
                }
                sb.AppendLine();
                sb.AppendLine("</ContactForms>");

                // Return string as a file            
                DownloadFile(sb.ToString());

            }
            else
            {
                wf_Status.Text = "No contacts found matching that criteria";
                wf_Status.Visible = true;
            }
        }

        private IList<ContactForm> GetRequestedContactForms()
        {
            List<string> formTypes = new List<string>();
            DateTime? startingDate;
            DateTime? endingDate;
            try
            {
                // Add FormTypes
                //formType = GetValidFormType(wf_FormTypes.SelectedValue);
                formTypes = GetFormTypes();

                // Check Starting Date
                startingDate = GetValidStartDate(wf_StartingFrom);
                if (!startingDate.HasValue)
                    return null;

                endingDate = GetValidEndDate(wf_EndingOn);
                if (!endingDate.HasValue)
                    return null;
                else
                {
                    if (endingDate < MaxDateTime.AddDays(-1))
                        endingDate = ((DateTime)endingDate).AddDays(1).AddMilliseconds(-1);
                }


                UpdatePreviewDescription(formTypes, startingDate, endingDate);

                return ContactFormDao.GetContactsByCriteria(formTypes, startingDate.Value, endingDate.Value);
            }
            catch (ArgumentOutOfRangeException outOfRangeException)
            {
                wf_Status.Text = outOfRangeException.Message;
                wf_Status.Visible = true;
                return null;
            }
            catch (ArgumentException ex)
            {
                wf_Status.Text = ex.Message;
                wf_Status.Visible = true;
                return null;
            }
        }


        private void UpdatePreviewDescription(List<string> formTypes, DateTime? startingDate, DateTime? endingDate)
        {
            StringBuilder previewDescriptionStringBuilder = new StringBuilder();
            if (startingDate.HasValue && startingDate.Value >= MinDateTime)
            {
                previewDescriptionStringBuilder.Append(" FROM (" + startingDate.Value.ToShortDateString() + ") ");
            }
            if (endingDate.HasValue && endingDate.Value <= MaxDateTime)
            {
                previewDescriptionStringBuilder.Append("TO (" + endingDate.Value.ToShortDateString() + ")");
            }

            wf_PreviewDesc.Text = previewDescriptionStringBuilder.ToString();
        }

        private static DateTime MinDateTime { get { return System.Data.SqlTypes.SqlDateTime.MinValue.Value; } }
        private static DateTime MaxDateTime { get { return System.Data.SqlTypes.SqlDateTime.MaxValue.Value; } }

        private DateTime? GetValidEndDate(TextBox endDateTextBox)
        {
            DateTime endDate = DateTime.Now;
            if (String.IsNullOrEmpty(endDateTextBox.Text))
            {
                endDate = MaxDateTime;
            }
            else if (!DateTime.TryParse(endDateTextBox.Text, out endDate))
            {
                throw new ArgumentException("End Date format is incorrect, please use MM/DD/YYYY format (ex 04/21/2009)");

            }

            if (endDate < MinDateTime ||
                endDate > MaxDateTime)
            {
                throw new ArgumentOutOfRangeException("Ending Date", String.Format("End Date format is incorrect, it must be between %0 and %1", MinDateTime.ToString(), MaxDateTime.ToString()));
            }

            return endDate;
        }

        private DateTime? GetValidStartDate(TextBox startDateTextBox)
        {
            DateTime startDate = DateTime.MinValue;
            if (String.IsNullOrEmpty(startDateTextBox.Text))
            {
                return startDate;
            }

            if (!DateTime.TryParse(startDateTextBox.Text, out startDate))
            {
                throw new ArgumentException("Starting Date format is incorrect, please use MM/DD/YYYY format (ex 08/22/2008)");

            }

            if (startDate <= MinDateTime ||
                startDate >= MaxDateTime)
            {
                throw new ArgumentOutOfRangeException("StartingDate", String.Format("Starting Date format is incorrect, it must be between %0 and %1", MinDateTime.ToString(), MaxDateTime.ToString()));
            }

            return startDate;

        }

        /// <summary>
        /// Gets a valid form type from the text 
        /// </summary>
        /// <param name="formTypeCandidate"></param>
        /// <returns>Empty string if the formType is 1 charachter or 'All forms'</returns>
        private string GetValidFormType(string formTypeCandidate)
        {
            if (formTypeCandidate.Length < 1 )
            {
                return string.Empty;
            }
            return formTypeCandidate;
        }

        private void DownloadFile(string textContent)
        {
            string fileName = "oc_contacts_"
                + DateTime.Now.Year.ToString()
                + DateTime.Now.Month.ToString("00")
                + DateTime.Now.Day.ToString("00") + "-"
                + DateTime.Now.Hour.ToString("00")
                + DateTime.Now.Minute.ToString("00") + ".xml";

            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.ContentType = "text/xml";
            Response.BufferOutput = true;
            Response.Write(textContent);
            Response.End();
        }

        private List<string> GetFormTypes()
        {
            List<string> formTypes = new List<string>();

            formTypes.Add("Energy Complete Contractor Contact Form");
            formTypes.Add("Energy Complete Homeowner Contact Form");
            formTypes.Add("Energy Complete Builder Contact Form");
            formTypes.Add("Energy Complete Architect Contact Form");

            return formTypes;
        }
    }
}
