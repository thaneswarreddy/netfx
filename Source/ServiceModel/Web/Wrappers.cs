using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

#if NetFx
	public class IncomingWebRequestContextWrapper : IIncomingWebRequestContext
#else
	internal class IncomingWebRequestContextWrapper : IIncomingWebRequestContext
#endif
	{
		private IncomingWebRequestContext context;

		public IncomingWebRequestContextWrapper(IncomingWebRequestContext context)
		{
			this.context = context;
		}

		#region IIncomingWebRequestContext Members

		public string Accept
		{
			get { return context.Accept; }
		}

		public long ContentLength
		{
			get { return context.ContentLength; }
		}

		public string ContentType
		{
			get { return context.ContentType; }
		}

		public System.Net.WebHeaderCollection Headers
		{
			get { return context.Headers; }
		}

		public string Method
		{
			get { return context.Method; }
		}

		public UriTemplateMatch UriTemplateMatch
		{
			get
			{
				return context.UriTemplateMatch;
			}
			set
			{
				context.UriTemplateMatch = value;
			}
		}

		public string UserAgent
		{
			get { return context.UserAgent; }
		}

		#endregion
	}

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

#if NetFx
	public interface IWebOperationContext
#else
	internal interface IWebOperationContext
#endif
	{
		IIncomingWebRequestContext IncomingRequest { get; }
		IOutgoingWebResponseContext OutgoingResponse { get; }
	}

#if NetFx
	public class OutgoingWebResponseContextWrapper : IOutgoingWebResponseContext
#else
	internal class OutgoingWebResponseContextWrapper : IOutgoingWebResponseContext
#endif
	{
		private OutgoingWebResponseContext context;

		public OutgoingWebResponseContextWrapper(OutgoingWebResponseContext context)
		{
			this.context = context;
		}

		#region IOutgoingWebResponseContext Members

		public void SetStatusAsCreated(Uri locationUri)
		{
			context.SetStatusAsCreated(locationUri);
		}

		public void SetStatusAsNotFound()
		{
			context.SetStatusAsNotFound();
		}

		public void SetStatusAsNotFound(string description)
		{
			context.SetStatusAsNotFound(description);
		}

		public long ContentLength
		{
			get
			{
				return context.ContentLength;
			}
			set
			{
				context.ContentLength = value;
			}
		}

		public string ContentType
		{
			get
			{
				return context.ContentType;
			}
			set
			{
				context.ContentType = value;
			}
		}

		public string ETag
		{
			get
			{
				return context.ETag;
			}
			set
			{
				context.ETag = value;
			}
		}

		public System.Net.WebHeaderCollection Headers
		{
			get { return context.Headers; }
		}

		public DateTime LastModified
		{
			get
			{
				return context.LastModified;
			}
			set
			{
				context.LastModified = value;
			}
		}

		public string Location
		{
			get
			{
				return context.Location;
			}
			set
			{
				context.Location = value;
			}
		}

		public System.Net.HttpStatusCode StatusCode
		{
			get
			{
				return context.StatusCode;
			}
			set
			{
				context.StatusCode = value;
			}
		}

		public string StatusDescription
		{
			get
			{
				return context.StatusDescription;
			}
			set
			{
				context.StatusDescription = value;
			}
		}

		public bool SuppressEntityBody
		{
			get
			{
				return context.SuppressEntityBody;
			}
			set
			{
				context.SuppressEntityBody = value;
			}
		}

		#endregion
	}

#if NetFx
	public class WebOperationContextWrapper : IWebOperationContext
#else
	internal class WebOperationContextWrapper : IWebOperationContext
#endif
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
