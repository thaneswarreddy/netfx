using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ServiceModel.Web
{
	public interface IWebOperationContext
	{
		IIncomingWebRequestContext IncomingRequest { get; }
		IOutgoingWebResponseContext OutgoingResponse { get; }
	}
}
