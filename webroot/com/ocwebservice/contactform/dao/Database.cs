using System;
using System.Collections.Generic;
using System.Text;
using OwensCorning.Utility.Data;

namespace OwensCorning.ContactService.Data
{
    [Serializable]
    public class Database : DatabaseBase, IDatabase
    {
        #region Instance management
        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// Initializes the base instance, sets the default, and loads the enumeration members.
        /// </summary>
        protected Database() : base()
        {
            SetBaseInstance(this);
            AddDatabaseType(Database.OwensCorning);
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        /// <value>The instance.</value>
        public static IDatabase Instance
        {
            get
            {
                IDatabase instance = GetBaseInstance();
                if (instance == null)
                {
                    instance = new Database();
                }
                return instance;
            }
        }
        #endregion Instance management

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="audienceName">Name of the audience.</param>
        public Database(string databaseName, string connectionStringConfigValue, string productionServerHostname)
            : base(databaseName, connectionStringConfigValue, productionServerHostname)
        {
        }

        #region Enum Values
        private static readonly Database owenscorningDatabase = new Database("OwensCorning", "dao.owens.sql.connectionstring", "ocsql01.oc.iscgnet.com");
        /// <summary>
        /// Gets the enumeration member representing a homeowner.
        /// </summary>
        /// <value>The homeowner.</value>
        public static Database OwensCorning
        {
            get { return owenscorningDatabase; }
        }
        #endregion Enum Values
    }
}
