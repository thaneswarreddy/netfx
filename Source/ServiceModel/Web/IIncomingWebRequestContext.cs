using System.Net;

namespace System.ServiceModel.Web
{
#if NetFx	
	public interface IIncomingWebRequestContext
#else
	internal interface IIncomingWebRequestContext
#endif
	{
		string Accept { get; }
		long ContentLength { get; }
		string ContentType { get; }
		WebHeaderCollection Headers { get; }
		string Method { get; }
		UriTemplateMatch UriTemplateMatch { get; set; }
		string UserAgent { get; }
	}
}
