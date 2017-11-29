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
        private List<ILocatorResult> GetBMLocatorResults(LocatorSearchOptions searchOptions)
        {
            List<ILocatorResult> locatorResults = new List<ILocatorResult>();
            if ((searchOptions.LocatorResultType & LocatorResultTypes.Dealer) == LocatorResultTypes.Dealer)
            {
                locatorResults.AddRange(GetDealerLocatorResults(searchOptions));
            }
            if ((searchOptions.LocatorResultType & LocatorResultTypes.Installer) == LocatorResultTypes.Installer || 
                (searchOptions.LocatorResultType & LocatorResultTypes.Builder) == LocatorResultTypes.Builder)
            {
                locatorResults.AddRange(GetContractorLocatorResults(searchOptions));
            }
            return locatorResults;
        }
    }
}