using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Net.Mail
{
	/// <summary>
	/// Interface implemented by POP3 clients.
	/// </summary>
	public interface IPopClient : IMailClient
	{
		/// <summary>
		/// Receives all e-mail messages in the inbox. 
		/// </summary>
		IEnumerable<MailMessage> ReceiveAll();
	}
}
