using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using com.ocwebservice.locator;
using com.ocwebservice.locator.utility;
using com.ocwebservice.locator.data;
using OwensCorning.Locator.Data;
using com.ocwebservice.locator.model;
using com.ocwebservice.locator.dao;

namespace com.ocwebservice
{
    // NOTE: If you change the class name "Locator" here, you must also update the reference to "Locator" in Web.config.
    public class LocatorV2 : ILocator
    {
        private List<ILocatorResult> GetInternalLocatorResults(LocatorSearchOptions searchOptions)
        {
            SetSearchOptionsGeolocation(searchOptions);
            List<ILocatorResult> results = LocatorDAO.Instance.GetLocatorResults(searchOptions);
            return results;
        }

        private List<CanadianLocatorResult> GetInternalCanadaLocatorResults(CanadaLocatorSearchOptions searchOptions)
        {
            if (searchOptions.Latitude == (float)0.0 &&
                searchOptions.Longitude == (float)0.0 &&
                !string.IsNullOrEmpty(searchOptions.PostalCode))
            {
                LocatorDAO_Canada.Instance.PopulateLatLongFromZip(searchOptions);
            }

            if (searchOptions.ResultsPerPage < 1)
            {
                searchOptions.ResultsPerPage = 20;
            }

            if (searchOptions.PageNumber < 1)
            {
                searchOptions.PageNumber = 1;
            }

            if (searchOptions.Products.Count() == 0 || string.IsNullOrEmpty(searchOptions.Products[0]))
            {
                searchOptions.Products = new string[] { "CommBatts", "CommFoam", "Batts", "Foam" };
            }

            return LocatorDAO_Canada.Instance.GetDealers(searchOptions).ToList();
        }

        public ILocatorResult[] GetLocatorResults(LocatorSearchOptions searchOptions)
        {
            SetSearchOptionsGeolocation(searchOptions);
            return GetInternalLocatorResults(searchOptions).ToArray();
        }

        public CanadianLocatorResult[] GetCanadaLocatorResults(CanadaLocatorSearchOptions searchOptions)
        {
            return GetInternalCanadaLocatorResults(searchOptions).ToArray();
        }
        public Contractor GetContractorWithForeignId(string foreignId)
        {
            return LocatorDAO.Instance.GetContractorWithForeignId(foreignId);
        }

        public Contractor GetContractorWithEmail(string email)
        {
            return LocatorDAO.Instance.GetContractorWithEmail(email);
        }

        public Contractor[] GetContractors(LocatorSearchOptions searchOptions)
        {
            searchOptions.LocatorResultType = LocatorResultTypes.Installer;
            SetSearchOptionsGeolocation(searchOptions);
            return GetInternalLocatorResults(searchOptions)
                .ConvertAll(result => result as Contractor).ToArray();
        }

        public Dealer[] GetRoofingDealers(string zipCode, int radius)
        {
            LocatorSearchOptions searchOptions =
                new LocatorSearchOptions
                {
                    LocatorBusinessType = LocatorBusinessTypes.Roofing,
                    LocatorResultType = LocatorResultTypes.Dealer,
                    Location = zipCode,
                    Radius = radius
                };
            SetSearchOptionsGeolocation(searchOptions);
            return GetInternalLocatorResults(searchOptions)
                .ConvertAll(result => result as Dealer).ToArray();
        }

        public Contractor[] GetRoofingInstallers(string zipCode, int radius)
        {
            LocatorSearchOptions searchOptions =
                new LocatorSearchOptions
                {
                    LocatorBusinessType = LocatorBusinessTypes.Roofing,
                    LocatorResultType = LocatorResultTypes.Installer,
                    Location = zipCode,
                    Radius = radius
                };
            SetSearchOptionsGeolocation(searchOptions);
            return GetInternalLocatorResults(searchOptions)
                .ConvertAll(result => result as Contractor).ToArray();
        }

        public Builder[] GetBuilders(string zipCode, int radius)
        {
            LocatorSearchOptions searchOptions =
                new LocatorSearchOptions
                {
                    LocatorBusinessType = (LocatorBusinessTypes.ResidentialInsulation | LocatorBusinessTypes.Roofing),
                    LocatorResultType = LocatorResultTypes.Builder,
                    Location = zipCode,
                    Radius = radius
                };
            SetSearchOptionsGeolocation(searchOptions);
            return GetInternalLocatorResults(searchOptions)
                .ConvertAll(result => result as Builder).ToArray();
        }

        public Dealer[] GetMasonryDealers(string zipCode, int radius)
        {
            LocatorSearchOptions searchOptions =
                new LocatorSearchOptions
                {
                    LocatorBusinessType = LocatorBusinessTypes.Masonry,
                    LocatorResultType = LocatorResultTypes.Dealer,
                    Location = zipCode,
                    Radius = radius
                };
            SetSearchOptionsGeolocation(searchOptions);
            return GetInternalLocatorResults(searchOptions)
                .ConvertAll(result => result as Dealer).ToArray();
        }

        public Contractor[] GetMasonryInstallers(string zipCode, int radius)
        {
            LocatorSearchOptions searchOptions =
                new LocatorSearchOptions
                {
                    LocatorBusinessType = LocatorBusinessTypes.Masonry,
                    LocatorResultType = LocatorResultTypes.Installer,
                    Location = zipCode,
                    Radius = radius
                };
            SetSearchOptionsGeolocation(searchOptions);
            return GetInternalLocatorResults(searchOptions)
                .ConvertAll(result => result as Contractor).ToArray();
        }

        private static void SetSearchOptionsGeolocation(LocatorSearchOptions searchOptions)
        {
            if (!String.IsNullOrEmpty(searchOptions.Location))
            {
                Location location = LocationDAO.Instance.GetLocationWithPostalCode(searchOptions.Location);
                searchOptions.Latitude = (double)location.Latitude;
                searchOptions.Longitude = (double)location.Longitude;
            }
        }

        #region ILocator Members


        public com.ocwebservice.locator.model.WCFPagedList<ILocatorResult> GetPagedLocatorResults(LocatorSearchOptions searchOptions, OwensCorning.Paging.PageInfo pagingInfo)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
