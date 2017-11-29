using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class admin_AdminMenu : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
       wf_Logout.Visible =  Context.User.Identity.IsAuthenticated;
    }

    protected void wf_Logout_Click(object sender, EventArgs e)
    {
        FormsAuthentication.SignOut();

        Response.Redirect(Request.ApplicationPath);
 
    }
}
