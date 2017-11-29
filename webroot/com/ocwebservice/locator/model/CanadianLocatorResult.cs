using System;
using System.Collections.Generic;
using System.Text;
using OwensCorning.Locator.Data;

namespace com.ocwebservice.locator.model
{
    /// <summary>
    /// Used to identify the database connection to use
    /// </summary>
    public enum DealerType
    {
        Dealer = 1
    }

    [Serializable]
    public class CanadianLocatorResult 
    {
        public CanadianLocatorResult(DealerType dealerType)
        {
            this.dealerType = dealerType;
        }

        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private string address;
        public string Address
        {
            get { return this.address; }
            set { this.address = value; }
        }

        private string city;
        public string City
        {
            get { return this.city; }
            set { this.city = value; }
        }

        private string province;
        public string Province
        {
            get { return this.province; }
            set { this.province = value; }
        }

        private string postalCode;
        public string PostalCode
        {
            get { return this.postalCode; }
            set { this.postalCode = value; }
        }

        private string phone;
        public string Phone
        {
            get { return this.phone; }
            set { this.phone = value; }
        }

        private string website;
        public string Website
        {
            get { return this.website; }
            set { this.website = value; }
        }

        private decimal distance;
        public decimal Distance
        {
            get { return this.distance; }
            set { this.distance = value; }
        }

        public int DistanceAsRoundedInt
        {
            get { return Convert.ToInt32(Math.Round(Math.Ceiling(Distance), 0)); }
        }

        private readonly DealerType dealerType;
        public DealerType DealerType
        {
            get { return dealerType; }
        }
    }
}
