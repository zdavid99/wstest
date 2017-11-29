using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.ocwebservice.locator.data;
using com.ocwebservice.locator.utility;
using OwensCorning.Locator.Data;
using com.ocwebservice.locator.model;

namespace com.ocwebservice.locator.dao
{
    public partial class LocatorDAO
    {
        private static int? TryParseInt(string s)
        {
            int i;
            return int.TryParse(s, out i) ? (int?)i : (int?)null;
        }

        public IEnumerable<ILocatorResult> GetDealerLocatorResults(LocatorSearchOptions searchOptions)
        {
            // early exits for currently unsupported conditions
            if (searchOptions.SearchType != OwensCorning.Locator.Data.SearchType.Advanced)
                throw new NotImplementedException();
            if (string.IsNullOrEmpty(searchOptions.Company))
                throw new NotImplementedException();

            using (DealerLocatorDataContext dataContext = new DealerLocatorDataContext())
            {
                var searchResults =
                    from contractor in dataContext.RawBMDealers
                    select contractor;

                if (!String.IsNullOrEmpty(searchOptions.Company))
                {
                    searchResults = searchResults.Where(result => result.ChainName.Contains(searchOptions.Company));
                    searchResults = searchResults.OrderBy(result => result.RecordID);
                }
                return searchResults.ToList().Where(r => TryParseInt(r.RecordID) != null).Select(a => BuildDealerLocatorResult(a) as ILocatorResult).ToList();
            }
        }

		public IEnumerable<ILocatorResult> GetDealerLocatorResultsV4(LocatorSearchOptions searchOptions)
		{
			// early exits for currently unsupported conditions
			if (searchOptions.SearchType != OwensCorning.Locator.Data.SearchType.Advanced)
				throw new NotImplementedException();
			if (string.IsNullOrEmpty(searchOptions.Company))
				throw new NotImplementedException();

			using (DealerLocatorDataContext dataContext = new DealerLocatorDataContext())
			{
				var searchResults =
					from contractor in dataContext.RawBMDealers
					select contractor;

				if (!String.IsNullOrEmpty(searchOptions.Company))
				{
					searchResults = searchResults.Where(result => result.ChainName.Contains(searchOptions.Company));
					searchResults = searchResults.OrderBy(result => result.RecordID);
				}
				return searchResults.ToList().Where(r => TryParseInt(r.RecordID) != null).Select(a => BuildDealerLocatorResultV4(a) as ILocatorResult).ToList();
			}
		}

		private static DealerV3 BuildDealerLocatorResult(RawBMDealer rawResult)
        {
			DealerV3 result = new DealerV3(LocatorBusinessTypes.Roofing);
            result.Id = int.Parse(rawResult.RecordID);
            result.Company = rawResult.StoreName;
            result.Address = rawResult.Address;
            result.City = rawResult.City;
            result.State = rawResult.State;
            result.Zip = rawResult.Zip;
            result.Country = rawResult.Country;
            result.Phone = rawResult.Phone;
            result.Website = rawResult.WebSiteAddress;

            return result;
        }

		private static DealerV4 BuildDealerLocatorResultV4(RawBMDealer rawResult)
		{
			DealerV4 result = new DealerV4(LocatorBusinessTypes.Roofing);
			result.Id = int.Parse(rawResult.RecordID);
			result.Company = rawResult.StoreName;
			result.Address = rawResult.Address;
			result.City = rawResult.City;
			result.State = rawResult.State;
			result.Zip = rawResult.Zip;
			result.Country = rawResult.Country;
			result.Phone = rawResult.Phone;
			result.Website = rawResult.WebSiteAddress;

			result.StocksFiberGlassInsulation = rawResult.StocksFiberGlassInsulation;
			result.StocksFoamInsulation = rawResult.StocksFoamInsulation;
			result.StocksVinylSiding = rawResult.StocksVinylSiding;
			result.StocksRoofing = rawResult.StocksRoofing;
			result.StocksHousewrap = rawResult.StocksHousewrap;
			result.StocksAttiCat = rawResult.StocksAttiCat;

			return result;
		}
    }
}