using System;
using System.ServiceModel;
using System.Web.Profile;

namespace OwensCorning.WebServices.ApplicationServices
{
	// NOTE: If you change the interface name "IProfileProvider" here, you must also update the reference to "IProfileProvider" in Web.config.
	[ServiceContract]
	public interface IProfileProvider
	{
		[OperationContract]
		int DeleteProfiles(string applicationName, ProfileInfoCollection profiles);

		[OperationContract]
		int DeleteProfilesByUsernames(string applicationName, string[] usernames);

		[OperationContract]
		int DeleteInactiveProfiles(string applicationName, ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate);

		[OperationContract]
		int GetNumberOfInactiveProfiles(string applicationName,  ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate);

		[OperationContract]
		ProfileInfoCollection GetAllProfiles(string applicationName,  ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords);

		[OperationContract]
		ProfileInfoCollection GetAllInactiveProfiles(string applicationName,  ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords);

		[OperationContract]
		ProfileInfoCollection FindProfilesByUserName(string applicationName,  ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords);

		[OperationContract]
		ProfileInfoCollection FindInactiveProfilesByUserName(string applicationName,  ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords);

		[OperationContract]
		TransportSettingsPropertyValueCollection GetPropertyValues(string applicationName, TransportSettingsContext context, TransportSettingsPropertyCollection collection);

		[OperationContract]
		void SetPropertyValues(string applicationName, TransportSettingsContext context, TransportSettingsPropertyValueCollection collection);
	}
}
