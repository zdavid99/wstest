<%@ Application Language="C#" %>
<%@ Import Namespace="log4net" %>

<script runat="server">
    static protected log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
    void Application_Start(object sender, EventArgs e) 
    {
        System.AppDomain myDomain = System.AppDomain.CurrentDomain;
        log4net.Config.XmlConfigurator.Configure();

        log.Info ("##########################################################################################################################################################################################");
        log.Info ("Application_Start");
        log.Info ("");
        log.Info ("  Logging: ");
        log.Info ("  ================================");
        log.Debug("  DEBUG: " + log.IsDebugEnabled);
        log.Info ("  INFO:  " + log.IsInfoEnabled);
        log.Warn ("  WARN:  " + log.IsWarnEnabled);
        log.Error("  ERROR: " + log.IsErrorEnabled);
        log.Fatal("  FATAL: " + log.IsFatalEnabled);
        log.Info ("##########################################################################################################################################################################################");
    }
    
    void Application_Error(object sender, EventArgs e) 
    {
        // get exception
        Exception objErr = Server.GetLastError().GetBaseException();

        // build simple message
        string err = "Error Caught in Application_Error event. Error in: " + Request.Url.ToString();

        // log exception
        log.Fatal(err, objErr);
    }

    void Application_End(object sender, EventArgs e)
    {
        log.Info("##########################################################################################################################################################################################");
        log.Info("Application_End");
        log.Info("##########################################################################################################################################################################################");
    }

    void Session_Start(object sender, EventArgs e)
    {
    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
        if (Request.RawUrl.Contains("contact-list-report.aspx"))
        {
            HttpContext.Current.SkipAuthorization = true;
        }
        //else
        //{
        //    HttpContext.Current.SkipAuthorization = false;
        //}
    }

</script>
