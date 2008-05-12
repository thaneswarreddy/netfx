
namespace System.ServiceModel.Web
{
#if NetFx	
	public interface IWebOperationContext
#else
	internal interface IWebOperationContext
#endif
	{
		IIncomingWebRequestContext IncomingRequest { get; }
		IOutgoingWebResponseContext OutgoingResponse { get; }
	}
}
