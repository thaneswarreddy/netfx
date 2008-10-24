/* 
 * Dependencies: 
 *		System.Web.Abstractions
 *		System.Web.Mvc
 *		System.Web.Routing
 * Authors: Brian Cardiff - bcardiff@manas.com.ar
 *          Juan Wajnerman - jwajnerman@manas.com.ar
 */

using System.Collections.Generic;

namespace System.Web.Mvc
{
	/// <summary>
	/// Executes a collection of ActionResult
	/// </summary>
	public class CompositeResult : ActionResult
	{
		IEnumerable<ActionResult> actionResults;

		public CompositeResult(IEnumerable<ActionResult> actionResults)
		{
			this.actionResults = actionResults;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			foreach (var actionResult in actionResults)
			{
				actionResult.ExecuteResult(context);
			}
		}
	}
}
