using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;

namespace OwensCorning.WebServices.ApplicationServices
{
	public class MembershipProvider : IMembershipProvider
	{
		private const string _ApplicationName = "OwensCorning";

		private readonly System.Web.Security.MembershipProvider _Provider;

		private MembershipProvider()
		{
			_Provider = Membership.Providers["SqlMembershipProvider"];
		}

		#region IMembershipProvider Members
		public bool ChangePassword(string applicationName, string username, string oldPassword, string newPassword)
		{
			_Provider.ApplicationName = _ApplicationName;

			return _Provider.ChangePassword(username, oldPassword, newPassword);
		}

		public bool ChangePasswordQuestionAndAnswer(string applicationName, string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			_Provider.ApplicationName = _ApplicationName;

			return _Provider.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
		}

		public TransportMembershipUser CreateUser(string applicationName, string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			_Provider.ApplicationName = _ApplicationName;

			return ConvertUser(_Provider.CreateUser(username, password, email, passwordQuestion, passwordQuestion, isApproved, providerUserKey, out status));
		}

		public bool DeleteUser(string applicationName, string username, bool deleteAllRelatedData)
		{
			_Provider.ApplicationName = _ApplicationName;

			return _Provider.DeleteUser(username, deleteAllRelatedData);
		}

		public List<TransportMembershipUser> FindUsersByEmail(string applicationName, string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			_Provider.ApplicationName = _ApplicationName;

			return BuildUserList(_Provider.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords));
		}

