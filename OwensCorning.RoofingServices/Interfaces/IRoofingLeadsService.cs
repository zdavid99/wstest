using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

//Custom References
using OwensCorning.RoofingServices.Classes;

namespace OwensCorning.RoofingServices.Interfaces
{
	[ServiceContract]
	public interface IRoofingLeadsService
	{
		[OperationContract]
		List<RoofingLead> GetLeadsByMemberID(String memberID);

		[OperationContract]
		List<RoofingLead> GetLeadsByMemberIDAndDate(String memberID, DateTime startDate);

		[OperationContract]
		List<RoofingLead> GetAllRoofingLeadsByDate(DateTime startDate);
	}
}
