using System;
using System.Runtime.Serialization;

namespace OwensCorning.WebServices.ApplicationServices
{
	[DataContract]
	public class TransportMembershipUser
	{
		[DataMember]
		public string Comment{get;set;}

		[DataMember]
		public DateTime CreationDate{get;set;}

		[DataMember]
		public string Email{get;set;}

		[DataMember]
		public bool IsApproved{get;set;}

		[DataMember]
		public bool IsLockedOut{get;set;}

		[DataMember]
		public bool IsOnline{get;set;}

		[DataMember]
		public DateTime LastActivityDate{get;set;}

		[DataMember]
		public DateTime LastLockoutDate{get;set;}

		[DataMember]
		public DateTime LastLoginDate{get;set;}

		[DataMember]
		public DateTime LastPasswordChangedDate{get;set;}

		[DataMember]
		public string PasswordQuestion{get;set;}

		[DataMember]
		public string ProviderName{get;set;}

		[DataMember]
		public object ProviderUserKey{get;set;}

		[DataMember]
		public string UserName{get;set;}
	}
}
