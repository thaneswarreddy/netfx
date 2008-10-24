/* 
 * Dependencies: 
 *		System.Web.Abstractions
 *		System.Web.Mvc
 *		System.Web.Routing
 *		System.ServiceModel.Web
 * Authors: Juan Wajnerman - jwajnerman@manas.com.ar
 */

using System.ServiceModel.Syndication;
using System.Xml;

namespace System.Web.Mvc
{
	public class SyndicationFeedResult : ActionResult
	{
		SyndicationFeed feed;
		string format;

		public SyndicationFeedResult(SyndicationFeed feed, string format)
		{
			this.feed = feed;
			this.format = format;
		}

		public SyndicationFeedResult(SyndicationFeed feed)
			: this(feed, "atom")
		{
		}

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.ContentType = "text/xml";
			SyndicationFeedFormatter f = format == "atom" ?
					(SyndicationFeedFormatter)new Atom10FeedFormatter(feed) :
					(SyndicationFeedFormatter)new Rss20FeedFormatter(feed);
			using (var writer = XmlWriter.Create(context.HttpContext.Response.Output))
				f.WriteTo(writer);
		}
	}
}
