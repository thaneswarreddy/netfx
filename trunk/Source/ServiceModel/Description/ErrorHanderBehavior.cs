using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Description
{
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
