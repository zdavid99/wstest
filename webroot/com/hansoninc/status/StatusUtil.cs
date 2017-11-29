using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using OwensCorning.ContactService.Data;
using OwensCorning.Utility.Status;

namespace com.hansoninc.status
{
	public class StatusUtil
    {
		/// <summary>
		/// This provides the DAO list for the status page
		/// </summary>
		/// <returns></returns>
		public static IList<IStatusMonitoringDAO> GetServiceList()
		{
            IList<IStatusMonitoringDAO> list = new List<IStatusMonitoringDAO>();
            list.Add((IStatusMonitoringDAO)ContactFormDao.Instance);
			return list;
		}
	}
}
