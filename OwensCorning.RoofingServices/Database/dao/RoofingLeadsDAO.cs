using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

//Custom References
using OwensCorning.Utility.Data;
using OwensCorning.Utility.Logging;
using OwensCorning.Utility.Status;
using OwensCorning.RoofingServices.Classes;
using OwensCorning.RoofingServices.Database.data;

namespace OwensCorning.RoofingServices.Database.dao
{
	public class RoofingLeadsDAO : AbstractDAO
	{
		#region [ Members ]

		private static ILogger log = LoggerFactory.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static RoofingLeadsDAO self = new RoofingLeadsDAO();

		#endregion [ Members ]

		#region [ Constructors ]

		private RoofingLeadsDAO()
		{ }

		#endregion [ Constructors ]

		#region [ Properties ]

		public static RoofingLeadsDAO Instance
		{
			get { return self; }
		}

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>
		/// Returns a list of Roofing Leads matching the passed in Contractor member ID
		/// </summary>
		/// <param name="memberID">Unique member ID of a contractor</param>
		/// <returns>A list of matching roofing leads attributed to the passed in contractor</returns>
		public static List<RoofingLead> GetRoofingLeadByMemberID(String memberID)
		{
			List<RoofingLead> returnList = new List<RoofingLead>();

			try
			{
				using (RoofingLeadsInformationDataContext context = RoofingLeadDataSource.RoofingLeadsInformationDataContext())
				{
					returnList.AddRange((from source in context.RfgRequestQuotes
										 where source.forwardedToIds.Contains(memberID)
										 select (new RoofingLead
										 {
											 MemberID = memberID,
											 Address = source.address,
											 BestTimeToCall = source.besttimetocall,
											 City = source.city,
											 EmailAddress = source.email,
											 LeadDate = source.date,
											 LeadDescription = source.description,
											 Name = source.name,
											 PhoneNumber = source.phone,
											 State = source.state,
											 Zip = source.zip
										 })).ToList());

					return returnList;
				}
			}
			catch (Exception ex)
			{
				log.Error(ex.Message, ex);
			}

			return returnList;
		}

		/// <summary>
		/// Returns a list of Roofing Leads matching the passed in Contractor member ID
		/// </summary>
		/// <param name="memberID">Unique member ID of a contractor</param>
		/// <param name="startDate">This parameter sets the beginning capture point for the results - so all results since [startDate]</param>
		/// <returns>A list of matching roofing leads attributed to the passed in contractor</returns>
		public static List<RoofingLead> GetRoofingLeadByMemberIDAndDate(String memberID, DateTime startDate)
		{
			List<RoofingLead> returnList = new List<RoofingLead>();

			try
			{
				using (RoofingLeadsInformationDataContext context = RoofingLeadDataSource.RoofingLeadsInformationDataContext())
				{
					returnList.AddRange((from source in context.RfgRequestQuotes
										 where source.forwardedToIds.Contains(memberID) && source.date >= startDate
										 select (new RoofingLead
										 {
											 MemberID = memberID,
											 Address = source.address,
											 BestTimeToCall = source.besttimetocall,
											 City = source.city,
											 EmailAddress = source.email,
											 LeadDate = source.date,
											 LeadDescription = source.description,
											 Name = source.name,
											 PhoneNumber = source.phone,
											 State = source.state,
											 Zip = source.zip
										 })).ToList());

					return returnList;
				}
			}
			catch (Exception ex)
			{
				log.Error(ex.Message, ex);
			}

			return returnList;
		}

		/// <summary>
		/// Returns a list of Roofing Leads that have been added to the database since the passed in date
		/// </summary>
		/// <param name="startDate">This parameter sets the beginning capture point for the results - so all results since [startDate]</param>
		/// <returns>A list of all roofing leads since the given date</returns>
		public static List<RoofingLead> GetAllRoofingLeadsByDate(DateTime startDate)
		{
			List<RoofingLead> returnList = new List<RoofingLead>();

			try
			{
				using (RoofingLeadsInformationDataContext context = RoofingLeadDataSource.RoofingLeadsInformationDataContext())
				{
					var tempList = (from source in context.RfgRequestQuotes
									where source.date >= startDate
									select (new RoofingLead
									{
										MemberID = source.forwardedToIds.TrimEnd(';'),
										Address = source.address,
										BestTimeToCall = source.besttimetocall,
										City = source.city,
										EmailAddress = source.email,
										LeadDate = source.date,
										LeadDescription = source.description,
										Name = source.name,
										PhoneNumber = source.phone,
										State = source.state,
										Zip = source.zip
									})).ToList();

					foreach (RoofingLead lead in tempList)
					{
						var memberIds = lead.MemberID.Split(';');
						if (memberIds.Length > 1)
						{
							foreach (String memberId in memberIds)
							{
								returnList.Add(new RoofingLead
										 {
											 MemberID = memberId,
											 Address = lead.Address,
											 BestTimeToCall = lead.BestTimeToCall,
											 City = lead.City,
											 EmailAddress = lead.EmailAddress,
											 LeadDate = lead.LeadDate,
											 LeadDescription = lead.LeadDescription,
											 Name = lead.Name,
											 PhoneNumber = lead.PhoneNumber,
											 State = lead.State,
											 Zip = lead.Zip
										 });
							}
						}
						else
						{
							returnList.Add(new RoofingLead
							 {
								 MemberID = memberIds[0],
								 Address = lead.Address,
								 BestTimeToCall = lead.BestTimeToCall,
								 City = lead.City,
								 EmailAddress = lead.EmailAddress,
								 LeadDate = lead.LeadDate,
								 LeadDescription = lead.LeadDescription,
								 Name = lead.Name,
								 PhoneNumber = lead.PhoneNumber,
								 State = lead.State,
								 Zip = lead.Zip
							 });
						}
					}

					#region [ Depricated ]

					//returnList.AddRange((from source in context.RfgRequestQuotes
					//                     where source.date >= startDate
					//                     select (new RoofingLead
					//                     {
					//                         MemberID = source.forwardedToIds,
					//                         Address = source.address,
					//                         BestTimeToCall = source.besttimetocall,
					//                         City = source.city,
					//                         EmailAddress = source.email,
					//                         LeadDate = source.date,
					//                         LeadDescription = source.description,
					//                         Name = source.name,
					//                         PhoneNumber = source.phone,
					//                         State = source.state,
					//                         Zip = source.zip
					//                     })).ToList());

					#endregion [ Depricated ]

					return returnList;
				}
			}
			catch (Exception ex)
			{
				log.Error(ex.Message, ex);
			}

			return returnList;
		}

		#endregion [ Methods ]

		#region IStatusMonitoringDAO Members

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public override int RecordCountTest
		{
			get
			{
				int count = 0;
				try
				{
					count = CountTable(Database.OwensCorning, "RfgRequestQuote");
				}
				catch (Exception ex)
				{
					log.Fatal("Monitoring: " + this.Name + " failed RecordCountTest.", ex);
				}
				return count;
			}
		}

		public override string Name
		{
			get { return this.GetType().ToString(); }
		}

		public override bool IsPass
		{
			get { return (RecordCountTest > 0); }
		}

		#endregion
	}
}