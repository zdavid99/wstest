using System;
using System.Collections.Generic;
using System.Text;
using System.Data;





namespace OwensCorning.ContactService.Data
{
	/// <summary>
	/// Used by the status page to do a basic check on DAO's
	/// </summary>
	public interface IStatusMonitoringDao
    {
		int RecordCountTest { get; }
		String Name { get; }
		Boolean IsPass { get; }
	}
}
