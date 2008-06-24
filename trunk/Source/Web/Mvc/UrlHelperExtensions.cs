using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Web.Routing;

namespace System.Web.Mvc
{
	/// <summary>
	/// Extensions for url creation using typed lambdas.
	/// </summary>
	public static class UrlHelperExtensions
	{
		/// <summary>
		/// Gets the action url for the given controller action.
		/// </summary>
		public static string Action<T>(this UrlHelper helper, Expression<Action<T>> action)
			where T : Controller
		{
			var call = ControllerExpression.GetMethodCall<T>(action);
			var actionName = call.Method.Name;
			var controllerName = ControllerExpression.GetControllerName<T>();

			var values = LinkBuilder.BuildParameterValuesFromExpression(call);
			values.Add("action", actionName);
			values.Add("controller", controllerName);

			var vpd = RouteTable.Routes.GetVirtualPath(helper.ViewContext, values);
			string target = null;
			if (vpd != null)
			{
				target = vpd.VirtualPath;
			}

			return target;
		}
	}
}
