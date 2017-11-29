using System;
using System.Collections;
using System.Runtime.Serialization;

namespace OwensCorning.WebServices.ApplicationServices
{
	[KnownType(typeof(TransportSettingsPropertyValue))]
	[CollectionDataContract]
	public class TransportSettingsPropertyValueCollection : CollectionBase
	{
        [DataMember]
        public TransportSettingsPropertyValue this[int index]
		{
			get
			{
				return (TransportSettingsPropertyValue)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

		public int Add(TransportSettingsPropertyValue value)
		{
			return List.Add(value);
		}

		public int IndexOf(TransportSettingsPropertyValue value)
		{
			return List.IndexOf(value);
		}

		public void Insert(int index, TransportSettingsPropertyValue value)
		{
			List.Insert(index, value);
		}

		public void Remove(TransportSettingsPropertyValue value)
		{
			List.Remove(value);
		}

		public bool Contains(TransportSettingsPropertyValue value)
		{
			// If value is not of type SettingsPropertyValue, this will return false.
			return List.Contains(value);
		}

		protected override void OnValidate(Object value)
		{
			if(value.GetType() != Type.GetType("OwensCorning.WebServices.ApplicationServices.TransportSettingsPropertyValue"))
			{
				throw new ArgumentException("value must be of type OwensCorning.WebServices.ApplicationServices.TransportSettingsPropertyValue.", "value");
			}
		}
	}
}
