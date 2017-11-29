using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OwensCorning.NotificationService;
using System.Configuration;

namespace NotificationServiceAdmin
{
    public partial class _Default : System.Web.UI.Page
    {
        private List<int> listExcludeFromNewBatch;
        private DocumentService docService;
        private EmailService emailService;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<string> sites = AdminHelper.GetAllSitesWithAvailableData();
                sites.Insert(0, "--Please select one--");
                ddlSite.DataSource = sites;
                ddlSite.DataBind();
            }

            docService = new DocumentService();
            emailService = new EmailService();
        }

        protected void ddlSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlBatch.DataSource = AdminHelper.GetBatchesForSite(ddlSite.SelectedItem.Text);
            ddlBatch.DataTextField = "endDate";
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
            pnlRunNewBatch.Visible = false;

            PopulateGrid();
        }

        protected void lbSentTo_Click(object sender, EventArgs e)
        {
            gvDocuments.Visible = false;
            gvSendTo.Visible = true;
            pnlRunNewBatch.Visible = false;

            PopulateGrid();
        }

        protected void lbRunNewBatch_Click(object sender, EventArgs e)
        {
            gvDocuments.Visible = false;
            gvSendTo.Visible = false;
            pnlRunNewBatch.Visible = true;

            PopulateGrid();
        }

        private void PopulateGrid()
        {
            if (ddlSite.SelectedIndex < 1)
                return;
            
            if (gvDocuments.Visible)
            {
                PopulateDocumentsGrid();
            }
            else if (gvSendTo.Visible)
            {
                PopulateSendToGrid();
            }
            else if (pnlRunNewBatch.Visible)
            {
                List<Batch> batches = AdminHelper.GetBatchesForSite(ddlSite.SelectedItem.Text);
                if (batches != null)
                {
                    lLastBatchRun.Text = "the last batch run date of " + batches[0].endDate.ToString("g");
                }
                else
                {
                    lLastBatchRun.Text = "the default time period of " + ConfigurationManager.AppSettings["DaysToCheckIfNoPreviousBatchRun"] + " days ago since there is no previous batch";
                }

                int numNewBatchDocuments = PopulateNewBatchDocumentsRepeater();
                pnlNoNewBatchDocuments.Visible = (numNewBatchDocuments == 0);
                pnlNewBatchDocumentsHead.Visible = (numNewBatchDocuments > 0);
                rNewBatchDocuments.Visible = (numNewBatchDocuments > 0);
                bRunBatch.Visible = (numNewBatchDocuments > 0);
            }
        }

        private void PopulateDocumentsGrid()
        {
            BatchedDocumentResults documentResults = docService.GetDocumentsList(ddlSite.SelectedItem.Text, Convert.ToInt32(ddlBatch.SelectedValue));
            gvDocuments.DataSource = documentResults.DocumentList;
            gvDocuments.DataBind();
        }
        
        private void PopulateSendToGrid()
        {
            
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
            else if (e.SortExpression == "subscriptionEmail")
            {
                if (e.SortDirection == SortDirection.Ascending)
                {
                    SentToInfo = SentToInfo.OrderBy(info => info.subscriptionEmail).ToList();
                }
                else
                {
                    SentToInfo = SentToInfo.OrderByDescending(info => info.subscriptionEmail).ToList();
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

        private DocumentProcessOptions CreateProcessingOptions()
        {
            string site = ddlSite.SelectedItem.Text;
            Batch batch = docService.GetLastBatchRun(site);
            DocumentProcessOptions processingOptions = new DocumentProcessOptions
            {
                Site = site,
                StartDate = batch.endDate,//.Date, //TODO: GetStartDate(),
                EndDate = DateTime.Now,//.Date, //TODO: GetEndDate(),
                FileTypes = new List<string> { "pdf", "doc" }, //TODO: GetFileTypes(),
                TaxonomyName = null, //TODO: GetTaxonomyName()
            };
            return processingOptions;
        }
        
        private int PopulateNewBatchDocumentsRepeater()
        {
            DocumentProcessOptions processingOptions = CreateProcessingOptions();
            List<UpdatedDoc> newBatchList = docService.GetUpdatedDocumentList(processingOptions);
            rNewBatchDocuments.DataSource = newBatchList;
            rNewBatchDocuments.ItemDataBound += new RepeaterItemEventHandler(rNewBatchDocuments_ItemDataBound);
            rNewBatchDocuments.DataBind();
            return newBatchList.Count;
        }

        void rNewBatchDocuments_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem ritem = e.Item;
            if ((ritem.ItemType == ListItemType.Item) || (ritem.ItemType == ListItemType.AlternatingItem))
            {
                UpdatedDoc result = (UpdatedDoc)ritem.DataItem;
                if (result != null)
                {
                    Literal lNewBatchDocumentName = (Literal)ritem.FindControl("lNewBatchDocumentName");
                    CheckBox cbIncludeInNewBatch = (CheckBox)ritem.FindControl("cbIncludeInNewBatch");

                    lNewBatchDocumentName.Text = result.documentName;
                    cbIncludeInNewBatch.Checked = true;
                }
            }
        }

        protected void bRunBatchSubmit_Click(object sender, EventArgs e)
        {
            listExcludeFromNewBatch = new List<int>();

            for (int i = rNewBatchDocuments.Items.Count - 1; i >= 0; i--)
            {
                RepeaterItem ritem = rNewBatchDocuments.Items[i];
                CheckBox cbIncludeInNewBatch = (CheckBox)ritem.FindControl("cbIncludeInNewBatch");
                if (!cbIncludeInNewBatch.Checked)
                    listExcludeFromNewBatch.Add(i);
            }

            if (rNewBatchDocuments.Items.Count == listExcludeFromNewBatch.Count)
                return;

            DocumentProcessOptions processingOptions = CreateProcessingOptions();
            DocumentProcessResults results = docService.ProcessEktronDocuments(processingOptions, listExcludeFromNewBatch);
            if (String.IsNullOrEmpty(results.Status) ||
                !results.Status.ToLower().Contains("error"))
            {
                emailService.SendEmailToRecipients(processingOptions.Site, results.BatchId);
            }
            else
            {
                //TODO: Log Error no Mail Sent
            }

            gvDocuments.Visible = true;
            gvSendTo.Visible = false;
            pnlRunNewBatch.Visible = false;

            ddlSite_SelectedIndexChanged(sender, e);
        }
    }
}
