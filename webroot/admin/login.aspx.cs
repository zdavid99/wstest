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

public partial class admin_login : System.Web.UI.Page
{
    //protected System.Web.UI.WebControls.TextBox wf_Username;
    //protected System.Web.UI.WebControls.TextBox wf_Password;
    //protected System.Web.UI.WebControls.Label wf_Status;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Login_Click(object sender, EventArgs e)
    {
        if (FormsAuthentication.Authenticate(wf_Username.Text, wf_Password.Text))
            FormsAuthentication.RedirectFromLoginPage(wf_Username.Text, true);
        else {
            wf_Status.Text += "Invalid Login";
            wf_Status.Visible = true;
        }
    }
}
