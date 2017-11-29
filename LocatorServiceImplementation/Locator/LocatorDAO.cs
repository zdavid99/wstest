using System;
using System.Collections.Generic;
using System.Linq;
using OwensCorning.Utility.LocatorServices;
using OwensCorning.Utility.Logging;
using OwensCorning.Utility.Status;
using System.Text.RegularExpressions;

namespace OwensCorning.Locator.Data
{
	public class LocatorDAO : IStatusMonitoringDAO
	{
		private static readonly ILogger log = 
            LoggerFactory.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly LocatorDAO self = new LocatorDAO();

        private LocatorDAO()
		{
		}

        public static LocatorDAO Instance
		{
			get { return self; }
		}

        public Contractor GetContractorWithForeignId(int foreignId)
        {
            using (LocatorClient client = new LocatorClient())
            {
                Contractor result = new Contractor(LocatorBusinessTypes.None);
                try
                {
                    result = client.GetContractorWithForeignId(foreignId.ToString());
                }
                catch (Exception ex)
                {
                    log.Error("Error finding contractor for id: " + foreignId, ex);
                }
                return result;
            }
        }

        public Contractor GetContractorWithEmail(string email)
        {
            using (LocatorClient client = new LocatorClient())
            {
                Contractor result = new Contractor(LocatorBusinessTypes.None);
                try
                {
                    result = client.GetContractorWithEmail(email);
                }
                catch (Exception ex)
                {
                    log.Error("Error finding contractor for email: " + email, ex);
                }
                return result;
            }
        }

        public List<ILocatorResult> GetLocatorResults(LocatorSearchOptions searchOptions)
        {
            using (LocatorClient client = new LocatorClient())
            {
                List<ILocatorResult> results = new List<ILocatorResult>();
                try
                {
                    results =
                        client.GetLocatorResults(searchOptions)
                        .ToList().ConvertAll(lr => lr as ILocatorResult);
                }
                catch (Exception ex)
                {
                    log.Error("Error getting locator results for search: " + searchOptions.ToString(), ex);
                }
                return results;
            }
        }

        public string LocatorHostName
        {
            get
            {
                using (LocatorClient client = new LocatorClient())
                {
                    return client.Endpoint.Address.Uri.Host;
                }
            }
        }

        #region IStatusMonitoringDAO Members

        public int RecordCountTest
        {
            get
            {
                int count = 0;
                try
                {
                    LocatorSearchOptions searchOptions =
                        new LocatorSearchOptions
                        {
                            LocatorBusinessType = (LocatorBusinessTypes.Masonry | LocatorBusinessTypes.Roofing),
                            LocatorResultType = LocatorResultTypes.Installer,
                            Location = "43537",
                            Radius = 25,
                            RequireEmailAddress = true,
                            MaxResultsPerType = 1
                        };

                    count = GetLocatorResults(searchOptions).Count;
                }
                catch (Exception ex)
                {
                    log.Fatal("Monitoring: " + Name + " failed RecordCountTest.", ex);
                }
                return count;
            }
        }

        public string Name
        {
            get { return GetType().ToString(); }
        }

        public bool IsPass
        {
            get { return (RecordCountTest > 0); }
        }
        #endregion
	}
}
