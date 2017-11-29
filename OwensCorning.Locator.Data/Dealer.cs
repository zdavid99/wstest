using System;
using System.Collections.Generic;
using System.Text;

namespace OwensCorning.Locator.Data
{
    [Flags]
    public enum DealerLevels
    {
        PlatinumElite = 16,
        Platinum = 8,
        Gold = 4,
        Silver = 2,
        None = 1,
        All = PlatinumElite | Platinum | Gold | Silver | None,
    }

    [Flags]
    public enum DealerPrograms
    { 
        //VALUES MUST BE POWERS OF 2
        None = 0,
        StocksRapidFlow = 1,
        StocksRoofing = 2,
        All = StocksRapidFlow | StocksRoofing
    }

    /// <summary>
    /// Represents a roofing dealer, a locator result who sells roofing supplies.
    /// </summary>
	[Serializable]
	public class Dealer : LocatorResult
    {
        public Dealer(LocatorBusinessTypes businessType)
            : base(LocatorResultTypes.Dealer, businessType)
        {
            DealerPrograms = DealerPrograms.None;
        }

        public DealerLevels DealerLevel { get; set; }
        public DealerPrograms DealerPrograms { get; set; }
    }
}
