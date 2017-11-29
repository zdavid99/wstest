using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using OwensCorning.NotificationService;
using System.ServiceModel;

namespace com.ocwebservice
{
    /// <summary>
    /// Summary description for NotificationService
    /// </summary>
    [WebService(Namespace = "http://ws.owenscorning.com/")]
    [ServiceContract(Namespace="http://ws.owenscorning.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class NotificationService : System.Web.Services.WebService
    {
        [WebMethod]
        [OperationContract]
        public DocumentProcessResults Process(DocumentProcessOptions processingOptions)
        {
            try
            {
                DocumentService docService = new DocumentService();
                EmailService emailService = new EmailService();

                DocumentProcessResults results = docService.ProcessEktronDocuments(processingOptions);
                Batch batch = docService.GetLastBatchRun(processingOptions.Site);
                emailService.SendEmailToRecipients(processingOptions.Site, batch.batchId);

                return results;
            }
            catch (Exception e)
            {
                //TODO: Log Exception!
                //return false;
                return new DocumentProcessResults
                {
                    Status = "ERROR: " + e.ToString(),
                    StartDate = processingOptions.StartDate,
                    EndDate = processingOptions.EndDate
                };
            }
        }

        [WebMethod]
        [OperationContract]
        public BatchedDocumentResults GetDocumentsList(string site, int? batchId)
        {
            DocumentService docService = new DocumentService();
            return docService.GetDocumentsList(site, (batchId ?? -1));
        }


        [WebMethod]
        [OperationContract]
        public Boolean SubscribeWithName(string email, string site, string firstName, string lastName)
        {
            return SubscriptionService.Subscribe(email, site, firstName, lastName);
        }

        [WebMethod]
        [OperationContract]
        public Boolean Subscribe(string email, string site)
        {
            return SubscriptionService.Subscribe(email, site);
        }

        [WebMethod]
        [OperationContract]
        public Boolean Unsubscribe(string email, string site)
        {
            return SubscriptionService.Unsubscribe(email, site);
        }
    }
}
