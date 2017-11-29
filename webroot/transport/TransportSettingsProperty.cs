using System.Configuration;
using System.Runtime.Serialization;

namespace OwensCorning.WebServices.ApplicationServices
{
	[DataContract]
	public class TransportSettingsProperty
	{
		[DataMember]
		public SettingsAttributeDictionary Attributes { get; set; }

		[DataMember]
		public object DefaultValue { get; set; }

		[DataMember]
		public bool IsReadOnly { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PropertyType { get; set; }

		[DataMember]
		public SettingsSerializeAs SerializeAs { get; set; }

		[DataMember]
		public bool ThrowOnErrorDeserializing { get; set; }

		[DataMember]
		public bool ThrowOnErrorSerializing { get; set; }
	}
}
