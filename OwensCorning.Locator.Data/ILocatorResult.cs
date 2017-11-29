using System;
using System.Collections;
namespace OwensCorning.Locator.Data
{
    public interface ILocatorResult
    {
        string Address { get; set; }
        string Address2 { get; set; }
        string City { get; set; }
        string Company { get; set; }
        string ContactName { get; set; }
        double Distance { get; set; }
        int DistanceAsRoundedInt { get; }
        string Email { get; set; }
        string Fax { get; set; }
        int Id { get; set; }
        LocatorBusinessTypes BusinessType { get; }
        string BusinessTypeName { get; }
        LocatorResultTypes LocatorResultType { get; }
        string Phone { get; set; }
        string State { get; set; }
        string Website { get; set; }
        string Zip { get; set; }
        string ToString();
    }
}
