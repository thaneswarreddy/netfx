/* 
 * Dependencies: 
 *		System.Web.Abstractions
 *		System.Web.Mvc
 *		System.Web.Routing
 *		
 * Authors: Daniel Cazzulino - daniel@cazzulino.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Web.Routing;

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
		{
			MethodCallExpression call = action.Body as MethodCallExpression;
			if (call == null)
			{
				throw new InvalidOperationException("Expression must be a method call");
			}
			if (call.Object != action.Parameters[0])
			{
				throw new InvalidOperationException("Method call must target lambda argument");
			}

			string actionName = call.Method.Name;
			// TODO: Use better logic to chop off the controller suffix
			string controllerName = typeof(T).Name;
			if (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
			{
				controllerName = controllerName.Remove(controllerName.Length - 10, 10);
			}

			var values = LinkBuilder.BuildParameterValuesFromExpression(call);
			values.Add("action", actionName);
			values.Add("controller", controllerName);

			VirtualPathData vpd = RouteTable.Routes.GetVirtualPath(controller.ControllerContext, values);
			string target = null;
			if (vpd != null)
			{
				target = vpd.VirtualPath;
			}

			return target;
		}
	}
}
