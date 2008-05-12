
using System.Net;

namespace System.ServiceModel
{
#if NetFx	
	public class ServiceException : Exception
#else
	internal class ServiceException : Exception
#endif
	{
		HttpStatusCode statusCode;

		public ServiceException()
			: base()
		{
		}

		public ServiceException(string message)
			: base(message)
		{
		}

		public ServiceException(string message, HttpStatusCode statusCode)
			: base(message)
		{
			this.statusCode = statusCode;
		}

		public HttpStatusCode StatusCode
		{
			get { return statusCode; }
		}
	}
}
