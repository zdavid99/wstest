using System.Runtime.Serialization;

namespace OwensCorning.WebServices.ApplicationServices
{
	[DataContract]
	public class TransportSettingsContext
	{
		[DataMember]
		public string Username { get;set; }

		[DataMember]
		public bool IsAuthenticated { get;set; }
	}
}
