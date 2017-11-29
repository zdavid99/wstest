using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwensCorning.Utility.Notification;
using System.Threading;
using System.Configuration;
using OwensCorning.Utility.Logging;
using System.Data.Linq;
using System.Net;
using System.IO;
using Amib.Threading;
using System.Diagnostics;

namespace OwensCorning.NotificationService
{
    public class EmailService
    {
        private static ILogger log = LoggerFactory.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string FromAddress
        {
            get { return ConfigurationManager.AppSettings["Email.From"]; }
        }

        private string EmailSubject
        {
            get;
            set;
        }

        private int maxThreads
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["Email.ThreadsMax"]); }
        }

        private class Email
        {
            public string to { get; set; }
            public string from { get; set; }
            public string body { get; set; }
            public string subject { get; set; }
        }

        /// <summary>
        /// Sends the email to recipients.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="documents">The updated documents.</param>
        public void SendEmailToRecipients(string site, int batchId)
        {
            try
            {
                List<Subscription> subscribers = SubscriptionService.GetCurrentSubscribers(site);
                if (null == subscribers || subscribers.Count == 0)
                {
                    log.Info("No Subscribers - email will not be sent");
                    return;
                }
                string emailTemplateURL = null;
                string emailSubject = null;
                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    SiteConfiguration siteConfiguration = dataContext.SiteConfigurations.FirstOrDefault(siteName => siteName.site == site);
                    emailTemplateURL = siteConfiguration.emailTemplateURL;
                    emailSubject = siteConfiguration.siteName + " " + (ConfigurationManager.AppSettings["Email.Subject"] as String);
                }

                string messageBody = GetEmailBody(emailTemplateURL + "?batchId=" + batchId);

                SmartThreadPool smartThreadPool = new SmartThreadPool();

                IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(maxThreads);

                foreach (Subscription subscription in subscribers)
                {
                    Email email = new Email
                    {
                        to = subscription.email,
                        from = FromAddress,
                        body = messageBody,
                        subject = emailSubject
                    };

                    PostExecuteWorkItemCallback afterEmailSend = delegate
                    {
                        SaveSentMailInformation(site, batchId, email);
                    };

                    WorkItemInfo workItem = new WorkItemInfo();
                    workItem.PostExecuteWorkItemCallback = afterEmailSend;
                    workItemsGroup.QueueWorkItem(workItem, SendMail, email);
                }

                workItemsGroup.WaitForIdle();

                smartThreadPool.Shutdown();

                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    Batch batch = dataContext.Batches.FirstOrDefault(b => b.site == site && b.batchId == batchId);

                    batch.finishDate = DateTime.Now;
                    batch.status = "Successful";

                    dataContext.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                log.Error("Unable to Send Email", e);
                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    Batch batch = dataContext.Batches.FirstOrDefault(b => b.site == site && b.batchId == batchId);

                    batch.status = "Unsuccessful";

                    dataContext.SubmitChanges();
                }
                throw e;
            }
        }

        private static void SaveSentMailInformation(string site, int batchId, Email email)
        {
            try
            {
                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    SentInfo sentTo = new SentInfo
                    {
                        batchId = batchId,
                        lastSendDate = DateTime.Now,
                        site = site,
                        status = "sent",
                        subscriptionEmail = email.to
                    };

                    dataContext.SentInfos.InsertOnSubmit(sentTo);
                    dataContext.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                LogException(site, batchId, email, e);
            }

        }

        private static void LogException(string site, int batchId, Email email, Exception e)
        {
            log.Error("Failed to insert the sent info item for: " + email.to, e);
            using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
            {
                SentInfo sentTo = dataContext.SentInfos.FirstOrDefault(sent => sent.batchId == batchId
                    && sent.subscriptionEmail == email.to
                    && sent.site == site);

                if (sentTo == null)
                {
                    sentTo = new SentInfo
                    {
                        batchId = batchId,
                        lastSendDate = DateTime.Now,
                        site = site,
                        status = "failed",
                        subscriptionEmail = email.to
                    };
                    dataContext.SentInfos.InsertOnSubmit(sentTo);
                }
                else
                {
                    sentTo.status = "failed";
                }

                dataContext.SubmitChanges();
            }
        }

        private object SendMail(object state)
        {
            Email email = (Email)state;

            try
            { 
                Debug.WriteLine(String.Format("ThreadId Context ID{0}, toAddress {1}, fromAddress {2}, messageBody {3}, subject {4}, Ticks {5}",
                    Thread.CurrentThread.ManagedThreadId, email.to, email.from, email.body.Length, email.subject, DateTime.Now.Ticks));

                EmailSender.Instance.SendEmail(email.to, email.from, email.body, email.subject, false, true);

                return true;
            }
            catch (Exception e)
            {
                log.Error("Failed Email: " + email.to, e);

                return false;
            }


        }

        /// <summary>
        /// Gets the sent info by site and batch id.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="batchId">The batch id.</param>
        /// <returns>List of SentInfo objects</returns>
        public List<SentInfo> GetSentInfoBySiteAndBatchId(string site, int batchId)
        {
            try
            {
                using(NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    DataLoadOptions dataContextOptions = new DataLoadOptions();
                    dataContextOptions.LoadWith<SentInfo>(info => info.Subscription);

                    dataContext.LoadOptions = dataContextOptions;

                    List<SentInfo> sentInfo = dataContext.SentInfos.Where(info => info.site == site && info.batchId == batchId).ToList();

                    return sentInfo;
                }
            }
            catch (Exception e)
            {
                log.Error("Unable to retrieve list of SentInfo objects for site " + site + " and batchId " + batchId, e);
                return null;
            }
        }

        private string GetEmailBody(string urlToTemplate)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                byte[] buffer = new byte[8192];
                string body = null;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlToTemplate);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                string tempString = null;
                int count = 0;

                do
                {
                    // fill the buffer with data
                    count = responseStream.Read(buffer, 0, buffer.Length);

                    // make sure we read some data
                    if (count != 0)
                    {
                        // translate from bytes to ASCII text
                        tempString = Encoding.ASCII.GetString(buffer, 0, count);

                        // continue building the string
                        sb.Append(tempString);
                    }
                }
                while (count > 0); // any more data to read?

                body = GetCleanBody(sb.ToString());
                return body;
            }
            catch (Exception e)
            {
                log.Error("Unable to get the body of the email.", e);
                throw e;
            }
        }

        private string GetCleanBody(string body)
        {
            //Strip out everything between the <body> tags to return
            int bodyIndex = body.ToLower().IndexOf("<body>");
            body = body.Remove(0, bodyIndex + "<body>".Length);
            bodyIndex = body.ToLower().IndexOf("</body>");
            body = body.Remove(bodyIndex);
            return body;
        }

        /// <summary>
        /// Resends the email.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="email">The email.</param>
        /// <param name="batchId">The batch id.</param>
        /// <returns>true if successful / false if not</returns>
        public Boolean ResendEmail(string site, string email, int batchId)
        {
            try
            {                
                string emailTemplateURL = null;
                string emailSubject = null;
                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    SiteConfiguration siteConfiguration = dataContext.SiteConfigurations.FirstOrDefault(siteName => siteName.site == site);
                    emailTemplateURL = siteConfiguration.emailTemplateURL;
                    emailSubject = siteConfiguration.siteName + " " + (ConfigurationManager.AppSettings["Email.Subject"] as String);
                }

                string messageBody = GetEmailBody(emailTemplateURL + "?batchId=" + batchId);

                EmailSender.Instance.SendEmail(email, FromAddress, messageBody, emailSubject, false, true);

                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    SentInfo sentInfo = dataContext.SentInfos.FirstOrDefault(info => info.site == site && info.subscriptionEmail == email && info.batchId == batchId);

                    sentInfo.lastSendDate = DateTime.Now;
                    sentInfo.status = "resent";

                    dataContext.SubmitChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                log.Error("Unable to resend email from batch " + batchId + " for site " + site + " to email " + email, e);

                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    SentInfo sentInfo = dataContext.SentInfos.FirstOrDefault(info => info.site == site && info.subscriptionEmail == email && info.batchId == batchId);

                    sentInfo.status = "resend failed";

                    dataContext.SubmitChanges();
                }

                return false;
            }
        }
    }
}
