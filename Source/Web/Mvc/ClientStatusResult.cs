/* 
 * Dependencies: 
 *		System.Web.Abstractions
 *		System.Web.Mvc
 *		System.Web.Routing
 *		JavascriptResult.cs
 *		Content/status.css
 *		Content/status.js
 * Authors: Brian Cardiff - bcardiff@manas.com.ar
 *          Juan Wajnerman - jwajnerman@manas.com.ar
 */

namespace System.Web.Mvc
{
	public enum ClientStatus { Info, Warning, Error }

	/// <summary>
	/// Performs a notification on the client with an status message.
	/// </summary>
	public class ClientStatusResult : JavascriptResult
	{
		public ClientStatusResult(ClientStatus kind, string message)
			: base(string.Empty)
		{
			string method;
			switch (kind)
			{
				case ClientStatus.Info:
					method = "showInfo";
					break;
				case ClientStatus.Warning:
					method = "showWarning";
					break;
				case ClientStatus.Error:
					method = "showError";
					break;
				default:
					throw new NotImplementedException();
			}

			Code = string.Format(@"jQuery.status.{0}(""{1}"")", method, message.Replace("\"", "\\\""));
		}
	}
}
