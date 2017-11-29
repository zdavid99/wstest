using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

//Custom References
using OwensCorning.RoofingServices.Interfaces;
using OwensCorning.RoofingServices.Classes;
using OwensCorning.RoofingServices.Database.dao;

namespace OwensCorning.RoofingServices
{
	public class RoofingLeadsService : IRoofingLeadsService
	{
		#region [ Get Methods ]

		/// <summary>
		/// Returns a list of Roofing Leads matching the passed in Contractor member ID
		/// </summary>
		/// <param name="memberID">Unique member ID of a contractor</param>
		/// <returns>A list of matching roofing leads attributed to the passed in contractor</returns>
		public List<RoofingLead> GetLeadsByMemberID(string memberID)
		{
			return RoofingLeadsDAO.GetRoofingLeadByMemberID(memberID);
		}

		/// <summary>
		/// Returns a list of Roofing Leads matching the passed in Contractor member ID
		/// </summary>
		/// <param name="memberID">Unique member ID of a contractor</param>
		/// <param name="startDate">This parameter sets the beginning capture point for the results - so all results since [startDate]</param>
		/// <returns>A list of matching roofing leads attributed to the passed in contractor</returns>
		public List<RoofingLead> GetLeadsByMemberIDAndDate(string memberID, DateTime startDate)
		{
			return RoofingLeadsDAO.GetRoofingLeadByMemberIDAndDate(memberID, startDate);
		}


		public List<RoofingLead> GetAllRoofingLeadsByDate(DateTime startDate)
		{
			return RoofingLeadsDAO.GetAllRoofingLeadsByDate(startDate);
		}

		#endregion [ Get Methods ]
	}
}
