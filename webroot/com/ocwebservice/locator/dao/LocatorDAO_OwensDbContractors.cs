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
        #region Contractor ID arrays
        private static int[] BUILDER_TYPE_IDS
        {
            get { return new int[] { 9, 28 }; }
        }
        private static int[] BUILDER_SYSTEM_THINKING_IDS
        {
            get { return new int[] { 27 }; }
        }
        private static int[] BUILDER_ALL_TYPE_IDS
        {
            get { return CombineIdArrays(BUILDER_TYPE_IDS, BUILDER_SYSTEM_THINKING_IDS); }
        }
        private static int[] INSULATION_INSTALLER_TYPE_IDS
        {
            get { return new int[] { 42 }; }
        }
        private static int[] INSULATION_CEP_TYPE_IDS
        {
            get { return new int[] { 3 }; }
        }
        private static int[] INSULATION_ALL_TYPE_IDS
        {
            get { return CombineIdArrays(INSULATION_CEP_TYPE_IDS, INSULATION_INSTALLER_TYPE_IDS); }
        }
        private static int[] ROOFING_INSTALLER_TYPE_IDS
        {
            get { return new int[] {12, 40, 41, 43, 44, 45, 46}; }
        }

        private static int[] CombineIdArrays(int[] array1, int[] array2)
        {
            List<int> combinedList = new List<int>();
            combinedList.AddRange(array1);
            combinedList.AddRange(array2);
            combinedList.Sort();
            return combinedList.ToArray();
        }
        #endregion Contractor ID arrays

        private static bool CheckContractorResultType(RawBMContractor rawResult, int[] typesToCheck)
        {
            bool typeFound = false;

            int[] contractorTypes =
            (from RawBMContractorType contractorType
             in rawResult.RawBMContractorTypes
             select contractorType.type_id).ToArray();

            foreach (int contractorType in contractorTypes)
            {
                if (typesToCheck.Contains(contractorType))
                {
                    typeFound = true;
                    break;
                }
            }
            return typeFound;
        }

        private Contractor GetBMContractorWithEmail(string email)
        {
            return GetBMContractorForFunction(c => c.MemberEmailID == email);
        }

        private Contractor GetBMContractorWithForeignId(string foreignId)
        {
            int prodeskId = int.Parse(foreignId);
            return GetBMContractorForFunction(c => c.id == prodeskId);
        }

        private Contractor GetBMContractorForFunction(Func<RawBMContractor, bool> func)
        {
            Contractor result = null;
            LocatorSearchOptions searchOptions = 
                new LocatorSearchOptions() 
                    { LocatorBusinessType = LocatorBusinessTypes.All };
            using (ContractorLocatorDataContext dataContext = new ContractorLocatorDataContext())
            {
                var rawResults =
                    dataContext.RawBMContractors.Where(func).ToList();
                if (rawResults.Count > 0)
                {
                    result = BuildContractorLocatorResult(rawResults.First(), 0.0, searchOptions);
                }
            }
            return result;
        }

        private IEnumerable<ILocatorResult> GetContractorLocatorResults(LocatorSearchOptions searchOptions)
        {
            int[] typesToSearchFor = new int[0];
            if ((searchOptions.LocatorResultType & LocatorResultTypes.Installer) == LocatorResultTypes.Installer)
            {
                if ((searchOptions.LocatorBusinessType & LocatorBusinessTypes.Roofing) == LocatorBusinessTypes.Roofing)
                {
                    typesToSearchFor = CombineIdArrays(typesToSearchFor, ROOFING_INSTALLER_TYPE_IDS);
                }
                if ((searchOptions.LocatorBusinessType & LocatorBusinessTypes.ResidentialInsulation) == LocatorBusinessTypes.ResidentialInsulation)
                {
                    typesToSearchFor = CombineIdArrays(typesToSearchFor, INSULATION_ALL_TYPE_IDS);
                }
            }
            if ((searchOptions.LocatorResultType & LocatorResultTypes.Builder) == LocatorResultTypes.Builder)
            {
                typesToSearchFor = CombineIdArrays(typesToSearchFor, BUILDER_ALL_TYPE_IDS);
            }

            double searchLatitude = searchOptions.Latitude;
            double searchLongitude = searchOptions.Longitude * -1;

            using (ContractorLocatorDataContext dataContext = new ContractorLocatorDataContext())
            {
                List<Contractor> contractors;
                if (searchOptions.SearchType != SearchType.Advanced)
                {
                    var searchResults =
                        (from contractor in dataContext.RawBMContractors
                        join location in dataContext.RawBMLocations
                        on contractor.zip equals location.PostalCode
                        let distance =
                            //TODO find some way to abstract this out into an expression JGK
                            DistanceCalculationUtil.EARTH_RADIUS * (
                                Math.Atan(
                                    Math.Sqrt(1 -
                                        Math.Pow(
                                            Math.Sin(searchLatitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Sin(location.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) +
                                            Math.Cos(searchLatitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Cos(location.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) *
                                            Math.Cos(location.Longitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN - searchLongitude / DistanceCalculationUtil.DEGREES_PER_RADIAN)
                                        , 2))
                                /
                                    (Math.Sin(searchLatitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Sin(location.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) +
                                    Math.Cos(searchLatitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Cos(location.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) *
                                    Math.Cos(location.Longitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN - searchLongitude / DistanceCalculationUtil.DEGREES_PER_RADIAN))
                                )
                            )
                        where distance <= searchOptions.Radius
                        && (searchOptions.RequireEmailAddress ? (contractor.MemberEmailID != null && contractor.MemberEmailID.Length > 5) : true)
                        && contractor.RawBMContractorTypes.Any(type => typesToSearchFor.Contains(type.type_id))
                         select new { Contractor = contractor, Distance = distance }).ToList();
                    contractors = searchResults.ConvertAll<Contractor>(a => BuildContractorLocatorResult(a.Contractor, a.Distance, searchOptions));
                }
                else
                {
                    var searchResults =
                        from contractor in dataContext.RawBMContractors
                        join location in dataContext.RawBMLocations
                        on contractor.zip equals location.PostalCode
                        let distance =
                            //TODO find some way to abstract this out into an expression JGK
                            DistanceCalculationUtil.EARTH_RADIUS * (
                                Math.Atan(
                                    Math.Sqrt(1 -
                                        Math.Pow(
                                            Math.Sin(searchLatitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Sin(location.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) +
                                            Math.Cos(searchLatitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Cos(location.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) *
                                            Math.Cos(location.Longitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN - searchLongitude / DistanceCalculationUtil.DEGREES_PER_RADIAN)
                                        , 2))
                                /
                                    (Math.Sin(searchLatitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Sin(location.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) +
                                    Math.Cos(searchLatitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Cos(location.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) *
                                    Math.Cos(location.Longitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN - searchLongitude / DistanceCalculationUtil.DEGREES_PER_RADIAN))
                                )
                            )
                        where contractor.RawBMContractorTypes.Any(type => typesToSearchFor.Contains(type.type_id))
                        select new { Contractor = contractor, Distance = distance };

                    if (!String.IsNullOrEmpty(searchOptions.Company))
                    {
                        searchResults = searchResults.Where(result => result.Contractor.store.Contains(searchOptions.Company));
                    }
                    if (!String.IsNullOrEmpty(searchOptions.City))
                    {
                        searchResults = searchResults.Where(result => result.Contractor.city.Contains(searchOptions.City));
                    }
                    if (!String.IsNullOrEmpty(searchOptions.State))
                    {
                        searchResults = searchResults.Where(result => result.Contractor.state == searchOptions.State);
                    }
                    if (searchOptions.RequireEmailAddress)
                    {
                        searchResults = searchResults.Where(result => result.Contractor.MemberEmailID != null && result.Contractor.MemberEmailID.Length > 5);
                    }
                    contractors = searchResults.Take(250).ToList()
                        .ConvertAll<Contractor>(a => 
                            BuildContractorLocatorResult(a.Contractor, a.Distance, searchOptions));
                }

				contractors = contractors.Where(c => !c.ContractorPrograms.Includes(ContractorPrograms.IsSilverRewards)).ToList();

                var sortedResults = (from contractor in contractors
                        //where (contractor.LocatorResultType & searchOptions.LocatorResultType) == contractor.LocatorResultType
                        orderby contractor.BusinessType descending,
                                contractor.LocatorResultType descending,
                                GetContractorSortOrder(contractor) descending,
                                contractor.Distance,
                                contractor.Company
                        select contractor);

                var finalResults = 
                    sortedResults
                        .Where(c => c.LocatorResultType == LocatorResultTypes.Installer)
                        .Take(searchOptions.MaxResultsPerType).ToList();
                finalResults.AddRange(
                    sortedResults
                        .Where(c => c.LocatorResultType == LocatorResultTypes.Builder)
                        .Take(searchOptions.MaxResultsPerType));
                return finalResults.ConvertAll(c => c as ILocatorResult);
            }
        }

        private static int GetContractorSortOrder(Contractor contractor)
        {
            if (contractor is Builder)
            {
                Builder builder = contractor as Builder;
                int sortOrder = 0;
                if ((builder.BuilderPrograms & BuilderPrograms.IsQuietZoneBuilder) == BuilderPrograms.IsQuietZoneBuilder)
                {
                    sortOrder += 8;
                }
                if ((builder.ContractorPrograms & ContractorPrograms.IsTopOfTheHouse) == ContractorPrograms.IsTopOfTheHouse)
                {
                    sortOrder += 4;
                }
                if ((builder.ContractorPrograms & ContractorPrograms.IsExteriorFx) == ContractorPrograms.IsExteriorFx)
                {
                    sortOrder += 2;
                }
                if ((builder.BuilderPrograms & BuilderPrograms.IsSystemThinkingBuilder) == BuilderPrograms.IsSystemThinkingBuilder)
                {
                    sortOrder += 1;
                }
                return sortOrder;
            }
            else
            {
                return (int)
                    (contractor.ContractorPrograms
                     & ~(ContractorPrograms.IsExteriorFx
                        |ContractorPrograms.HasProConnectProfile|ContractorPrograms.IsTotalProtectionRoofingSystem));
            }
        }

        private static int GetBuilderSortOrder(Builder builder)
        {
            int sortOrder = 0;
            if (builder != null)
            {
                return (int)builder.BuilderPrograms;
            }
            return sortOrder;
        }

        private static Contractor BuildContractorLocatorResult(RawBMContractor rawResult, double distance, LocatorSearchOptions searchOptions)
        {
            Contractor result =
                CreateContractorOrBuilder(rawResult, searchOptions.LocatorBusinessType);

            result.Id = rawResult.id;
            result.ContactName = rawResult.contact;
            result.Company = rawResult.store;
            result.Address = rawResult.address1;
            result.Address2 = rawResult.address2;
            result.City = rawResult.city;
            result.State = rawResult.state;
            result.Zip = rawResult.zip;
            result.Phone = rawResult.phone;
            result.Fax = rawResult.phone2;
            result.AsmEmail = rawResult.SalesRepEmailID;
            result.AsmLastName = rawResult.salesrep;
            result.AsmFirstName = rawResult.SalesRepFirstName;
            result.Email = rawResult.MemberEmailID;
            result.Website = rawResult.url;
            result.Distance = distance;

            result.ContractorPrograms |= rawResult.ProConnectFlag ? ContractorPrograms.HasProConnectProfile : 0;
            result.ContractorPrograms |= rawResult.Top_Of_The_House_Certified == 'Y' ? ContractorPrograms.IsTopOfTheHouse : 0;
            result.ContractorPrograms |= rawResult.IsExFxCertified == 'Y' ? ContractorPrograms.IsExteriorFx : 0;
            bool isContractor = CheckContractorResultType(rawResult, ROOFING_INSTALLER_TYPE_IDS);
            result.ContractorPrograms |= isContractor ? ContractorPrograms.IsPreferred : 0;
            result.ContractorPrograms |= rawResult.IsPreferredPride == 'Y' ? ContractorPrograms.IsPreferredPride : 0;
            result.ContractorPrograms |= rawResult.IsPreferredPlat == 'Y' ? ContractorPrograms.IsPlatinumPreferred : 0;
            result.ContractorPrograms |= rawResult.Platinum_Pride_Award == 'Y' ? ContractorPrograms.IsPlatinumAwardWinner : 0;
            result.ContractorPrograms |= rawResult.Total_Protection_Rfg_System == 'Y' ? ContractorPrograms.IsTotalProtectionRoofingSystem : 0;

            result.ContractorPrograms |= GetRewardsFlags(rawResult.RewardsLevel);

            if (result is Builder)
            {
                Builder builder = result as Builder;
                builder.BuilderPrograms |= rawResult.IsQuietZoneBuilder == 'Y' ? BuilderPrograms.IsQuietZoneBuilder : 0;
                bool isSystemThinkingBuilder = CheckContractorResultType(rawResult, BUILDER_SYSTEM_THINKING_IDS);
                builder.BuilderPrograms |= isSystemThinkingBuilder ? BuilderPrograms.IsSystemThinkingBuilder : 0;
            }

            return result;
        }

        private static ContractorPrograms GetRewardsFlags(int? rewardsLevel)
        {
            if ( rewardsLevel.HasValue )
            {
                switch (rewardsLevel.Value)
                {
                    case 1:
                        return ContractorPrograms.IsSilverRewards;
                    case 2:
                        return ContractorPrograms.IsGoldRewards;
                    case 3:
                        return ContractorPrograms.IsPlatinumRewards;
                    case 4:
                        return ContractorPrograms.IsDiamondRewards;
                    default:
                        break;
                }
            }
            return ContractorPrograms.None;
        }

        private static Contractor CreateContractorOrBuilder(RawBMContractor rawResult, LocatorBusinessTypes businessType)
        {
            if (CheckContractorResultType(rawResult, BUILDER_ALL_TYPE_IDS))
            {
                return new Builder(LocatorBusinessTypes.Roofing | LocatorBusinessTypes.ResidentialInsulation);
            }
            else if (CheckContractorResultType(rawResult, ROOFING_INSTALLER_TYPE_IDS))
            {
                return new Contractor(LocatorBusinessTypes.Roofing);
            }
            else if (CheckContractorResultType(rawResult, INSULATION_ALL_TYPE_IDS))
            {
                return new Contractor(LocatorBusinessTypes.ResidentialInsulation);
            }
            return new Contractor(businessType);
        }
    }
}