using System;
using System.Threading;
using System.Net.Mail;
using System.Configuration;
using System.Net.Configuration;

namespace System.Diagnostics
{
	/// <summary>
	/// Listener that uses the SMTP configuration in the system.net/mailSettings/smtp section 
	/// together with a "to" attribute on the listener to send traces via email.
	/// </summary>
	public class EmailTraceListener : TraceListener
	{
		private static readonly AppDomain currentDomain;
		private static readonly Process currentProcess;

		static EmailTraceListener()
		{
			currentDomain = AppDomain.CurrentDomain;
			currentProcess = Process.GetCurrentProcess();
		}

		/// <summary>
		/// Returns the "to" attribute which is the only supported configuration.
		/// </summary>
		protected override string[] GetSupportedAttributes()
		{
			return new string[] { "to" };
		}

		/// <summary>
		/// Gets the recipients for the emails.
		/// </summary>
		public string To { get { return this.Attributes["to"]; } }

		#region TraceListener overrides

		public override void Write(string message)
		{
			WriteLogEntry(0, -1, TraceEventType.Information, string.Empty, DateTime.Now, Guid.Empty, null,
						  Environment.MachineName, currentDomain.FriendlyName,
						  currentProcess.Id, currentProcess.ProcessName,
						  Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId, message);
		}

		public override void WriteLine(string message)
		{
			Write(message);
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if (data is string)
			{
				WriteLogEntry(id, -1, eventType, string.Empty, DateTime.UtcNow, Guid.Empty, null, Environment.MachineName,
							  currentDomain.FriendlyName, currentProcess.Id, currentProcess.ProcessName,
							  Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId, (string)data);
			}
			else
			{
				base.TraceData(eventCache, source, eventType, id, data);
			}
		}

		public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
		{
			WriteLogEntry(id, -1, TraceEventType.Transfer, string.Empty, DateTime.UtcNow,
							  Trace.CorrelationManager.ActivityId, relatedActivityId, Environment.MachineName,
							  currentDomain.FriendlyName, currentProcess.Id, currentProcess.ProcessName, Thread.CurrentThread.Name,
							  Thread.CurrentThread.ManagedThreadId, message);
		}

		#endregion

		private void WriteLogEntry(int eventId, int priority, TraceEventType severity, string title, DateTime timeStamp,
			Guid activityId, Guid? relatedActivityId, string machineName, string appDomainName, int processId, string processName, string threadName,
			int threadId, string message)
		{
			//HACK: Workaround to ignore useless entries
			if (message.Contains("Information: 0 :"))
				return;

			var settings = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

			var client = new SmtpClient();
			client.Send(settings.From, this.To,
				String.Format("{0}: {1}-{2}", severity, machineName, appDomainName),
				String.Format(
@"Severity: {0}
ActivityId: {1}
Related Id: {2}
--
Machine:    {3}
AppDomain:  {4}
Process:    {5}
Thread:     {6}
--
{7}",
				severity, activityId, relatedActivityId, machineName, appDomainName, processName, threadName, message)
			);
		}
	}
}
