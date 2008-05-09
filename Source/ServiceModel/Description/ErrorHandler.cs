using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

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
				this.Context.OutgoingResponse.StatusCode = ((ServiceException)error).StatusCode;
				this.Context.OutgoingResponse.StatusDescription = error.Message;
				this.Context.OutgoingResponse.SuppressEntityBody = false;
			}
		}
	}


}
