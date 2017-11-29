using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ScheduledNotificationKickoff
{
    class Program
    {


        static void Main(string[] args)
        {

            List<NotificationService.DocumentProcessOptions> documentProcessOptionsList = LoadDocumentProcessOptions(args);
          
            NotificationService.NotificationService service = new ScheduledNotificationKickoff.NotificationService.NotificationService();
            Boolean success = true;

            foreach (NotificationService.DocumentProcessOptions processingOptions in documentProcessOptionsList)
            {
                Console.WriteLine("Running " + processingOptions.Site);

                NotificationService.DocumentProcessResults results = service.Process(processingOptions);
                //Boolean individualSuccess = service.Process(site, pro);
                bool processFailed = (results.Status.ToLower().Contains("error"));
                success = success && !processFailed;

                //TODO: Log all of this information!!!!!!!!!!
                if (processFailed)
                    Console.WriteLine(processingOptions.Site + " failed.");
            }

            if (!success)
            {
                //TODO: Log Error here instead of this
                Console.ReadKey();
            }
        }

        private static List<ScheduledNotificationKickoff.NotificationService.DocumentProcessOptions> LoadDocumentProcessOptions(string[] args)
        {
            //args should point to a config file that can be loaded

            string site = (ConfigurationManager.AppSettings["notificationService.site"] as String) ?? "commercial.owenscorning.com";
            //TODO: this should come from configuration
            return new  List<NotificationService.DocumentProcessOptions> {
                new NotificationService.DocumentProcessOptions
                {
                    Site = site,
                    StartDate = DateTime.Now.AddDays(-360).Date,
                    EndDate = DateTime.Now.Date.AddDays(1).AddMinutes(-1),
                    FileTypes = new string[] { "pdf", "doc" },
                    TaxonomyName = null
                },
            };
        }
    }
}
