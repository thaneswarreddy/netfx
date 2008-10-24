/* 
 * Dependencies: 
 *		System.Web.Abstractions
 *		System.Web.Mvc
 *		System.Web.Routing
 * Authors: Juan Wajnerman - jwajnerman@manas.com.ar
 */

namespace System.Web.Mvc
{
	public class BinaryResult : ActionResult
	{
		byte[] data;
		string contentType;

		public BinaryResult(string contentType, byte[] data)
		{
			this.data = data;
			this.contentType = contentType;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.ContentType = contentType;
			context.HttpContext.Response.BinaryWrite(data);
		}
	}
}
