using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OwensCorning.Locator.Data
{
    /// <summary>
    /// Represents a roofing contractor, a locator result who installs and services roofs for existing houses.
    /// </summary>
	[Serializable]
	public class Contractor : LocatorResult
    {
        public Contractor(LocatorBusinessTypes businessType)
            : this(LocatorResultTypes.Installer, businessType)
        {
        }

        protected Contractor(LocatorResultTypes locatorResultType, LocatorBusinessTypes locatorBusinessType)
            : base(locatorResultType, locatorBusinessType)
        {
            ContractorPrograms = ContractorPrograms.None;
        }

        public ContractorPrograms ContractorPrograms { get; set; }

        #region Account Sales Manager Info
        /// <summary>
        /// Gets or sets the first name of the account sales manager.
        /// </summary>
        /// <value>The first name of the account sales manager.</value>
        public string AsmFirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last name of the account sales manager.
        /// </summary>
        /// <value>The last name of the account sales manager.</value>
        public string AsmLastName
        {
            get;
            set;
        }        
        
        /// <summary>
        /// Gets the full name of the account sales manager.
        /// </summary>
        /// <value>The full name of the account sales manager.</value>
        public string AsmFullName
        {
            get { return AsmFirstName + " " + AsmLastName; }
        }
        
        /// <summary>
        /// Gets or sets the account sales manager's email.
        /// </summary>
        /// <value>The account sales manager's email.</value>
        public string AsmEmail
        {
            get;
            set;
        }
        #endregion ASM Info
    }

    [Flags]
    public enum ContractorPrograms
    { 
        //VALUES MUST BE POWERS OF 2
        None = 0,
        HasProConnectProfile = 1,
        IsExteriorFx = 2,
        IsPreferred = 4,
        IsPreferredPride = 8,
        IsPlatinumPreferred = 16,
        IsTopOfTheHouse = 32,
        IsPlatinumAwardWinner = 64,
        All = IsExteriorFx | IsPlatinumAwardWinner | HasProConnectProfile |
        IsPreferred | IsPreferredPride | IsTopOfTheHouse | IsPlatinumPreferred
    }
}
