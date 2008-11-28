using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace System.Net.Mail
{
	/// <summary>
	/// Interface implemented by SMTP clients.
	/// </summary>
	public interface ISmtpClient : IMailClient
	{
		/// <summary>
		/// Sends the specified e-mail message to an SMTP server for delivery. 
		/// </summary>
		void Send(MailMessage message);
	}

	/// <summary>
	/// Adds usability overloads to <see cref="ISmtpClient"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ISmtpClientExtensions
	{
		/// <summary>
		/// Sends the specified e-mail message to an SMTP server for delivery. 
		/// </summary>
		public static void Send(this ISmtpClient smtp, string from, string recipients, string subject, string body)
		{
			MailMessage message = new MailMessage(from, recipients, subject, body);
			smtp.Send(message);
		}
	}
}
