using System.Net;

namespace System.ServiceModel.Web
{
#if NetFx	
	public interface IOutgoingWebResponseContext
#else
	internal interface IOutgoingWebResponseContext
#endif
	{
		void SetStatusAsCreated(Uri locationUri);
		void SetStatusAsNotFound();
		void SetStatusAsNotFound(string description);

		long ContentLength { get; set; }
		string ContentType { get; set; }
		string ETag { get; set; }
		WebHeaderCollection Headers { get; }
		DateTime LastModified { get; set; }
		string Location { get; set; }
		HttpStatusCode StatusCode { get; set; }
		string StatusDescription { get; set; }
		bool SuppressEntityBody { get; set; }
	}
}
