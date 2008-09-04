/* 
 * Dependencies: 
 *		System.Web.Abstractions
 *		System.Web.Mvc
 *		System.Web.Routing
 *		ControllerExpressions.cs
 *		
 * Authors: Daniel Cazzulino - daniel@cazzulino.com
 */

using System.Linq.Expressions;
using System.Web.Routing;
using Microsoft.Web.Mvc;

namespace System.Web.Mvc
{
	/// <summary>
	/// Extensions to <see cref="Controller"/> classes.
	/// </summary>
	public static class ControllerExtensions
	{
		/// <summary>
		/// Performs a redirect based on an expression representing an 
		/// invocation to a controller method that may include arguments.
		/// </summary>
		/// <typeparam name="T">Type of the controller to redirect to. Can be omitted as it can be inferred from the action type.</typeparam>
		/// <param name="controller">The controller performing the redirect.</param>
		/// <param name="action">The action containing the redirect.</param>
		public static void RedirectToAction<T>(this Controller controller, Expression<Action<T>> action)
			where T : Controller
		{
			string target = controller.ActionUrl<T>(action);
			controller.HttpContext.Response.Redirect(target);
		}

		/// <summary>
		/// Returns the URL for performing an invokation to an action based
		/// on an expression representing an invocation to a controller method
		/// that may include arguments.
		/// </summary>
		/// <typeparam name="T">Type of the controller to call to. Can be omitted as it can be inferred from the action type.</typeparam>
		/// <param name="controller">The controller performing the call.</param>
		/// <param name="action">The action containing the call.</param>
		public static string ActionUrl<T>(this Controller controller, Expression<Action<T>> action)
			where T : Controller
		{
			var call = ControllerExpression.GetMethodCall<T>(action);

			string actionName = call.Method.Name;
			string controllerName = ControllerExpression.GetControllerName<T>();

			var values = LinkBuilder.BuildParameterValuesFromExpression(call);
			values.Add("action", actionName);
			values.Add("controller", controllerName);

			var vpd = RouteTable.Routes.GetVirtualPath(controller.ControllerContext, values);
			string target = null;
			if (vpd != null)
			{
				target = vpd.VirtualPath;
			}

			return target;
		}

		/// <summary>
		/// Returns the full URL for performing an invocation to an action based
		/// on an expression representing an invocation to a controller method
		/// that may include arguments.
		/// </summary>
		/// <typeparam name="T">Type of the controller to call to. Can be omitted as it can be inferred from the action type.</typeparam>
		/// <param name="controller">The controller performing the call.</param>
		/// <param name="action">The action containing the call.</param>
		public static string FullActionUrl<T>(this Controller controller, Expression<Action<T>> action)
			where T : Controller
		{
			string host = controller.HttpContext.Request.Url.Authority;
			string schema = controller.HttpContext.Request.Url.Scheme;
			string virtualPath = ActionUrl(controller, action);

			return string.Format("{0}://{1}{2}", schema, host, virtualPath);
		}
	}
}
