﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace System.ServiceModel.Web
{
	public interface IIncomingWebRequestContext
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