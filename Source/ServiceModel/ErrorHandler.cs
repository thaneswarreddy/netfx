using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Description;

namespace System.ServiceModel.Description
{
	public class ErrorHandler : IErrorHandler
	{
		const string FaultNs = "http://code.google.com/p/netfx/";
		IWebOperationContext operationContext;

		public ErrorHandler()
		{
		}

		public IWebOperationContext Context
		{
			get
			{
				if (this.operationContext == null && WebOperationContext.Current != null)
				{
					this.operationContext = new WebOperationContextWrapper(WebOperationContext.Current);
				}

				return this.operationContext;
			}
			set
			{
				this.operationContext = value;
			}
		}

		public bool HandleError(Exception error)
		{
			return true;
		}

		public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			FaultCode code = FaultCode.CreateSenderFaultCode(error.GetType().Name, FaultNs);
			fault = Message.CreateMessage(version, code, error.Message, null);

			if (this.Context != null)
			{
				// TODO: to exception shielding
				// TODO: do logging
				// TODO: ensure we do special behavior only for ServiceException (check cast)
				this.Context.OutgoingResponse.StatusCode = ((ServiceException)error).StatusCode;

				// Strip invalid chars.
				var sb = new StringBuilder();
				for (int i = 0; i < error.Message.Length; i++)
				{
					char ch = (char)('\x00ff' & error.Message[i]);
					if (((ch <= '\x001f') && (ch != '\t')) || (ch == '\x007f'))
					{
						// Specified value has invalid Control characters.
						// See HttpListenerResponse.StatusDescription
					}
					else
					{
						sb.Append(ch);
					}
				}

				this.Context.OutgoingResponse.StatusDescription = sb.ToString();
				this.Context.OutgoingResponse.SuppressEntityBody = false;
			}
		}
	}

	public class ErrorHanderBehavior : WebHttpBehavior
	{
		IErrorHandler errorHandler;

		public ErrorHanderBehavior(IErrorHandler errorHandler)
		{
			this.errorHandler = errorHandler;
		}

		protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.ChannelDispatcher.ErrorHandlers.Clear();
			endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(errorHandler);
		}
	}
}

namespace System.ServiceModel.Configuration
{
	public class ErrorHandlerExtensionElement : BehaviorExtensionElement
	{
		public override Type BehaviorType
		{
			get { return typeof(ErrorHanderBehavior); }
		}

		protected override object CreateBehavior()
		{
			return new ErrorHanderBehavior(new ErrorHandler());
		}
	}
}

