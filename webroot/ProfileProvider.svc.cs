using System;
using System.Configuration;
using System.Web.Profile;

namespace OwensCorning.WebServices.ApplicationServices
{
	public class ProfileProvider : IProfileProvider
	{
		private readonly System.Web.Profile.ProfileProvider _Provider;
        private const string _ApplicationName = "OwensCorning";

        private ProfileProvider()
		{
			_Provider = ProfileManager.Providers["SqlProfileProvider"];
		}

		#region IProfileProvider Members
		
		public int DeleteProfiles(string applicationName, ProfileInfoCollection profiles)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.DeleteProfiles(profiles);
		}

		public int DeleteProfilesByUsernames(string applicationName, string[] usernames)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.DeleteProfiles(usernames);
		}

		public int DeleteInactiveProfiles(string applicationName, ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.DeleteInactiveProfiles(authenticationOption, userInactiveSinceDate);
		}

		public int GetNumberOfInactiveProfiles(string applicationName, ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.GetNumberOfInactiveProfiles(authenticationOption, userInactiveSinceDate);
		}

		public ProfileInfoCollection GetAllProfiles(string applicationName, ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.GetAllProfiles(authenticationOption, pageIndex, pageSize, out totalRecords);
		}

		public ProfileInfoCollection GetAllInactiveProfiles(string applicationName, ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.GetAllInactiveProfiles(authenticationOption, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
		}

		public ProfileInfoCollection FindProfilesByUserName(string applicationName, ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.FindProfilesByUserName(authenticationOption, usernameToMatch, pageIndex, pageSize, out totalRecords);
		}

		public ProfileInfoCollection FindInactiveProfilesByUserName(string applicationName, ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
		{
            _Provider.ApplicationName = _ApplicationName;

			return _Provider.FindInactiveProfilesByUserName(authenticationOption, usernameToMatch, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
		}

		public TransportSettingsPropertyValueCollection GetPropertyValues(string applicationName, TransportSettingsContext context, TransportSettingsPropertyCollection collection)
		{
            _Provider.ApplicationName = _ApplicationName;

			var settingscontext = new SettingsContext { { "UserName", context.Username }, { "IsAuthenticated", context.IsAuthenticated } };

			var settingspropertycollection = new SettingsPropertyCollection();

			foreach(TransportSettingsProperty settingsproperty in collection)
			{
				settingspropertycollection.Add(ConvertSettingsProperty(settingsproperty));
			}

			var returncollection = new TransportSettingsPropertyValueCollection();

			foreach(SettingsPropertyValue settingspropertyvalue in _Provider.GetPropertyValues(settingscontext, settingspropertycollection))
			{
				returncollection.Add(ConvertSettingsPropertyValue(settingspropertyvalue));
			}

			return returncollection;
		}

		public void SetPropertyValues(string applicationName, TransportSettingsContext context, TransportSettingsPropertyValueCollection collection)
		{
            _Provider.ApplicationName = _ApplicationName;

			var settingscontext = new SettingsContext { { "UserName", context.Username }, { "IsAuthenticated", context.IsAuthenticated } };

			var settingspropertyvaluecollection = new SettingsPropertyValueCollection();

			foreach(TransportSettingsPropertyValue propertyvalue in collection)
			{
				settingspropertyvaluecollection.Add(ConvertSettingsPropertyValue(propertyvalue));
			}

			_Provider.SetPropertyValues(settingscontext, settingspropertyvaluecollection);
		}

		#endregion

		private SettingsProperty ConvertSettingsProperty(TransportSettingsProperty settingsProperty)
		{
			return new SettingsProperty(settingsProperty.Name, Type.GetType(settingsProperty.PropertyType), _Provider, settingsProperty.IsReadOnly, settingsProperty.DefaultValue, settingsProperty.SerializeAs, settingsProperty.Attributes, settingsProperty.ThrowOnErrorDeserializing, settingsProperty.ThrowOnErrorSerializing);
		}

		private static TransportSettingsProperty ConvertSettingsProperty(SettingsProperty settingsProperty)
		{
			return new TransportSettingsProperty {Name = settingsProperty.Name, PropertyType = settingsProperty.PropertyType.ToString(), IsReadOnly = settingsProperty.IsReadOnly, DefaultValue = settingsProperty.DefaultValue, SerializeAs = settingsProperty.SerializeAs, Attributes = settingsProperty.Attributes, ThrowOnErrorDeserializing = settingsProperty.ThrowOnErrorDeserializing, ThrowOnErrorSerializing = settingsProperty.ThrowOnErrorSerializing};
		}

		private SettingsPropertyValue ConvertSettingsPropertyValue(TransportSettingsPropertyValue settingsPropertyValue)
		{
			return new SettingsPropertyValue(ConvertSettingsProperty(settingsPropertyValue.Property)) {Deserialized = settingsPropertyValue.Deserialized, IsDirty = settingsPropertyValue.IsDirty, PropertyValue = settingsPropertyValue.PropertyValue, SerializedValue = settingsPropertyValue.SerializedValue};
		}

		private static TransportSettingsPropertyValue ConvertSettingsPropertyValue(SettingsPropertyValue settingsPropertyValue)
		{
			return new TransportSettingsPropertyValue {Property = ConvertSettingsProperty(settingsPropertyValue.Property), Deserialized = settingsPropertyValue.Deserialized, IsDirty = settingsPropertyValue.IsDirty, PropertyValue = settingsPropertyValue.PropertyValue, SerializedValue = settingsPropertyValue.SerializedValue};
		}
	}
}