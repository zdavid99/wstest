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
using com.ocwebservice.contactform.service;

public partial class contact_list : System.Web.UI.Page
{
    const string ALLAREAS_DESC = "[ALL AREAS]";
    const string ALLFORMS_DESC = "[ALL FORMS]";

    protected void Page_Load(object sender, EventArgs e)
    {
        // reset and description status message
        wf_PreviewDesc.Text = string.Empty;
        wf_Status.Text = string.Empty;
        wf_Status.Visible = false;

        if (!IsPostBack)
            BindData();

        wf_BusinessTypes.SelectedIndexChanged += new EventHandler(wf_BusinessTypes_SelectedIndexChanged);
    }

    private void BindData()
    {
        // Populate Business Area drop down. . .
        wf_BusinessTypes.DataSource = ContactFormDao.GetContactFormBusinessAreas();
        wf_BusinessTypes.DataBind();
        wf_BusinessTypes.AutoPostBack = true;
        wf_BusinessTypes.Items.Insert(0, ALLAREAS_DESC);

        // Populate Form Types so that admin can filter down what site/form
        ResetFormTypes(ContactFormDao.GetContactFormTypes());

        // lets prepopulate starting date to 7 days ago
        wf_StartingFrom.Text = DateTime.Now.AddDays(-7).ToShortDateString();

        wf_Contacts.DataSource = ContactFormDao.GetLast100Contacts();
        wf_Contacts.DataBind();

        wf_PreviewDesc.Text = "Viewing Last 100 Contacts";
        wf_PreviewDesc.Visible = true;

    }

    void wf_BusinessTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        var businessArea = GetValidBusinessArea(wf_BusinessTypes.SelectedValue);
        IList<ContactFormType> formTypes;

        if (string.IsNullOrEmpty(businessArea)) // If 'All Areas' or an invalid value is selected, fill with all items. . . 
            formTypes = ContactFormDao.GetContactFormTypes();
        else // Fill with business Area items. . .
            formTypes = ContactFormDao.GetContactFormTypes(businessArea);

        ResetFormTypes(formTypes);
    }

    void ResetFormTypes(IList<ContactFormType> types)
    {
        wf_FormTypes.DataSource = types;
        wf_FormTypes.DataBind();
        wf_FormTypes.Items.Insert(0, ALLFORMS_DESC);
    }

    protected void wf_ViewData_Click(object sender, EventArgs e)
    {
        // show requested contact forms
        wf_Contacts.DataSource = GetRequestedContactForms();
        wf_Contacts.DataBind();
        if(wf_Contacts.DataSource == null) return; //

        if (wf_Contacts.Rows.Count <= 0) {
            wf_Status.Text = "No contacts found matching that criteria";
            //wf_Status.Visible = true;
        }
    }

    protected void wf_DownloadXLS_Click(object sender, EventArgs e)
    {
        ExportResults("xls");
    }

    protected void wf_DownloadCSV_Click(object sender, EventArgs e)
    {
        ExportResults("csv");
    }

    private void ExportResults(string format)
    {
        var service = new ContactFormService(); 

        // Write the file. . .
        var success = service.WriteContactListFileToResponse(GetValidFormType(wf_FormTypes.SelectedValue), GetValidBusinessArea(wf_BusinessTypes.SelectedValue), wf_EndingOn.Text, wf_StartingFrom.Text, null, format, Response);

        if (!success)
        {
            wf_Status.Visible = true;
            wf_Status.Text = "The was an error processing your export.";
        }
        else
            wf_Status.Visible = false;

        Response.End();
    }


    protected void wf_DownloadXML_Click(object sender, EventArgs e)
    {
        StringBuilder sb = new StringBuilder();

        IList<ContactForm> cforms = GetRequestedContactForms();
        if (cforms == null) return; //

        if (cforms.Count >= 0) {
            // Create XML Doc from contactForms - that Excel can open
            sb.Append("<ContactForms>");
            foreach (ContactForm cf in cforms) {
                sb.AppendLine();
                sb.Append(cf.FormData);
            }
            sb.AppendLine();
            sb.AppendLine("</ContactForms>");

            // Return string as a file            
            DownloadFile(sb.ToString());
            
        } else {
            wf_Status.Text = "No contacts found matching that criteria";
            wf_Status.Visible = true;
        }
    }

    private IList<ContactForm> GetRequestedContactForms()
    {
        string formType;
        DateTime? startingDate;
        DateTime? endingDate;
        try
        {
            var formTypes = new List<string>();

            // Grab the business area
            var businessArea = GetValidBusinessArea(wf_BusinessTypes.SelectedValue);
            
            // Check FormType
            formType = GetValidFormType(wf_FormTypes.SelectedValue);

            if (!string.IsNullOrEmpty(formType))
                formTypes.Add(formType);
            else
                formTypes.AddRange(ContactFormDao.GetContactFormTypes(businessArea).Select(x => x.SourceFormName));

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


            UpdatePreviewDescription(businessArea, formType, startingDate, endingDate);

            return ContactFormDao.GetContactsByCriteria(businessArea, formTypes, startingDate.Value, endingDate.Value);
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


    private void UpdatePreviewDescription(string businessArea, string formType, DateTime? startingDate, DateTime? endingDate)
    {
        // Set the description of what we're previewing
        if (formType != string.Empty)
            wf_PreviewDesc.Text = "FORM (" + formType + ") - ";
        else if (businessArea != string.Empty)
            wf_PreviewDesc.Text = string.Format("BUSINESS AREA ({0}) - ", businessArea);
        else
            wf_PreviewDesc.Text = string.Empty;

        StringBuilder previewDescriptionStringBuilder = new StringBuilder();
        if (startingDate.HasValue && startingDate.Value >= MinDateTime)
        {
            previewDescriptionStringBuilder.Append(" FROM (" + startingDate.Value.ToShortDateString() + ") ");
        }
        if (endingDate.HasValue && endingDate.Value <= MaxDateTime)
        {
            previewDescriptionStringBuilder.Append("TO (" + endingDate.Value.ToShortDateString() + ")");
        }

        wf_PreviewDesc.Text += previewDescriptionStringBuilder.ToString();
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

        if (endDate < MinDateTime||
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
        if (formTypeCandidate.Length < 1 ||
            formTypeCandidate == ALLFORMS_DESC)
        {
            return string.Empty;
        }
        return formTypeCandidate;
    }

    /// <summary>
    /// Gets a valid business area from the text 
    /// </summary>
    /// <param name="formTypeCandidate"></param>
    private string GetValidBusinessArea(string businessAreaCandidate)
    {
        if (businessAreaCandidate.Length < 1 ||
            businessAreaCandidate == ALLAREAS_DESC)
        {
            return string.Empty;
        }
        return businessAreaCandidate;
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
}
