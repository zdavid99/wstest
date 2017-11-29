using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwensCorning.Utility.Logging;

namespace OwensCorning.NotificationService
{
    public class AdminHelper
    {
        private static ILogger log = LoggerFactory.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets all sites with available data.
        /// </summary>
        /// <returns>List of site names</returns>
        public static List<string> GetAllSitesWithAvailableData()
        {
            try
            {
                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    return dataContext.Batches.Select(batch => batch.site).Distinct().ToList();
                }
            }
            catch (Exception e)
            {
                log.Error("Unable to retrieve a list of available sites", e);
                return null;
            }
        }

        /// <summary>
        /// Gets the batches for site.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>List of Batch objects for this site</returns>
        public static List<Batch> GetBatchesForSite(string site)
        {
            try
            {
                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    return dataContext.Batches.Where(batch => batch.site == site).OrderByDescending(batch => batch.startDate).ToList();
                }
            }
            catch (Exception e)
            {
                log.Error("Unable to retrieve a list of batches for this site: " + site, e);
                return null;
            }
        }

        
    }
}
