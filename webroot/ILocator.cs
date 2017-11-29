using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using OwensCorning.Locator.Data;
using com.ocwebservice.locator.model;

namespace com.ocwebservice
{
    // NOTE: If you change the interface name "ILocator" here, you must also update the reference to "ILocatorService" in Web.config.
    [ServiceContract]
    public interface ILocator
    {
        [OperationContract]
        [ServiceKnownType(typeof(Contractor))]
        [ServiceKnownType(typeof(Dealer))]
        [ServiceKnownType(typeof(Builder))]
        ILocatorResult[] GetLocatorResults(LocatorSearchOptions searchOptions);

        [OperationContract]
        CanadianLocatorResult[] GetCanadaLocatorResults(CanadaLocatorSearchOptions searchOptions);

        [OperationContract]
        Contractor GetContractorWithForeignId(string foreignId);

        [OperationContract]
        Contractor GetContractorWithEmail(string email);

        [OperationContract]
        Contractor[] GetContractors(LocatorSearchOptions searchOptions);

        [OperationContract]
        Dealer[] GetRoofingDealers(string zipCode, int radius);

        [OperationContract]
        Contractor[] GetRoofingInstallers(string zipCode, int radius);

        [OperationContract]
        Builder[] GetBuilders(string zipCode, int radius);

        [OperationContract]
        Dealer[] GetMasonryDealers(string zipCode, int radius);

        [OperationContract]
        Contractor[] GetMasonryInstallers(string zipCode, int radius);

        [OperationContract]
        [ServiceKnownType(typeof(Contractor))]
        [ServiceKnownType(typeof(Dealer))]
        [ServiceKnownType(typeof(DealerV3))]
		[ServiceKnownType(typeof(DealerV4))]
        [ServiceKnownType(typeof(Builder))]
        com.ocwebservice.locator.model.WCFPagedList<ILocatorResult> GetPagedLocatorResults(LocatorSearchOptions searchOptions, OwensCorning.Paging.PageInfo pagingInfo);
    }
}
