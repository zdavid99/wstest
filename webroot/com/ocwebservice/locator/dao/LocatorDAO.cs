using System.Collections.Generic;
using com.ocwebservice.locator.utility;
using OwensCorning.Locator.Data;

namespace com.ocwebservice.locator.dao
{
    public partial class LocatorDAO
    {
        private LocatorDAO()
        {
        }
        private static readonly LocatorDAO self = new LocatorDAO();
        public static LocatorDAO Instance
        {
            get { return self; }
        }

        public Contractor GetContractorWithForeignId(string foreignId)
        {
            Contractor result = GetBMContractorWithForeignId(foreignId);
            if (result == null)
            {
                result = GetMasonryContractorWithForeignId(foreignId);
            }
            return result;
        }

        public Contractor GetContractorWithEmail(string email)
        {
            Contractor result = GetBMContractorWithEmail(email);
            if (result == null)
            {
                result = GetMasonryContractorWithEmail(email);
            }
            return result;
        }

        public List<ILocatorResult> GetLocatorResults(LocatorSearchOptions searchOptions)
        {
            List<ILocatorResult> results = new List<ILocatorResult>();
            if ((searchOptions.LocatorBusinessType & LocatorBusinessTypes.Masonry) == LocatorBusinessTypes.Masonry)
            {
                results.AddRange(GetMasonryLocatorResults(searchOptions));
            }
            if ((searchOptions.LocatorBusinessType & LocatorBusinessTypes.Roofing) == LocatorBusinessTypes.Roofing)
            {
                results.AddRange(GetBMLocatorResults(searchOptions));
            }
            return results;
        }
    }
}