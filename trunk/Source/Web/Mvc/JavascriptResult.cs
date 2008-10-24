/* 
 * Dependencies: 
 *		System.Web.Abstractions
 *		System.Web.Mvc
 *		System.Web.Routing
 * Authors: Brian Cardiff - bcardiff@manas.com.ar
 *          Juan Wajnerman - jwajnerman@manas.com.ar
 */

namespace System.Web.Mvc
{
	/// <summary>
	/// Renders a JavaScript code
	/// </summary>
	public class JavascriptResult : ActionResult
	{
		public JavascriptResult(string code)
		{
			Code = code;
		}

		public string Code { get; set; }

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.Output.WriteLine(string.Format(
				@"<script type=""text/javascript"">{0}</script>", Code
			));
		}
	}
}
