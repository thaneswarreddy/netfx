using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Net.Mail
{
	public interface IMailClient : IDisposable
	{
		/// <summary>
		/// Gets or sets the name or IP address of the host used for mail transactions. 
		/// </summary>
		string Host { get; set; }

		/// <summary>
		/// Gets or sets the port used for mail transactions. 
		/// </summary>
		int Port { get; set; }

		/// <summary>
		/// Gets or sets the credentials used to authenticate the client. 
		/// </summary>
		ICredentialsByHost Credentials { get; set; }

		/// <summary>
		/// Specify whether the client uses Secure Sockets Layer (SSL) to encrypt the connection. 
		/// </summary>
		bool EnableSsl { get; set; }
	}
}
