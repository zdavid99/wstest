using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.ocwebservice.locator.data;
using com.ocwebservice.locator.utility;
using OwensCorning.Locator.Data;
using com.ocwebservice.locator.model;
using System.Linq.Expressions;

namespace com.ocwebservice.locator.dao
{
    public partial class LocatorDAO
    {
        private static readonly LocatorResultTypes AllMasonryLocatorResultTypes =
            LocatorResultTypes.Dealer | LocatorResultTypes.Installer;

        private static readonly string MasonrySelectInstaller = "103";
        private static readonly string MasonryCommercialInstaller = "102";
        private static readonly string MasonryResidentialInstaller = "101";

        private void InitializeMasonryDAO()
        {
            AddDealerLevel(DealerLevels.PlatinumElite,"PE");
            AddDealerLevel(DealerLevels.Platinum, "PLATINUM");
            AddDealerLevel(DealerLevels.Gold, "GOLD");
            AddDealerLevel(DealerLevels.Silver, "SILVER");
            AddDealerLevel(DealerLevels.None, String.Empty);
            AddLocatorResultType(LocatorResultTypes.Installer, "CSI");
            AddLocatorResultType(LocatorResultTypes.Builder, "CSB");
            AddLocatorResultType(LocatorResultTypes.Dealer, "CSD");
            AddLocatorResultType(LocatorResultTypes.None, string.Empty);
        }

        private class MasonryLocatorResultWrapper
        {
            public RawMasonryLocatorResult Result { get; set; }
            public double Distance { get; set; }
        }

        private Contractor GetMasonryContractorWithEmail(string email)
        {
            return GetMasonryContractorForFunction(c => c.Email == email);
        }

        private Contractor GetMasonryContractorWithForeignId(string foreignId)
        {
            int prodeskId = int.Parse(foreignId);
            return GetMasonryContractorForFunction(c => c.ProDesk_ID == prodeskId);
        }

        private Contractor GetMasonryContractorForFunction(Func<RawMasonryLocatorResult, bool> func)
        {
            if (MasonryLocatorResultTypeToKeyDictionary.Count +
                MasonryDealerLevelToKeyDictionary.Count == 0)
            {
                InitializeMasonryDAO();
            }

            Contractor result = null;
            using (MasonryLocatorDataContext dataContext = new MasonryLocatorDataContext())
            {
                var rawResults =
                    dataContext.RawMasonryLocatorResults
                    .Where(func).ToList();
                if (rawResults.Count > 0)
                {
                    result = BuildMasonryLocatorResult(rawResults.First(), 0.0) as Contractor;
                }
            }
            return result;
        }

