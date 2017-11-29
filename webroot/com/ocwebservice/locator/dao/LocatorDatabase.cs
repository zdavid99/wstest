using System;
using System.Collections.Generic;
using System.Text;
using OwensCorning.Utility.Data;

namespace com.ocwebservice.locator.dao
{
    [Serializable]
    public class LocatorDatabase : DatabaseBase, IDatabase
    {
		#region Instance management
		/// <summary>
		/// Initializes a new instance of the <see cref="Database"/> class.
		/// Initializes the base instance, sets the default, and loads the enumeration members.
		/// </summary>
		protected LocatorDatabase()
		{
			SetBaseInstance(this);

			AddDatabaseType(Canada);
			AddDatabaseType(Common);
		}

		/// <summary>
		/// Gets the single instance of this class.
		/// </summary>
		/// <value>The instance.</value>
		public static IDatabase Instance
		{
			get
			{
				var instance = GetBaseInstance() ?? new LocatorDatabase();
				return instance;
			}
		}
		#endregion Instance management

        public LocatorDatabase(string DatabaseName, string connectionStringConfigKey, string productionServerHostname)
			: base(DatabaseName, connectionStringConfigKey, productionServerHostname) {}

		#region Enum Values
        private static readonly LocatorDatabase canadaDatabase = new LocatorDatabase("OCCanadianInsulation", "OwensCorning.Locator.Canada", "ocsql02.oc.iscgnet.com");
        private static readonly LocatorDatabase commonDatabase = new LocatorDatabase("Common", "OwensCorning.Common", "ocsql02.oc.iscgnet.com");
        private static readonly LocatorDatabase owenscorningDatabase = new LocatorDatabase("OwensCorning", "owc_owensConnectionString", "ocsql01.oc.iscgnet.com");

		/// <summary>
		/// The Owens Corning common data DB
		/// </summary>
		/// <value>Owens Corning common data DB</value>
        public static LocatorDatabase Common
		{
			get
			{
				return commonDatabase;
			}
		}

		/// <summary>
		/// The CulturedStone DB
		/// </summary>
		/// <value>CulturedStone DB</value>
        public static LocatorDatabase Canada
		{
			get
			{
				return canadaDatabase;
			}
		}

        /// <summary>
        /// Gets the enumeration member representing the Owens Corning database.
        /// </summary>
        /// <value>The homeowner.</value>
        public static LocatorDatabase OwensCorning
        {
            get { return owenscorningDatabase; }
        }
		#endregion
	}
}
