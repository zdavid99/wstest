using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

namespace OwensCorning.Locator.Data
{
    /// <summary>
    /// Used to identify the type of dealer to retrieve
    /// </summary>
    [Flags]
    public enum LocatorResultTypes
    {
        None = 0,
        Dealer = 1,
        Installer = 2,
        Builder = 4
    }

    /// <summary>
    /// Used to identify where the search should be performed
    /// </summary>
    [Flags]
    public enum LocatorBusinessTypes
    {
        None = 0,
        Roofing = 1,
        Masonry = 2,
        ResidentialInsulation = 4,
        All = Roofing | Masonry | ResidentialInsulation
    }

    /// <summary>
    /// Represents a third party who sells or uses Owens Corning products.
    /// </summary>
    [Serializable]
	public abstract class LocatorResult : ILocatorResult
    {
        public LocatorResult() :
            this(LocatorResultTypes.Dealer, LocatorBusinessTypes.Roofing)
        {
        }

        public bool IsBusinessTypeOf(LocatorBusinessTypes constant)
        {
            return (this.locatorBusinessType & constant) == constant;
        }

        public LocatorResult(LocatorResultTypes locatorResultType, LocatorBusinessTypes locatorBusinessType)
        {
            this.locatorResultType = locatorResultType;
            this.locatorBusinessType = locatorBusinessType;
        }

        private readonly LocatorResultTypes locatorResultType;
        public LocatorResultTypes LocatorResultType
        {
            get { return locatorResultType; }
        }

        private readonly LocatorBusinessTypes locatorBusinessType;
        /// <summary>
        /// Gets the business type of the locator result.
        /// </summary>
        /// <value>The business type of the locator result.</value>
        public LocatorBusinessTypes BusinessType
        {
            get { return locatorBusinessType; }
        }

        /// <summary>
        /// Gets the name of the business type.
        /// </summary>
        /// <value>The name of the business type.</value>
        public string BusinessTypeName
        {
            get { return Enum.GetName(typeof(LocatorBusinessTypes), BusinessType); }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the contact.
        /// </summary>
        /// <value>The name of the contact.</value>
        public string ContactName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>The company.</value>
        public string Company
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second line of the address.
        /// </summary>
        /// <value>The second line of the address.</value>
        public String Address2
        {
            get;
            set;
        }

        public string City
        {
            get;
            set;
        }

        public string State
        {
            get;
            set;
        }

        public string Zip
        {
            get;
            set;
        }

        public string Phone
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the fax number.
        /// </summary>
        /// <value>The fax number.</value>
        public String Fax
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the web site.
        /// </summary>
        /// <value>The web site.</value>
        public string Website
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the distance.
        /// </summary>
        /// <value>The distance.</value>
        public double Distance
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the distance as rounded int.
        /// </summary>
        /// <value>The distance as rounded int.</value>
        public int DistanceAsRoundedInt
        {
            get { return Convert.ToInt32(Math.Round(Math.Ceiling(Distance), 0)); }
        }

        public override string ToString()
        {
            return BusinessType + ":" + LocatorResultType + ":" + Company + ":" + Zip + ":" + Distance;
        }
    }
}
