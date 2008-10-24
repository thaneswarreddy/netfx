/* 
 * Dependencies: 
 *		System.Web.Abstractions
 *		System.Web.Mvc
 *		System.Web.Routing
 * Authors: Juan Wajnerman - jwajnerman@manas.com.ar
 */

using System.Net;

namespace System.Web.Mvc
{
	public class StatusCodeResult : ActionResult
	{
		HttpStatusCode statusCode;

		public StatusCodeResult(HttpStatusCode statusCode)
		{
			this.statusCode = statusCode;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.StatusCode = (int)statusCode;
		}
	}
}