		public List<TransportMembershipUser> FindUsersByName(string applicationName, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			_Provider.ApplicationName = _ApplicationName;

			return BuildUserList(_Provider.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords));
		}

		public List<TransportMembershipUser> GetAllUsers(string applicationName, int pageIndex, int pageSize, out int totalRecords)
		{
			_Provider.ApplicationName = _ApplicationName;

			return BuildUserList(_Provider.GetAllUsers(pageIndex, pageSize, out totalRecords));
		}

		public int GetNumberOfUsersOnline(string applicationName)
		{
			_Provider.ApplicationName = _ApplicationName;

			return _Provider.GetNumberOfUsersOnline();
		}

		public string GetPassword(string applicationName, string username, string answer)
		{
			_Provider.ApplicationName = _ApplicationName;

			return _Provider.GetPassword(username, answer);
		}

		public TransportMembershipUser GetUserByUsername(string applicationName, string username, bool userIsOnline)
		{
			_Provider.ApplicationName = _ApplicationName;

			var returnUser = ConvertUser(_Provider.GetUser(username, userIsOnline));

			return returnUser;
		}

		public TransportMembershipUser GetUserByProviderUserKey(string applicationName, object providerUserKey, bool userIsOnline)
		{
			_Provider.ApplicationName = _ApplicationName;

			return ConvertUser(_Provider.GetUser(providerUserKey, userIsOnline));
		}

		public string GetUserNameByEmail(string applicationName, string email)
		{
			_Provider.ApplicationName = _ApplicationName;

			return _Provider.GetUserNameByEmail(email);
		}

		public string ResetPassword(string applicationName, string username, string answer)
		{
			_Provider.ApplicationName = _ApplicationName;

			return _Provider.ResetPassword(username, answer);
		}

		public bool UnlockUser(string applicationName, string username)
		{
			_Provider.ApplicationName = _ApplicationName;

			return _Provider.UnlockUser(username);
		}

		public void UpdateUser(string applicationName, TransportMembershipUser user)
		{
			_Provider.ApplicationName = _ApplicationName;

			_Provider.UpdateUser(ConvertUser(user));
		}

		public bool ValidateUser(string applicationName, string username, string password)
		{
			_Provider.ApplicationName = _ApplicationName;

			return _Provider.ValidateUser(username, password);
		}

		public int RecordUserLoginHistory(string applicationName, string username)
		{
			using(var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
			{
				var getApplicationCommand = new SqlCommand("usp_GetApplicationIdByName", connection) {CommandType = CommandType.StoredProcedure};

				var insertUserLoginHistoryCommand = new SqlCommand("usp_InsertUserLoginHistory", connection) { CommandType = CommandType.StoredProcedure };

				try
				{
					getApplicationCommand.Parameters.Clear();
					getApplicationCommand.Parameters.Add(new SqlParameter { ParameterName = "ApplicationName", DbType = DbType.String, Size = 256, Value = _ApplicationName, Direction = ParameterDirection.Input });
					getApplicationCommand.Parameters.Add(new SqlParameter { ParameterName = "ApplicationId", DbType = DbType.Guid, Value = DBNull.Value, Direction = ParameterDirection.InputOutput });

					connection.Open();

					getApplicationCommand.ExecuteNonQuery();

					connection.Close();

					var applicationId = (Guid)getApplicationCommand.Parameters["ApplicationId"].Value;

					var membershipUser = GetUserByUsername(_ApplicationName, username, false);

					insertUserLoginHistoryCommand.Parameters.Clear();
					insertUserLoginHistoryCommand.Parameters.Add(new SqlParameter { ParameterName = "ApplicationId", DbType = DbType.Guid, Value = applicationId, Direction = ParameterDirection.Input });
					insertUserLoginHistoryCommand.Parameters.Add(new SqlParameter { ParameterName = "UserId", DbType = DbType.Guid, Value = membershipUser.ProviderUserKey, Direction = ParameterDirection.Input });
					insertUserLoginHistoryCommand.Parameters.Add(new SqlParameter { ParameterName = "SiteName", DbType = DbType.String, Size = 256, Value = applicationName, Direction = ParameterDirection.Input });
					insertUserLoginHistoryCommand.Parameters.Add(new SqlParameter { ParameterName = "UserLoginHistoryID", DbType = DbType.Int32, Value = DBNull.Value, Direction = ParameterDirection.InputOutput });

					connection.Open();

					insertUserLoginHistoryCommand.ExecuteNonQuery();

					connection.Close();

					return (int)insertUserLoginHistoryCommand.Parameters["UserLoginHistoryID"].Value;
				}
				finally
				{
					if(connection.State == ConnectionState.Open)
					{
						connection.Close();
					}

					getApplicationCommand.Dispose();

					insertUserLoginHistoryCommand.Dispose();
				}
			}
		}
		#endregion

		protected TransportMembershipUser ConvertUser(MembershipUser user)
		{
			if(user == null)
			{
				return null;
			}

			var returnUser = new TransportMembershipUser { Comment = user.Comment, CreationDate = user.CreationDate, Email = user.Email, IsApproved = user.IsApproved, IsLockedOut = user.IsLockedOut, IsOnline = user.IsOnline, LastActivityDate = user.LastActivityDate, LastLockoutDate = user.LastLockoutDate, LastLoginDate = user.LastLoginDate, LastPasswordChangedDate = user.LastPasswordChangedDate, PasswordQuestion = user.PasswordQuestion, ProviderName = user.ProviderName, ProviderUserKey = user.ProviderUserKey, UserName = user.UserName };

			return returnUser;
		}

		protected MembershipUser ConvertUser(TransportMembershipUser user)
		{
			if(user == null)
			{
				return null;
			}

			var returnUser = new MembershipUser(user.ProviderName, user.UserName, user.ProviderUserKey, user.Email, user.PasswordQuestion, user.Comment, user.IsApproved, user.IsLockedOut, user.CreationDate, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate, user.LastLockoutDate);

			return returnUser;
		}

		protected List<TransportMembershipUser> BuildUserList(MembershipUserCollection userCollection)
		{
			if(userCollection == null)
			{
				return null;
			}

			var userlist = new List<TransportMembershipUser>();

			foreach(MembershipUser user in userCollection)
			{
				userlist.Add(ConvertUser(user));
			}

			return userlist;
		}
	}
}