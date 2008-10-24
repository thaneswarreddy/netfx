/* 
 * Dependencies: 
 *		System.Web.Abstractions
 *		System.Web.Mvc
 *		System.Web.Routing
 * Authors: Juan Wajnerman - jwajnerman@manas.com.ar
 */

using System.IO;

namespace System.Web.Mvc
{
	public class FileResult : ActionResult
	{
		FileInfo file;
		string contentType;

		public FileResult(string filePath, string contentType)
			: this(new FileInfo(filePath), contentType)
		{
		}

		public FileResult(FileInfo file, string contentType)
		{
			this.file = file;
			this.contentType = contentType;
		}

		public bool DownloadAsAttachment { get; set; }

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.ContentType = contentType;
			context.HttpContext.Response.AppendHeader("Content-Length", file.Length.ToString());
			context.HttpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=" + file.Name);
			context.HttpContext.Response.TransmitFile(file.FullName);
		}
	}
}
