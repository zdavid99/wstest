using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using OwensCorning.Utility.Logging;

namespace OwensCorning.ContactService.Data
{
    public delegate Object SqlDataReaderHandler(SqlDataReader reader);

	/// <summary>
	/// Used to identify the database connection to use
	/// </summary>
	public enum Database
	{
        None = 0,
		OwensCorning = 1

	}

    public abstract class AbstractDao : IStatusMonitoringDao
    {
        private static ILogger log = LoggerFactory.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Gets a connection for the specified database
		/// </summary>
		/// <param name="db"></param>
		/// <returns></returns>
        protected static SqlConnection GetConnection(Database db)
		{
			switch (db)
			{
				case Database.OwensCorning:
					return new SqlConnection(GetConnectionString(db));
			}

			return null;
        }

		public static Boolean IsProductionConnectionStringConfigured(Database db)
		{
			switch (db)
			{
				case Database.OwensCorning:
					return (GetConnectionString(db).Contains("ocsql01.oc.iscgnet.com"));
			}

			return false;
		}

		protected static String GetConnectionString(Database db)
		{
			switch (db)
			{
				case Database.OwensCorning:
					return ConfigurationManager.ConnectionStrings["dao.owens.sql.connectionstring"].ConnectionString;
			}

			return null;
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        protected static void Close(SqlConnection connection, SqlCommand command, SqlDataReader reader)
        {
            if (reader != null) reader.Close();
            if (command != null) command.Dispose();
            if (connection != null) connection.Close();
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        protected static int ExecuteInsert(Database database, String sqlInsertCommand, params object[] parameters)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                log.Debug("ExecuteInsert: " + sqlInsertCommand);
                conn = GetConnection(database);
                conn.Open();
                cmd = new SqlCommand(sqlInsertCommand + "; SET @recordID = SCOPE_IDENTITY() ", conn);

                for (int i = 0; i < parameters.Length; i++)
                {
                    log.Debug("Param " + i + " = " + parameters[i]);
                    cmd.Parameters.Add(new SqlParameter("@" + i, parameters[i]));
                }
                SqlParameter recordIdParam = new SqlParameter("@recordID", SqlDbType.Int);
                recordIdParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(recordIdParam);

                cmd.ExecuteNonQuery();
                int returnValue = (int)recordIdParam.Value;
                log.Debug("ExecuteInsert returned: " + returnValue);
                return returnValue;
            }
            catch (Exception ex)
            {
                log.Error("Error executing sql: " + sqlInsertCommand, ex);
                throw new ApplicationException("Error executing sql: " + sqlInsertCommand, ex);
            }
            finally
            {
                Close(conn, cmd, null);
            }
        }

		protected static int ExecuteNonQuery(Database database, string parameterizedSql, params object[] parameters)
        {
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                log.Debug("ExecuteNonQuery: " + parameterizedSql);
             	connection = GetConnection(database);
                connection.Open();
                command = new SqlCommand(parameterizedSql, connection);

                for (int i = 0; i < parameters.Length; i++)
                {
                    log.Debug("Param " + i + " = " + parameters[i]);
                    command.Parameters.Add(new SqlParameter("@" + i, parameters[i]));
                }
                
                int rowsAffected = command.ExecuteNonQuery();
                log.Debug("ExecuteNonQuery rows affected: " + rowsAffected);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                log.Error("Error executing sql: " + parameterizedSql, ex);
                throw new ApplicationException("Error executing sql: " + parameterizedSql, ex);
            }
            finally
            {
                Close(connection, command, null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        protected static T ExecuteQuery<T>(Database db, SqlDataReaderHandler handler, StringBuilder parameterizedSql, params object[] parameters)
		{
			// 30 seconds is default CommandTimeout value
			return ExecuteQuery<T>(db, handler, 30, parameterizedSql, parameters);
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        protected static T ExecuteQuery<T>(Database db, SqlDataReaderHandler handler, int commandTimeoutSeconds, StringBuilder parameterizedSql, params object[] parameters)
		{
			return ExecuteQuery<T>(db, handler, commandTimeoutSeconds, parameterizedSql.ToString(), parameters);
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        protected static T ExecuteQuery<T>(Database db, SqlDataReaderHandler handler, string parameterizedSql, params object[] parameters)
		{
			// 30 seconds is default CommandTimeout value
			return ExecuteQuery<T>(db, handler, 30, parameterizedSql, parameters);
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        protected static T ExecuteQuery<T>(Database db, SqlDataReaderHandler handler, int commandTimeoutSeconds, string parameterizedSql, params object[] parameters)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rdr = null;
            try
            {
                //log.Debug("ExecuteQuery: " + sql);
                conn = GetConnection(db);
                conn.Open();
                cmd = new SqlCommand(parameterizedSql, conn);
				cmd.CommandTimeout = commandTimeoutSeconds;

                for (int i = 0; i < parameters.Length; i++)
                {
                    //log.Debug("Param " + i + " = " + param[i]);
                    cmd.Parameters.Add(new SqlParameter("@" + i, parameters[i]));
                }

                rdr = cmd.ExecuteReader();
                return (T)handler(rdr);
            }
            catch (Exception ex)
            {
                log.Error("Error executing sql: " + parameterizedSql, ex);
                throw new ApplicationException("Error executing sql: " + parameterizedSql, ex);
            }
            finally
            {
                Close(conn, cmd, rdr);
            }

        }

		/// <summary>
		/// Counts the records in the specified table and database
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		protected static int CountTable(Database db, String tableName)
		{
			StringBuilder sql = new StringBuilder();
			int count = 0;

			sql.AppendLine(" select count(*) as total ");
			sql.AppendLine(" from " + tableName);

			SqlDataReaderHandler handler = delegate(SqlDataReader rdr)
			{
				if (rdr.HasRows)
				{
					if (rdr.Read())
					{
						count = rdr.GetInt32(rdr.GetOrdinal("total"));
					}
				}

				return count;
			};

			return ExecuteQuery<int>(db, handler, sql);
		}

		#region IStatusMonitoringDAO Members

		public abstract int RecordCountTest { get; }
		public abstract string Name { get; }
		public abstract Boolean IsPass { get; }

		#endregion
	}
}
