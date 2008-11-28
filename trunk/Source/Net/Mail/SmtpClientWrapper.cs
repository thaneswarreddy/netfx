using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Net.Mail
{
	/// <summary>
	/// Implements the <see cref="ISmtpClient"/> interface 
	/// by inheriting from <see cref="SmtpClient"/>.
	/// </summary>
	public sealed class SmtpClientWrapper : SmtpClient, ISmtpClient
	{
		void IDisposable.Dispose()
		{
		}
	}
}
