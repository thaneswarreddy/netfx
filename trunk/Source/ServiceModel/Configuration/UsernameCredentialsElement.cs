
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.ServiceModel.Description;

namespace System.ServiceModel.Configuration
{
	public class UsernameCredentialsElement : ClientCredentialsElement
	{
		public UsernameCredentialsElement()
			: base()
		{
		}

		protected override object CreateBehavior()
		{
			ClientCredentials credentials = new ClientCredentials();
			if (this.SecurityMode == SecurityMode.Transport)
			{
				credentials.Windows.ClientCredential.UserName = this.Username;
				credentials.Windows.ClientCredential.Password = this.Password;
			}
			else if (this.SecurityMode == SecurityMode.Message ||
				this.SecurityMode == SecurityMode.TransportWithMessageCredential)
			{
				credentials.UserName.UserName = this.Username;
				credentials.UserName.Password = this.Password;
			}
			return credentials;
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				ConfigurationPropertyCollection properties = base.Properties;
				if (!properties.Contains("userName"))
				{
					properties.Add(new ConfigurationProperty("userName", typeof(string)));
				}

				if (!properties.Contains("password"))
				{
					properties.Add(new ConfigurationProperty("password", typeof(string)));
				}

				if (!properties.Contains("securityMode"))
				{
					properties.Add(new ConfigurationProperty("securityMode", typeof(SecurityMode), SecurityMode.Transport));
				}

				return properties;
			}
		}

		[ConfigurationProperty("userName")]
		public string Username
		{
			get { return (string)base["userName"]; }
		}

		[ConfigurationProperty("password")]
		public string Password
		{
			get { return (string)base["password"]; }
		}

		[ConfigurationProperty("securityMode", DefaultValue = SecurityMode.Transport)]
		public SecurityMode SecurityMode
		{
			get { return (SecurityMode)base["securityMode"]; }
		}
	}
}
