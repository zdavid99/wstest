using System.ServiceModel;

namespace OwensCorning.WebServices.ApplicationServices
{
	[ServiceContract]
	public interface IRoleProvider
	{
		[OperationContract]
		void AddUsersToRoles(string applicationName, string[] usernames, string[] roleNames);

		[OperationContract]
		void CreateRole(string applicationName, string roleName);

		[OperationContract]
		bool DeleteRole(string applicationName, string roleName, bool throwOnPopulatedRole);

		[OperationContract]
		string[] FindUsersInRole(string applicationName, string roleName, string usernameToMatch);

		[OperationContract]
		string[] GetAllRoles(string applicationName);

		[OperationContract]
		string[] GetRolesForUser(string applicationName, string username);

		[OperationContract]
		string[] GetUsersInRole(string applicationName, string roleName);

		[OperationContract]
		bool IsUserInRole(string applicationName, string username, string roleName);

		[OperationContract]
		void RemoveUsersFromRoles(string applicationName, string[] usernames, string[] roleNames);

		[OperationContract]
		bool RoleExists(string applicationName, string roleName);
	}
}
