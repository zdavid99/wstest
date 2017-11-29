using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;


using com.ocwebservice.model;
using OwensCorning.ContactService.Data;
using OwensCorning.Utility.Extensions;
using OwensCorning.Excel;
using System.Xml.Schema;
using System.Xml;
using System.IO;
using System.Data;
using com.ocwebservice.contactform.service;

namespace com.ocwebservice.admin
{
    public partial class contact_list_report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string formType = Request["formType"];
            string businessArea = Request["businessArea"];
            string endingDateValue = Request["endDate"];
            string startingDateValue = Request["startDate"];
            string contactType = Request["contactType"];
            string format = Request["format"];
            string columnList = Request["columnList"];
            string columnOrderList = Request["columnOrderList"];
			string columnMap = Request["columnMap"];

            if (string.IsNullOrEmpty(formType) && string.IsNullOrEmpty(businessArea))
            {
                wf_Status.Text = "No form type provided.";
                wf_Status.Visible = true;
            }
            else
            {
                var service = new ContactFormService();
				if (string.IsNullOrEmpty(columnMap))
				{
					service.WriteContactListFileToResponse(formType, businessArea, endingDateValue, startingDateValue, contactType, format, columnList, columnOrderList, Response);
				}
				else
				{
					service.WriteContactListFileToResponseColumnMap(formType, businessArea, endingDateValue, startingDateValue, contactType, format, columnMap, Response);
				}
                Response.End();
            }
        }
    }
}
