using System;
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

namespace OwensCorning.RoofingServices.Database.data
{
	public class RoofingLeadDataSource
	{
		/// <summary>
		/// Builds a database context connection for LINQ to SQL functionality
		/// </summary>
		/// <returns>LINQ to SQL data context object</returns>
		public static RoofingLeadsInformationDataContext RoofingLeadsInformationDataContext()
		{
			ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings["OwensCorning.Roofing"];
			RoofingLeadsInformationDataContext roofingLeadsRepository = new RoofingLeadsInformationDataContext(connectionSettings.ConnectionString);
			return roofingLeadsRepository;
		}
	}
}
