using System;
using System.Collections;
using System.Runtime.Serialization;

namespace OwensCorning.WebServices.ApplicationServices
{
	[KnownType(typeof(TransportSettingsProperty))]
	[CollectionDataContract]
	public class TransportSettingsPropertyCollection : CollectionBase
	{
		[DataMember]
		public TransportSettingsProperty this[int index]
		{
			get
			{
				return (TransportSettingsProperty)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

		public int Add(TransportSettingsProperty value)
		{
			return List.Add(value);
		}

		public int IndexOf(TransportSettingsProperty value)
		{
			return List.IndexOf(value);
		}

		public void Insert(int index, TransportSettingsProperty value)
		{
			List.Insert(index, value);
		}

		public void Remove(TransportSettingsProperty value)
		{
			List.Remove(value);
		}

		public bool Contains(TransportSettingsProperty value)
		{
			// If value is not of type SettingsProperty, this will return false.
			return List.Contains(value);
		}

		protected override void OnValidate(Object value)
		{
			if(value.GetType() != Type.GetType("OwensCorning.WebServices.ApplicationServices.TransportSettingsProperty"))
			{
				throw new ArgumentException("value must be of type OwensCorning.WebServices.ApplicationServices.TransportSettingsProperty.", "value");
			}
		}
	}
}
