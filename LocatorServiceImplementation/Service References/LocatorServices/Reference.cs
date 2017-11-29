﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4200
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OwensCorning.Utility.LocatorServices {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LocatorServices.ILocator")]
    public interface ILocator {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocator/GetLocatorResults", ReplyAction="http://tempuri.org/ILocator/GetLocatorResultsResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.Builder))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.Contractor))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.LocatorResult))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.LocatorBusinessTypes))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.LocatorResultTypes))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.ContractorPrograms))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.BuilderPrograms))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.Dealer))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.DealerLevels))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.DealerPrograms))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.LocatorSearchOptions))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.SearchType))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.Contractor[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.Dealer[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.Builder[]))]
        object[] GetLocatorResults(OwensCorning.Locator.Data.LocatorSearchOptions searchOptions);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocator/GetContractorWithForeignId", ReplyAction="http://tempuri.org/ILocator/GetContractorWithForeignIdResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.Builder))]
        OwensCorning.Locator.Data.Contractor GetContractorWithForeignId(string foreignId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocator/GetContractorWithEmail", ReplyAction="http://tempuri.org/ILocator/GetContractorWithEmailResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(OwensCorning.Locator.Data.Builder))]
        OwensCorning.Locator.Data.Contractor GetContractorWithEmail(string email);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocator/GetContractors", ReplyAction="http://tempuri.org/ILocator/GetContractorsResponse")]
        OwensCorning.Locator.Data.Contractor[] GetContractors(OwensCorning.Locator.Data.LocatorSearchOptions searchOptions);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocator/GetRoofingDealers", ReplyAction="http://tempuri.org/ILocator/GetRoofingDealersResponse")]
        OwensCorning.Locator.Data.Dealer[] GetRoofingDealers(string zipCode, int radius);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocator/GetRoofingInstallers", ReplyAction="http://tempuri.org/ILocator/GetRoofingInstallersResponse")]
        OwensCorning.Locator.Data.Contractor[] GetRoofingInstallers(string zipCode, int radius);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocator/GetBuilders", ReplyAction="http://tempuri.org/ILocator/GetBuildersResponse")]
        OwensCorning.Locator.Data.Builder[] GetBuilders(string zipCode, int radius);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocator/GetMasonryDealers", ReplyAction="http://tempuri.org/ILocator/GetMasonryDealersResponse")]
        OwensCorning.Locator.Data.Dealer[] GetMasonryDealers(string zipCode, int radius);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocator/GetMasonryInstallers", ReplyAction="http://tempuri.org/ILocator/GetMasonryInstallersResponse")]
        OwensCorning.Locator.Data.Contractor[] GetMasonryInstallers(string zipCode, int radius);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface ILocatorChannel : OwensCorning.Utility.LocatorServices.ILocator, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class LocatorClient : System.ServiceModel.ClientBase<OwensCorning.Utility.LocatorServices.ILocator>, OwensCorning.Utility.LocatorServices.ILocator {
        
        public LocatorClient() {
        }
        
        public LocatorClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LocatorClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LocatorClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LocatorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public object[] GetLocatorResults(OwensCorning.Locator.Data.LocatorSearchOptions searchOptions) {
            return base.Channel.GetLocatorResults(searchOptions);
        }
        
        public OwensCorning.Locator.Data.Contractor GetContractorWithForeignId(string foreignId) {
            return base.Channel.GetContractorWithForeignId(foreignId);
        }
        
        public OwensCorning.Locator.Data.Contractor GetContractorWithEmail(string email) {
            return base.Channel.GetContractorWithEmail(email);
        }
        
        public OwensCorning.Locator.Data.Contractor[] GetContractors(OwensCorning.Locator.Data.LocatorSearchOptions searchOptions) {
            return base.Channel.GetContractors(searchOptions);
        }
        
        public OwensCorning.Locator.Data.Dealer[] GetRoofingDealers(string zipCode, int radius) {
            return base.Channel.GetRoofingDealers(zipCode, radius);
        }
        
        public OwensCorning.Locator.Data.Contractor[] GetRoofingInstallers(string zipCode, int radius) {
            return base.Channel.GetRoofingInstallers(zipCode, radius);
        }
        
        public OwensCorning.Locator.Data.Builder[] GetBuilders(string zipCode, int radius) {
            return base.Channel.GetBuilders(zipCode, radius);
        }
        
        public OwensCorning.Locator.Data.Dealer[] GetMasonryDealers(string zipCode, int radius) {
            return base.Channel.GetMasonryDealers(zipCode, radius);
        }
        
        public OwensCorning.Locator.Data.Contractor[] GetMasonryInstallers(string zipCode, int radius) {
            return base.Channel.GetMasonryInstallers(zipCode, radius);
        }
    }
}