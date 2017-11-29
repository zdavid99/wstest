using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OwensCorning.Locator.Data
{
    /// <summary>
    /// The type of locator search to perform, using latitude/longitude, postal code, or keywords.
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// Uses latitude and longitude to find locator results in a given radius
        /// </summary>
        Map,
        /// <summary>
        /// Uses a postal code to find locator results in a given radius
        /// </summary>
        Zip,
        /// <summary>
        /// Searches for locator results based on keywords.
        /// </summary>
        Advanced
    }

    /// <summary>
    /// Specifies a search for a group of dealers, builders, or contractors that
    /// use or supply Owens Corning products.
    /// </summary>
    public class LocatorSearchOptions
    {
        public LocatorSearchOptions()
        {
            RequireEmailAddress = false;
            MaxResultsPerType = 9999;
        }

        public SearchType SearchType { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }

        #region Advanced Searching 
        public string Company { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string TradeArea { get; set; }
        public string TradeCodeListId { get; set; }
        public string DistributorCodeListId { get; set; }
        public LocatorResultTypes LocatorResultType { get; set; }
        public LocatorBusinessTypes LocatorBusinessType { get; set; }
        public DealerLevels DealerLevel { get; set; }
        public bool RequireEmailAddress { get; set; }
        public int MaxResultsPerType { get; set; }
        #endregion

        public override string ToString()
        {
            return SearchType.ToString() + ":" + LocatorBusinessType + ":" + LocatorResultType + ":" + Location;
        }
    }
}
