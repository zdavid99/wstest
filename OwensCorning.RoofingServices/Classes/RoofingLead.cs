using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace OwensCorning.RoofingServices.Classes
{
	/// <summary>
	/// Data container for the Leads Web Service that drives OCProconnect.com
	/// </summary>
	[DataContract]
	public struct RoofingLead
	{
		#region [ Members ]

		private String m_MemberID;
		private String m_Name;
		private String m_Address;
		private String m_City;
		private String m_State;
		private String m_Zip;
		private String m_EmailAddress;
		private String m_PhoneNumber;
		private String m_LeadDescription;
		private DateTime? m_LeadDate;
		private String m_BestTimeToCall;

		#endregion [ Members ]

		#region [ Properties ]

		[DataMember]
		public String MemberID { get; set; }
		[DataMember]
		public String Name { get; set; }
		[DataMember]
		public String Address { get; set; }
		[DataMember]
		public String City { get; set; }
		[DataMember]
		public String State { get; set; }
		[DataMember]
		public String Zip { get; set; }
		[DataMember]
		public String EmailAddress { get; set; }
		[DataMember]
		public String PhoneNumber { get; set; }
		[DataMember]
		public String LeadDescription { get; set; }
		[DataMember]
		public DateTime? LeadDate { get; set; }
		[DataMember]
		public String BestTimeToCall { get; set; }

		#endregion [ Properties ]
	}
}