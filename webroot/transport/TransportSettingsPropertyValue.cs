using System.Runtime.Serialization;

namespace OwensCorning.WebServices.ApplicationServices
{
	[KnownType(typeof(TransportSettingsProperty))]
    [KnownType(typeof(System.Collections.Generic.Dictionary<string,string>))]
	[DataContract]
	public class TransportSettingsPropertyValue
	{
        [DataMember]
		public bool Deserialized {get;set;}

        [DataMember]
		public bool IsDirty {get;set;}

		[DataMember]
		public TransportSettingsProperty Property {get;set;}

		[DataMember]
		public object PropertyValue { get;set; }

		[DataMember]
		public object SerializedValue { get;set; }
	}
}
