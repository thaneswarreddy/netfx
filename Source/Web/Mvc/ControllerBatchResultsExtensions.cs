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
	public static class ControllerBatchResultsExtensions
	{
		/// <summary>
		/// Helper method that returns the composition of the results
		/// </summary>
		/// <param name="controlle"></param>
		/// <param name="results"></param>
		/// <returns></returns>
		public static ActionResult BatchResults(this Controller controller, params ActionResult[] results)
		{
			return new CompositeResult(results);
		}
	}
}
