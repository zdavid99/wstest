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
using OwensCorning.SurveyService.Data;

public partial class survey_summary : System.Web.UI.Page
{
    const string ALLFORMS_DESC = "[ALL SURVEYS]";
    const string NOSURVEYS = "No surveys found matching that criteria";
    private string PresentQuestionName { get; set; }
    private List<AggregateResult> Responses { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        List<string> surveyNames;

        if (!IsPostBack)
            BindData();

        surveyNames = SurveyDAO.Instance.GetSurveyNames(GetValidStartDate(wf_StartingFrom), GetValidEndDate(wf_EndingOn));

        if (surveyNames.Count == 0)
        {
            wf_Status.Text = NOSURVEYS;
            wf_Status.Visible = true;
        }
        else
        {
            wf_Status.Text = "";
            wf_Status.Visible = false;

            wf_Surveys.DataSource = surveyNames;
            wf_Surveys.ItemDataBound += new RepeaterItemEventHandler(wf_Surveys_ItemDataBound);
            wf_Surveys.DataBind();
        }
    }

    void wf_Surveys_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
		RepeaterItem rItem = e.Item;

        if ((rItem.ItemType == ListItemType.Item) || (rItem.ItemType == ListItemType.AlternatingItem))
        {
            string surveyName = rItem.DataItem.ToString();

            Responses = SurveyDAO.Instance.GetSurveySummary(surveyName, GetValidStartDate(wf_StartingFrom), GetValidEndDate(wf_EndingOn));

            List<string> questionNames = (from questionName
                                          in Responses
                                          select questionName.QuestionName).
                                          Distinct<string>().ToList<string>();

            if (Responses == null || Responses.Count == 0)
            {
                rItem.Visible = false;
            }
            else
            {
                Repeater questions = (Repeater)rItem.FindControl("wf_Questions");

                questions.DataSource = questionNames;
                questions.ItemDataBound += new RepeaterItemEventHandler(wf_Questions_ItemDataBound);
                questions.DataBind();
            }
        }
    }

    void wf_Questions_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
		RepeaterItem rItem = e.Item;

        if ((rItem.ItemType == ListItemType.Item) || (rItem.ItemType == ListItemType.AlternatingItem))
        {
            string questionName = rItem.DataItem.ToString();
            GridView responses = (GridView)rItem.FindControl("wf_Responses");

            responses.DataSource = (from response
                                    in Responses
                                    where response.QuestionName == questionName
                                    select response).ToList<AggregateResult>();
            responses.DataBind();
        }
    }

    private void BindData()
    {
        // lets prepopulate starting date to 7 days ago
        wf_StartingFrom.Text = DateTime.Now.AddDays(-7).ToShortDateString();

        // Populate Form Types so that admin can filter down what site/form
        wf_FormTypes.DataSource = SurveyDAO.Instance.GetAllSurveyNames();
        wf_FormTypes.DataBind();
        wf_FormTypes.Items.Insert(0, ALLFORMS_DESC);
    }

    protected void wf_ViewData_Click(object sender, EventArgs e)
    {
        if (sender.GetType() == typeof(LinkButton))
            wf_FormTypes.SelectedValue = ((LinkButton)sender).Text;


        if (wf_FormTypes.SelectedItem.Text != ALLFORMS_DESC)
        {
            wf_Surveys.Visible = false;
            wf_SurveyDetails.Visible = true;
            wf_DownloadExcel.Visible = true;

            wf_SurveyDetails.DataSource = GetRequestedSurveys();
            wf_SurveyDetails.DataBind();
            if (wf_SurveyDetails.DataSource == null) return;

            if (wf_SurveyDetails.Rows.Count <= 0)
            {
                wf_Status.Text = NOSURVEYS;
                wf_Status.Visible = true;
            }
            else
            {
                wf_Status.Text = "";
                wf_Status.Visible = false;
            }
        }
        else
        {
            wf_Surveys.Visible = true;
            wf_SurveyDetails.Visible = false;
            wf_Status.Text = "";
            wf_Status.Visible = false;
            wf_DownloadExcel.Visible = false;
        }
    }

    protected void wf_DownloadExcel_Click(object sender, EventArgs e)
    {
        if (wf_FormTypes.SelectedItem.Text == ALLFORMS_DESC)
        {
            wf_Status.Text = "Please choose a survey.";
            wf_Status.Visible = true;
        }
        else
        {
            wf_Status.Text = "";
            wf_Status.Visible = false;

            DataGrid details = new DataGrid();

            details.DataSource = GetRequestedSurveys();
            details.DataBind();
            if (details.DataSource == null) return;

            DownloadFile(details);
        }
    }

    private DataTable GetRequestedSurveys()
    {
        string surveyName;
        DateTime? startingDate;
        DateTime? endingDate;
        try
        {
            // Check FormType
            surveyName = GetValidSurveyName(wf_FormTypes.SelectedValue);

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


            UpdatePreviewDescription(surveyName, startingDate, endingDate);

            return SurveyDAO.Instance.GetSurveyDetails(surveyName, startingDate, endingDate);
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


    private void UpdatePreviewDescription(string formType, DateTime? startingDate, DateTime? endingDate)
    {
        // Set the description of what we're previewing
        if (formType != string.Empty)
            wf_PreviewDesc.Text = "SURVEY (" + formType + ") - ";

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
    private string GetValidSurveyName(string surveyNameCandidate)
    {
        if (surveyNameCandidate.Length < 1 ||
            surveyNameCandidate == ALLFORMS_DESC)
        {
            return string.Empty;
        }
        return surveyNameCandidate;
    }

    private void DownloadFile(DataGrid grid)
    {
        string fileName = "oc_surveys_"
            + DateTime.Now.Year.ToString()
            + DateTime.Now.Month.ToString("00")
            + DateTime.Now.Day.ToString("00") + "-"
            + DateTime.Now.Hour.ToString("00")
            + DateTime.Now.Minute.ToString("00") + ".xls";

        StringBuilder sb = new StringBuilder();
        System.IO.StringWriter tw = new System.IO.StringWriter(sb);
        HtmlTextWriter hw = new HtmlTextWriter(tw);
        grid.RenderControl(hw);

        Response.Clear();
        Response.Charset = "";
        this.EnableViewState = false;
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
        Response.BufferOutput = true;
        Response.Write(sb.ToString());
        Response.End();
    }
}
