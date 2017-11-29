using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OwensCorning.Locator.Data;
using com.ocwebservice.locator.dao;

namespace com.ocwebservice.locator.service
{
    public class LocatorService : OwensCorning.Utility.Services.AbstractService
    {
        private static LocatorService self = new LocatorService();
		public static LocatorService Instance
		{
			get { return self; }
		}
        private LocatorService()
		{
		}

        public IEnumerable<ILocatorResult> GetDealerLocatorResults(LocatorSearchOptions searchOptions)
        {
            return ObtainCacheableData<IEnumerable<ILocatorResult>, LocatorSearchOptions>(searchOptions.ToString(), "cache.locator.duration", System.Web.Caching.CacheItemPriority.Normal, LoadDealerLocatorResults, searchOptions);
        }

        private IEnumerable<ILocatorResult> LoadDealerLocatorResults(LocatorSearchOptions searchOptions)
        {
            return LocatorDAO.Instance.GetDealerLocatorResults(searchOptions);
		}

		#region V4

		public IEnumerable<ILocatorResult> GetDealerLocatorResultsV4(LocatorSearchOptions searchOptions)
		{
			return ObtainCacheableData<IEnumerable<ILocatorResult>, LocatorSearchOptions>("V4." + searchOptions.ToString(), "cache.locator.duration", System.Web.Caching.CacheItemPriority.Normal, LoadDealerLocatorResultsV4, searchOptions);
		}

		private IEnumerable<ILocatorResult> LoadDealerLocatorResultsV4(LocatorSearchOptions searchOptions)
		{
			return LocatorDAO.Instance.GetDealerLocatorResultsV4(searchOptions);
		}

		#endregion
	}
}
