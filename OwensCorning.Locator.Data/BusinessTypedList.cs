using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OwensCorning.Locator.Data
{
    /// <summary>
    /// Represents a collection of locator results for a particular business type.
    /// </summary>
	[Serializable]
	public class BusinessTypedList<T> 
        : List<T> where T : ILocatorResult
    {
        public LocatorBusinessTypes BusinessType { get; set; }

        public BusinessTypedList(LocatorBusinessTypes businessType)
            : base()
        {
            BusinessType = businessType;
        }

        public BusinessTypedList(LocatorBusinessTypes businessType, IEnumerable<T> collection)
            : base(collection)
        {
            BusinessType = businessType;
        }
    }
}
