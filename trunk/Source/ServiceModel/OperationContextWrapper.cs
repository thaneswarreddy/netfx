using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ServiceModel
{
	public class OperationContextWrapper : IOperationContext
	{
		OperationContext context;

		public OperationContextWrapper(OperationContext context)
		{
			this.context = context;
		}

		#region IOperationContext Members

		public event EventHandler OperationCompleted;

		public T GetCallbackChannel<T>()
		{
			return context.GetCallbackChannel<T>();
		}

		public void SetTransactionComplete()
		{
			context.SetTransactionComplete();
		}

		public IContextChannel Channel
		{
			get { return context.Channel; }
		}

		public IExtensionCollection<OperationContext> Extensions
		{
			get { return context.Extensions; }
		}

		public bool HasSupportingTokens
		{
			get { return context.HasSupportingTokens; }
		}

		public ServiceHostBase Host
		{
			get { return context.Host; }
		}

		public System.ServiceModel.Channels.MessageHeaders IncomingMessageHeaders
		{
			get { return context.IncomingMessageHeaders; }
		}

		public System.ServiceModel.Channels.MessageProperties IncomingMessageProperties
		{
			get { return context.IncomingMessageProperties; }
		}

		public System.ServiceModel.Channels.MessageVersion IncomingMessageVersion
		{
			get { return context.IncomingMessageVersion; }
		}

		public InstanceContext InstanceContext
		{
			get { return context.InstanceContext; }
		}

		public bool IsUserContext
		{
			get { return context.IsUserContext; }
		}

		public System.ServiceModel.Channels.MessageHeaders OutgoingMessageHeaders
		{
			get { return context.OutgoingMessageHeaders; }
		}

		public System.ServiceModel.Channels.MessageProperties OutgoingMessageProperties
		{
			get { return context.OutgoingMessageProperties; }
		}

		public System.ServiceModel.Channels.RequestContext RequestContext
		{
			get
			{
				return context.RequestContext;
			}
			set
			{
				context.RequestContext = value;
			}
		}

		public IServiceSecurityContext ServiceSecurityContext
		{
			get { return new ServiceSecurityContextWrapper(context.ServiceSecurityContext); }
		}

		public string SessionId
		{
			get { return context.SessionId; }
		}

		public ICollection<System.ServiceModel.Security.SupportingTokenSpecification> SupportingTokens
		{
			get { return context.SupportingTokens; }
		}

		#endregion
	}
}