        private List<ILocatorResult> GetMasonryLocatorResults(LocatorSearchOptions searchOptions)
        {
            if (MasonryLocatorResultTypeToKeyDictionary.Count + 
                MasonryDealerLevelToKeyDictionary.Count == 0)
            {
                InitializeMasonryDAO();
            }

            //Disable builder search
            var locatorResultType = searchOptions.LocatorResultType & ~LocatorResultTypes.Builder;

            using (MasonryLocatorDataContext dataContext = new MasonryLocatorDataContext())
            {
                IQueryable<MasonryLocatorResultWrapper> results;

                if (searchOptions.SearchType != SearchType.Advanced)
                {
                    results =
                        (from dealer in dataContext.RawMasonryLocatorResults
                         join zipCode in dataContext.RawMasonryLocations on dealer.ShipZip equals zipCode.PostalCode
                         let distance =
                             //TODO find some way to abstract this out into an expression JGK
                             DistanceCalculationUtil.EARTH_RADIUS * (
                                 Math.Atan(
                                     Math.Sqrt(1 -
                                         Math.Pow(
                                             Math.Sin(searchOptions.Latitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Sin(zipCode.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) +
                                             Math.Cos(searchOptions.Latitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Cos(zipCode.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) *
                                             Math.Cos(zipCode.Longitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN - searchOptions.Longitude / DistanceCalculationUtil.DEGREES_PER_RADIAN)
                                         , 2))
                                 /
                                     (Math.Sin(searchOptions.Latitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Sin(zipCode.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) +
                                     Math.Cos(searchOptions.Latitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Cos(zipCode.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) *
                                     Math.Cos(zipCode.Longitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN - searchOptions.Longitude / DistanceCalculationUtil.DEGREES_PER_RADIAN))
                                 )
                             )
                         where distance <= searchOptions.Radius
                         orderby distance, zipCode.PostalCode
                         select new MasonryLocatorResultWrapper{ Result = dealer, Distance = distance });
                }
                else
                {
                    if (searchOptions.Latitude > 0.0)
                    {
                        results =
                            from dealer in dataContext.RawMasonryLocatorResults
                            join zipCode in dataContext.RawMasonryLocations on dealer.ShipZip equals zipCode.PostalCode
                            let distance =
                                //TODO find some way to abstract this out into an expression JGK
                             DistanceCalculationUtil.EARTH_RADIUS * (
                                 Math.Atan(
                                     Math.Sqrt(1 -
                                         Math.Pow(
                                             Math.Sin(searchOptions.Latitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Sin(zipCode.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) +
                                             Math.Cos(searchOptions.Latitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Cos(zipCode.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) *
                                             Math.Cos(zipCode.Longitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN - searchOptions.Longitude / DistanceCalculationUtil.DEGREES_PER_RADIAN)
                                         , 2))
                                 /
                                     (Math.Sin(searchOptions.Latitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Sin(zipCode.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) +
                                     Math.Cos(searchOptions.Latitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Cos(zipCode.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) *
                                     Math.Cos(zipCode.Longitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN - searchOptions.Longitude / DistanceCalculationUtil.DEGREES_PER_RADIAN))
                                 )
                             )
                            orderby dealer.Product_Lead_Type_Code descending, dealer.Company ascending
                            select new MasonryLocatorResultWrapper { Result = dealer, Distance = distance };
                    }
                    else
                    {

                        results =
                            from dealer in dataContext.RawMasonryLocatorResults
                            orderby (dealer.Product_Lead_Type_Code == MasonrySelectInstaller) descending, dealer.Company ascending
                            select new MasonryLocatorResultWrapper { Result = dealer, Distance = 0 };
                    }

                    if (!String.IsNullOrEmpty(searchOptions.Company))
                    {
                        results = results.Where(lr => lr.Result.Company.Contains(searchOptions.Company));
                    }

                    if (!String.IsNullOrEmpty(searchOptions.City))
                    {
                        results = results.Where(lr => lr.Result.ShipCity.Contains(searchOptions.City));
                    }

                    if (!String.IsNullOrEmpty(searchOptions.State))
                    {
                        results = results.Where(lr => lr.Result.ShipState == searchOptions.State);
                    }
                }

                if (searchOptions.RequireEmailAddress)
                {
                    results = results.Where(lr => lr.Result.Email != null && lr.Result.Email.Length > 5);
                }

                // add where for dealer type
                if (!(LocatorResultTypes.None == locatorResultType || //If none is selected, return everything
                      AllMasonryLocatorResultTypes == locatorResultType)) //If everything is selected, do not apply filter.
                {
                    //if builders are ever added, this needs to have another case to support two out of three locator result types
                    results =
                        results
                        .Where(lr => 
                                lr.Result.Member_Type_Code.StartsWith(MasonryLocatorResultTypeToKeyDictionary[locatorResultType]
                            ));
                }

                List<ILocatorResult> locatorResults =
                        results.ToList().ConvertAll(r => BuildMasonryLocatorResult(r.Result, r.Distance));

                // if these are dealers and not installers sort by dealer level 
                if ((LocatorResultTypes.Dealer & locatorResultType) == locatorResultType)
                {
                    List<Contractor> contractors =
                        locatorResults
                            .Take(searchOptions.MaxResultsPerType)
                            .Where(lr => lr is Contractor)
                            .ToList().ConvertAll(lr => lr as Contractor);
                    List<Dealer> dealers =
                        locatorResults
                            .Take(searchOptions.MaxResultsPerType)
                            .Where(lr => lr is Dealer)
                            .ToList().ConvertAll(lr => lr as Dealer)
                            .OrderByDescending(d => d.DealerLevel).ToList();

                    locatorResults = new List<ILocatorResult>();
                    locatorResults.AddRange(contractors.ConvertAll(c => c as ILocatorResult));
                    locatorResults.AddRange(dealers.ConvertAll(d => d as ILocatorResult));
                }
                else
                {
                    locatorResults = locatorResults.Take(searchOptions.MaxResultsPerType).ToList();
                }

                return locatorResults;
            }
        }
        private static ILocatorResult BuildMasonryLocatorResult(RawMasonryLocatorResult rawResult, double distance)
        {
            ILocatorResult result;
            LocatorResultTypes type = MasonryKeyToLocatorResultTypeDictionary[TrimDealerTypeCode(rawResult)];
            if (type == LocatorResultTypes.Installer)
            {
                Contractor installer = new Contractor(LocatorBusinessTypes.Masonry);                
                result = installer;
            }
            else
            {
                Dealer dealer = new Dealer(LocatorBusinessTypes.Masonry);
                dealer.DealerLevel = MasonryKeyToDealerLevelDictionary[rawResult.Member_Level];
                result = dealer;
            }

            result.Id = rawResult.ProDesk_ID;
            result.Company = rawResult.Company;
            result.Address = rawResult.ShipAddress1;
            result.City = rawResult.ShipCity;
            result.State = rawResult.ShipState;
            result.Zip = rawResult.ShipZip;
            result.Phone = rawResult.Phone;
            result.Email = rawResult.Email;
            result.Website = rawResult.URL;
            result.DateEnrolled = rawResult.Date_Enrolled;
            result.Distance = distance;

            result.BusinessMarket = LocatorBusinessMarkets.None;
            if (rawResult.Product_Lead_Type_Code == MasonryResidentialInstaller || rawResult.Product_Lead_Type_Code == MasonrySelectInstaller)
                result.BusinessMarket |= LocatorBusinessMarkets.Residential;
            if (rawResult.Product_Lead_Type_Code == MasonryCommercialInstaller || rawResult.Product_Lead_Type_Code == MasonrySelectInstaller)
                result.BusinessMarket |= LocatorBusinessMarkets.Professional;

            if (type == LocatorResultTypes.Installer)
            {
                Contractor installer = result as Contractor;
                installer.AsmEmail = rawResult.SalesRepEmailID;
                installer.AsmFirstName = rawResult.SalesRepFirstName;
                installer.AsmLastName = rawResult.SalesRepLastName;
            }

            return result;
        }

        /// <summary>
        /// Trims the dealer type code so that distributor (CSD) and dealer (CSDLR) types are the same.
        /// </summary>
        /// <param name="rawResult">The raw result.</param>
        /// <returns></returns>
        private static string TrimDealerTypeCode(RawMasonryLocatorResult rawResult)
        {
            return rawResult.Member_Type_Code.Substring(0, 3);
        }

        public static Dictionary<LocatorResultTypes, string> MasonryLocatorResultTypeToKeyDictionary = new Dictionary<LocatorResultTypes,string>();
        public static Dictionary<string, LocatorResultTypes> MasonryKeyToLocatorResultTypeDictionary = new Dictionary<string,LocatorResultTypes>();
        private void AddLocatorResultType(LocatorResultTypes resultType, string key)
        {
            MasonryLocatorResultTypeToKeyDictionary.Add(resultType, key);
            MasonryKeyToLocatorResultTypeDictionary.Add(key, resultType);
        }

        public static Dictionary<DealerLevels, string> MasonryDealerLevelToKeyDictionary = new Dictionary<DealerLevels,string>();
        public static Dictionary<string, DealerLevels> MasonryKeyToDealerLevelDictionary = new Dictionary<string,DealerLevels>();
        private void AddDealerLevel(DealerLevels dealerLevel, string key)
        {
            MasonryDealerLevelToKeyDictionary.Add(dealerLevel, key);
            MasonryKeyToDealerLevelDictionary.Add(key, dealerLevel);
        }
    }
}