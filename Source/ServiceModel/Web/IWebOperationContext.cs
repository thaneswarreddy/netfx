
namespace System.ServiceModel.Web
{
	public interface IWebOperationContext
	{
		IIncomingWebRequestContext IncomingRequest { get; }
		IOutgoingWebResponseContext OutgoingResponse { get; }
	}
}
