using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NotificationService;

namespace NotificationServiceAdmin
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<string> sites = AdminHelper.GetAllSitesWithAvailableData();
                sites.Insert(0, "--Please select one--");
                ddlSite.DataSource = sites;
                ddlSite.DataBind();
            }
        }

        protected void ddlSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlBatch.DataSource = AdminHelper.GetBatchesForSite(ddlSite.SelectedItem.Text);
            ddlBatch.DataTextField = "startDate";
            ddlBatch.DataValueField = "batchId";
            ddlBatch.DataBind();

            PopulateGrid();
        }

        protected void ddlBatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
            upDetails.Update();
        }

        protected void lbDocuments_Click(object sender, EventArgs e)
        {
            gvDocuments.Visible = true;
            gvSendTo.Visible = false;

            PopulateGrid();
        }

        protected void lbSentTo_Click(object sender, EventArgs e)
        {
            gvDocuments.Visible = false;
            gvSendTo.Visible = true;

            PopulateGrid();
        }

        private void PopulateGrid()
        {
            if (gvDocuments.Visible)
            {
                PopulateDocumentsGrid();
            }
            else
            {
                PopulateSendToGrid();
            }
        }

        private void PopulateDocumentsGrid()
        {
            DocumentService docService = new DocumentService();
            gvDocuments.DataSource = docService.GetDocumentsList(ddlSite.SelectedItem.Text, Convert.ToInt32(ddlBatch.SelectedValue));
            gvDocuments.DataBind();
        }

        private void PopulateSendToGrid()
        {
            EmailService emailService = new EmailService();
            List<SentInfo> SentToInfo = emailService.GetSentInfoBySiteAndBatchId(ddlSite.SelectedItem.Text, Convert.ToInt32(ddlBatch.SelectedValue));
            Cache.Insert("SentToInfo", SentToInfo, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 15, 0));
            gvSendTo.DataSource = SentToInfo;
            gvSendTo.DataBind();
        }

        protected void gvSendTo_Sorting(object sender, GridViewSortEventArgs e)
        {
            List<SentInfo> SentToInfo = Cache.Get("SentToInfo") as List<SentInfo>;

            if (SentToInfo == null)
            {
                EmailService emailService = new EmailService();
                SentToInfo = emailService.GetSentInfoBySiteAndBatchId(ddlSite.SelectedItem.Text, Convert.ToInt32(ddlBatch.SelectedValue));
            }

            if (e.SortExpression == "Subscription.firstName")
            {
                if (e.SortDirection == SortDirection.Ascending)
                {
                    SentToInfo = SentToInfo.OrderBy(info => info.Subscription.firstName).ToList();
                }
                else
                {
                    SentToInfo = SentToInfo.OrderByDescending(info => info.Subscription.firstName).ToList();
                }
            }
            else if (e.SortExpression == "Subscription.lastName")
            {
                if (e.SortDirection == SortDirection.Ascending)
                {
                    SentToInfo = SentToInfo.OrderBy(info => info.Subscription.lastName).ToList();
                }
                else
                {
                    SentToInfo = SentToInfo.OrderByDescending(info => info.Subscription.lastName).ToList();
                }
            }

            Cache.Insert("SentToInfo", SentToInfo, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 10, 0));
            gvSendTo.DataSource = SentToInfo;
            gvSendTo.DataBind();
        }

        protected void gvSendTo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Sort")
            {
                string email = gvSendTo.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();
                EmailService emailService = new EmailService();
                emailService.ResendEmail(ddlSite.SelectedItem.Text, email, Convert.ToInt32(ddlBatch.SelectedValue));
                PopulateGrid();
            }
        }
    }
}
