using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using OwensCorning.Utility.Logging;



/// <summary>
/// Summary description for _main
/// </summary>
public partial class admin : MasterPage
{
	private static ILogger log = LoggerFactory.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private string _appPath;
    public string AppPath
    {
        get { return _appPath; }
    }

	protected void Page_Load(object sender, EventArgs e) 
	{
        // Hard code application path
        if (Request.ApplicationPath != "/")
            _appPath = Request.ApplicationPath;
        else
            _appPath = "";

		if (!Page.IsPostBack)
		{
			// copyright information
			//lCopyrightDate.Text = DateTime.Now.Year.ToString();
		}


		return;
    }

}
