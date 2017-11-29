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
using System.Collections.Generic;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.ComponentModel;
using System.Text.RegularExpressions;

using com.hansoninc.status;
using OwensCorning.Utility.Logging;
using OwensCorning.Utility.Status;
using OwensCorning.ContactService.Data;


    public partial class StatusView : Page
    {
        private static ILogger log = LoggerFactory.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static PerformanceCounter PerfUpTime = new PerformanceCounter("System", "System Up Time", true);

        protected const string TEST_PASSED = "Passed";
        protected const string TEST_FAILED = "Failed";
        protected const string CSS_STATUS_NORMAL = "statusnormal";
        protected const string CSS_STATUS_ERROR = "statuserror";
        protected const string CSS_STATUS_WARN = "statuswarning";
        protected const string CSS_PASS = "pass";
        protected const string CSS_FAIL = "fail";

        protected void Page_Load(object sender, EventArgs e)
        {
            bool isError = true;
            bool isWarning = true;

            try {
                Boolean environmentTest = EnvironmentErrorTest();
                Boolean databaseTest = DatabaseErrorTest();
                Boolean appConfigTest = ApplicationConfigurationTest();
                Boolean customErrorTest = CustomErrorTest();
                Boolean emailTest = false; // EmailErrorTest();

                isWarning = (appConfigTest || customErrorTest);
                isError = (environmentTest || databaseTest || emailTest);
            } catch (Exception ex) {
                log.Fatal("Status Page Error: " + ex.Message, ex);
                isError = true;
            }


            if (!isError) {
                LErrorsOnPage.Visible = false;
                LStatusOK.Visible = true;
                log.Info("Status is OK");
            } else {
                LErrorsOnPage.Visible = true;
                LStatusOK.Visible = false;
                log.Error("Errors on Page!");
            }

            if (!isWarning) {
                LWarningsOnPage.Visible = false;
                LWarningStatusOK.Visible = true;
                log.Info("No Warnings Detected");
            } else {
                LWarningsOnPage.Visible = true;
                LWarningStatusOK.Visible = false;
                log.Error("Warnings on Page.");
            }

            return;
        }

        private Boolean CustomErrorTest()
        {
            Boolean isError = false;

            try {
                Configuration config = WebConfigurationManager.OpenWebConfiguration("~/web.config");
                CustomErrorsSection section = (CustomErrorsSection)config.GetSection("system.web/customErrors");
                CustomErrorsMode mode = section.Mode;

                hDefaultRedirect.Text = section.DefaultRedirect;
                hDefaultRedirect.NavigateUrl = section.DefaultRedirect;
                LCustomErrorsMode.Text = mode.ToString();

                RCustomErrorCodes.DataSource = section.Errors;
                RCustomErrorCodes.DataBind();

                if (mode == CustomErrorsMode.Off) {
                    isError = true;
                }

            } catch (Exception ex) {
                isError = true;
                log.Error("Error checking custom error settings in web.config.", ex);
            }

            if (isError) {
                LCustomErrorStatus.Text = "Failed: Custom Errors are disabled in web.config, <br/>users may see unfriendly warnings or code details";
                LCustomErrorStatus.CssClass = CSS_STATUS_ERROR;
                LCustomErrorsMode.CssClass = CSS_STATUS_WARN;
            } else {
                LCustomErrorStatus.Text = "Passed";
                LCustomErrorStatus.CssClass = CSS_STATUS_NORMAL;
                LCustomErrorsMode.CssClass = CSS_PASS;
            }

            return isError;
        }


        private bool ApplicationConfigurationTest()
        {
            bool isError = false;
            try {
                if (Database.OwensCorning.IsProductionConnectionStringConfigured) {
                    lConnectionStringCheck.Text = "Pass";
                    lConnectionStringCheck.CssClass = CSS_PASS;
                } else {
                    isError = true;
                    lConnectionStringCheck.Text = "Non-production DB setting detected";
                    lConnectionStringCheck.CssClass = CSS_STATUS_WARN;
                }
            } catch (Exception ex) {
                isError = true;
                log.Error("Error checking application settings: " + ex.Message, ex);
            }

            if (isError) {
                LAppConfigurationStatus.Text = TEST_FAILED;
                LAppConfigurationStatus.CssClass = CSS_STATUS_ERROR;
            } else {
                LAppConfigurationStatus.Text = TEST_PASSED;
                LAppConfigurationStatus.CssClass = CSS_STATUS_NORMAL;
            }

            return isError;
        }

        private bool EnvironmentErrorTest()
        {
            bool isError = false;
            try {
                string dotNetVersion = Environment.Version.ToString();
                OperatingSystem os = Environment.OSVersion;

                LUpTime.Text = GetUpTime();
                LNetVersion.Text = dotNetVersion;
                LMachineName.Text = Environment.MachineName;
                LDateTime.Text = DateTime.Now.ToString();
                LOSVersion.Text = os.VersionString;

                if (dotNetVersion.StartsWith(ConfigurationManager.AppSettings["status.check.net.version.prefix"]) == false) {
                    isError = true;
                    LNetVersion.CssClass = CSS_FAIL;
                    log.Error("Incorrect .NET version: " + dotNetVersion);
                } else {
                    LNetVersion.CssClass = CSS_PASS;
                }
            } catch (Exception ex) {
                isError = true;
                log.Error("Error checking environment: " + ex.Message, ex);
            }

            if (isError) {
                LEnvironmentStatus.Text = TEST_FAILED;
                LEnvironmentStatus.CssClass = CSS_STATUS_ERROR;
            } else {
                LEnvironmentStatus.Text = TEST_PASSED;
                LEnvironmentStatus.CssClass = CSS_STATUS_NORMAL;
            }

            return isError;
        }

   
        private bool DatabaseErrorTest()
        {
            bool isError = false;

            IList<IStatusMonitoringDAO> daoList = null;

            try {
                daoList = StatusUtil.GetServiceList();

                foreach (IStatusMonitoringDAO dao in daoList) {
                    if (dao.IsPass == false) {
                        isError = true;
                        log.Error("DAO: " + dao.Name + " failed status check.");
                    }
                }

                RDAO.ItemCreated += new RepeaterItemEventHandler(RDAO_ItemCreated);
                RDAO.DataSource = daoList;
                RDAO.DataBind();
            } catch (Exception ex) {
                isError = true;
                log.Error("Error checking data access objects: " + ex.Message, ex);
            }

            if (isError) {
                LDatabaseStatus.Text = TEST_FAILED;
                LDatabaseStatus.CssClass = CSS_STATUS_ERROR;
            } else {
                LDatabaseStatus.Text = TEST_PASSED;
                LDatabaseStatus.CssClass = CSS_STATUS_NORMAL;
            }

            return isError;
        }

        private void RDAO_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem ritem = e.Item;
            if ((ritem.ItemType == ListItemType.Item) || (ritem.ItemType == ListItemType.AlternatingItem)) {
                IStatusMonitoringDAO dao = (IStatusMonitoringDAO)ritem.DataItem;
                Label txt = (Label)ritem.FindControl("TDaoPassFail");
                if (dao.IsPass) {
                    txt.Text = TEST_PASSED;
                    txt.CssClass = CSS_PASS;
                } else {
                    txt.Text = TEST_FAILED;
                    txt.CssClass = CSS_FAIL;
                }
            }

        }

        private string GetUpTime()
        {
            String upTime = null;
            try {
                //PerfUpTime.NextValue();
                TimeSpan ts = TimeSpan.FromSeconds(PerfUpTime.NextValue());

                upTime = ts.Days + " day(s), " + ts.Hours + " hours, " + ts.Minutes + " minutes, " + ts.Seconds + " seconds";
            } catch (Exception ex) {
                upTime = "Unable to determine uptime for this server.";
                log.Error("Error determining uptime: " + ex.Message, ex);
            }

            return upTime;
        }


    }
