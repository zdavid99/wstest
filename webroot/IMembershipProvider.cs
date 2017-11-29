using System.Collections.Generic;
using System.ServiceModel;

namespace OwensCorning.WebServices.ApplicationServices
{
	[ServiceContract]
	public interface IMembershipProvider
	{
		[OperationContract]
		bool ChangePassword(string applicationName, string username, string oldPassword, string newPassword);

		[OperationContract]
		bool ChangePasswordQuestionAndAnswer(string applicationName, string username, string password, string newPasswordQuestion, string newPasswordAnswer);

		[OperationContract]
		TransportMembershipUser CreateUser(string applicationName, string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status);

		[OperationContract]
		bool DeleteUser(string applicationName, string username, bool deleteAllRelatedData);

		[OperationContract]
		List<TransportMembershipUser> FindUsersByEmail(string applicationName, string emailToMatch, int pageIndex, int pageSize, out int totalRecords);

		[OperationContract]
		List<TransportMembershipUser> FindUsersByName(string applicationName, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords);

		[OperationContract]
		List<TransportMembershipUser> GetAllUsers(string applicationName, int pageIndex, int pageSize, out int totalRecords);

		[OperationContract]
		int GetNumberOfUsersOnline(string applicationName);

		[OperationContract]
		string GetPassword(string applicationName, string username, string answer);

		[OperationContract]
		TransportMembershipUser GetUserByUsername(string applicationName, string username, bool userIsOnline);

		[OperationContract]
		TransportMembershipUser GetUserByProviderUserKey(string applicationName, object providerUserKey, bool userIsOnline);

		[OperationContract]
		string GetUserNameByEmail(string applicationName, string email);

		[OperationContract]
		string ResetPassword(string applicationName, string username, string answer);

		[OperationContract]
		bool UnlockUser(string applicationName, string username);

		[OperationContract]
		void UpdateUser(string applicationName, TransportMembershipUser user);

		[OperationContract]
		bool ValidateUser(string applicationName, string username, string password);

		[OperationContract]
		int RecordUserLoginHistory(string applicationName, string username);
	}
}
