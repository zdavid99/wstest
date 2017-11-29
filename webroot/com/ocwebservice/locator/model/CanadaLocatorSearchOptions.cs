using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using OwensCorning.Locator.Data;

namespace com.ocwebservice.locator.model
{
    [DataContract]
    public class CanadaLocatorSearchOptions 
    {
        public CanadaLocatorSearchOptions()
        {
            RequireEmailAddress = false;
            MaxResultsPerType = 9999;
        }

        public static CanadaLocatorSearchOptions Empty
        {
            get { return new CanadaLocatorSearchOptions(); }
        }

        [DataMember]
        public SearchType SearchType { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string PostalCode
        {
            get { return Location; }
            set { Location = value; }
        }

        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        [DataMember]
        public double Radius { get; set; }

        #region Advanced Searching
        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string County { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string TradeArea { get; set; }

        [DataMember]
        public string TradeCodeListId { get; set; }

        [DataMember]
        public string DistributorCodeListId { get; set; }

        [DataMember]
        public LocatorResultTypes LocatorResultType { get; set; }

        [DataMember]
        public LocatorBusinessTypes LocatorBusinessType { get; set; }

        [DataMember]
        public DealerLevels DealerLevel { get; set; }

        [DataMember]
        public bool RequireEmailAddress { get; set; }

        [DataMember]
        public int MaxResultsPerType { get; set; }

        [DataMember]
        public string[] Products { get; set; }

        [DataMember]
        public int ResultsPerPage { get; set; }

        [DataMember]
        public int PageNumber { get; set; }
        #endregion

        public override string ToString()
        {
            return SearchType.ToString() + ":" + LocatorBusinessType + ":" + LocatorResultType + ":" + Location;
        }
    }
}
