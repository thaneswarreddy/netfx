﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Net;

namespace System.ServiceModel
{
	/// <summary>
	/// Server behavior to control the caching of the results. It can be applied at the service or operation level.
	/// </summary>
	internal class HttpCacheAttribute : Attribute, IOperationBehavior, IContractBehavior, IParameterInspector, IDispatchMessageInspector
	{
		private double maxSeconds;

		public HttpCacheAttribute(double maxSeconds)
		{
			this.maxSeconds = maxSeconds;
		}

		private void SetHeaders()
		{
			var expires = maxSeconds == 0 ? DateTime.MinValue : DateTime.Now.AddSeconds(maxSeconds).ToUniversalTime();
			var formattedExpires = expires.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'");
			var headers = WebOperationContext.Current.OutgoingResponse.Headers;
			if (headers[HttpResponseHeader.Expires] == null)
				headers.Add(HttpResponseHeader.Expires, formattedExpires);
			if (headers[HttpResponseHeader.CacheControl] == null)
				if (maxSeconds > 0)
					headers.Add(HttpResponseHeader.CacheControl, "max-age=" + (int)maxSeconds);
				else
					headers.Add(HttpResponseHeader.CacheControl, "no-cache");
		}

		void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
		{
			dispatchOperation.ParameterInspectors.Add(this);
		}

		void IContractBehavior.ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
		{
			dispatchRuntime.MessageInspectors.Add(this);
		}

		void IParameterInspector.AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
		{
			SetHeaders();
		}

		void IDispatchMessageInspector.BeforeSendReply(ref Message reply, object correlationState)
		{
			SetHeaders();
		}

		#region Unused hooks

		void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
		{
		}

		void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
		{
		}

		void IOperationBehavior.Validate(OperationDescription operationDescription)
		{
		}

		object IParameterInspector.BeforeCall(string operationName, object[] inputs)
		{
			return null;
		}

		void IContractBehavior.AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		void IContractBehavior.ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
		}

		void IContractBehavior.Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
		{
		}

		object IDispatchMessageInspector.AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			return null;
		}

		#endregion
	}
}
