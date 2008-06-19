
using System.Net;

namespace System.ServiceModel
{
	/// <summary>
	/// Exception thrown by a service, optionally reporting an 
	/// <see cref="HttpStatusCode"/> to the client.
	/// </summary>
#if NetFx	
	public class ServiceException : Exception
#else
	internal class ServiceException : Exception
#endif
	{
		HttpStatusCode statusCode;

		/// <summary>
		/// Initializes the exception.
		/// </summary>
		public ServiceException()
			: base()
		{
		}

		/// <summary>
		/// Initializes the exception with the given message.
		/// </summary>
		public ServiceException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes the exception with the given message and HTTP status code.
		/// </summary>
		public ServiceException(string message, HttpStatusCode statusCode)
			: base(message)
		{
			this.statusCode = statusCode;
		}

		/// <summary>
		/// Gets the status code for the exception.
		/// </summary>
		public HttpStatusCode StatusCode
		{
			get { return statusCode; }
		}
	}
}
