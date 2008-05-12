
using System.Net;

namespace System.ServiceModel
{
	public class ServiceException : Exception
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
