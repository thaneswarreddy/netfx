using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace System.ServiceModel
{
	/// <summary>
	/// Provides an implementation of <see cref="IOperationContext"/> that 
	/// wraps the underlying runtime <see cref="OperationContext"/>.
	/// </summary>
	public class OperationContextWrapper : IOperationContext
	{
		OperationContext context;

		public OperationContextWrapper(OperationContext context)
		{
			this.context = context;
		}

		public event EventHandler OperationCompleted;

		 T IOperationContext.GetCallbackChannel<T>()
		{
			return context.GetCallbackChannel<T>();
		}

		 void IOperationContext.SetTransactionComplete()
		{
			context.SetTransactionComplete();
		}

		 IContextChannel IOperationContext.Channel
		{
			get { return context.Channel; }
		}

		 IExtensionCollection<OperationContext> IOperationContext.Extensions
		{
			get { return context.Extensions; }
		}

		 bool IOperationContext.HasSupportingTokens
		{
			get { return context.HasSupportingTokens; }
		}

		 ServiceHostBase IOperationContext.Host
		{
			get { return context.Host; }
		}

		 MessageHeaders IOperationContext.IncomingMessageHeaders
		{
			get { return context.IncomingMessageHeaders; }
		}

		 MessageProperties IOperationContext.IncomingMessageProperties
		{
			get { return context.IncomingMessageProperties; }
		}

		 MessageVersion IOperationContext.IncomingMessageVersion
		{
			get { return context.IncomingMessageVersion; }
		}

		 InstanceContext IOperationContext.InstanceContext
		{
			get { return context.InstanceContext; }
		}

		 bool IOperationContext.IsUserContext
		{
			get { return context.IsUserContext; }
		}

		 MessageHeaders IOperationContext.OutgoingMessageHeaders
		{
			get { return context.OutgoingMessageHeaders; }
		}

		 MessageProperties IOperationContext.OutgoingMessageProperties
		{
			get { return context.OutgoingMessageProperties; }
		}

		 RequestContext IOperationContext.RequestContext
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

		 IServiceSecurityContext IOperationContext.ServiceSecurityContext
		{
			get { return new ServiceSecurityContextWrapper(context.ServiceSecurityContext); }
		}

		 string IOperationContext.SessionId
		{
			get { return context.SessionId; }
		}

		 ICollection<SupportingTokenSpecification> IOperationContext.SupportingTokens
		{
			get { return context.SupportingTokens; }
		}
	}
}
