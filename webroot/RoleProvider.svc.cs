using System.Web.Security;

namespace OwensCorning.WebServices.ApplicationServices
{
	public class RoleProvider : IRoleProvider
	{
		private readonly System.Web.Security.RoleProvider _Provider;
        private const string _ApplicationName = "OwensCorning";

        private RoleProvider()
		{
			_Provider = Roles.Providers["SqlRoleProvider"];
		}

		#region IRoleProvider Members
		public void AddUsersToRoles(string applicationName, string[] usernames, string[] roleNames)
		{
            _Provider.ApplicationName = _ApplicationName;

			_Provider.AddUsersToRoles(usernames, roleNames);
		}

		public void CreateRole(string applicationName, string roleName)
		{
            _Provider.ApplicationName = _ApplicationName;

			_Provider.CreateRole(roleName);
		}

		public bool DeleteRole(string applicationName, string roleName, bool throwOnPopulatedRole)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.DeleteRole(roleName, throwOnPopulatedRole);
		}

		public string[] FindUsersInRole(string applicationName, string roleName, string usernameToMatch)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.FindUsersInRole(roleName, usernameToMatch);
		}

		public string[] GetAllRoles(string applicationName)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.GetAllRoles();
		}

		public string[] GetRolesForUser(string applicationName, string username)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.GetRolesForUser(username);
		}

		public string[] GetUsersInRole(string applicationName, string roleName)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.GetUsersInRole(roleName);
		}

		public bool IsUserInRole(string applicationName, string username, string roleName)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.IsUserInRole(username, roleName);
		}

		public void RemoveUsersFromRoles(string applicationName, string[] usernames, string[] roleNames)
		{
            _Provider.ApplicationName = _ApplicationName;

			_Provider.RemoveUsersFromRoles(usernames, roleNames);
		}

		public bool RoleExists(string applicationName, string roleName)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.RoleExists(roleName);
		}
		#endregion
	}
}