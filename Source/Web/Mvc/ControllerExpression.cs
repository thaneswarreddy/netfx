/* 
 * Dependencies: 
 *		System.Web.Abstractions
 *		System.Web.Mvc
 *		System.Web.Routing
 *		ControllerExpressions.cs
 * Authors: Daniel Cazzulino - daniel@cazzulino.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace System.Web.Mvc
{
	static class ControllerExpression
	{
		public static MethodCallExpression GetMethodCall<T>(Expression<Action<T>> action) where T : Controller
		{
			var call = action.Body as MethodCallExpression;
			if (call == null)
			{
				throw new InvalidOperationException("Expression must be a method call");
			}
			if (call.Object != action.Parameters[0])
			{
				throw new InvalidOperationException("Method call must target lambda argument");
			}
			return call;
		}

		public static string GetControllerName<T>() where T : Controller
		{
			// TODO: Use better logic to chop off the controller suffix
			string controllerName = typeof(T).Name;
			if (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
			{
				controllerName = controllerName.Remove(controllerName.Length - 10, 10);
			}
			return controllerName;
		}
	}
}
