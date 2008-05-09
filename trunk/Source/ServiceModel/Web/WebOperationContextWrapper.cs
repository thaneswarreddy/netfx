
namespace System.ServiceModel.Web
{
	public class WebOperationContextWrapper : IWebOperationContext
	{
		private WebOperationContext context;

		public WebOperationContextWrapper(WebOperationContext context)
		{
			this.context = context;
		}

		#region IWebOperationContext Members

		public IIncomingWebRequestContext IncomingRequest
		{
			get { return new IncomingWebRequestContextWrapper(context.IncomingRequest); }
		}

		public IOutgoingWebResponseContext OutgoingResponse
		{
			get { return new OutgoingWebResponseContextWrapper(context.OutgoingResponse); }
		}

		#endregion
	}
}
