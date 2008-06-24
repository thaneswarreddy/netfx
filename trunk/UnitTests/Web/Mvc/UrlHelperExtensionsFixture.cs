using System;
using System.Web.Mvc;
using NUnit.Framework;
using System.Web;
using Moq;
using System.Web.Routing;

namespace NetFx.UnitTests.Web.Mvc
{
	[TestFixture]
	public class UrlHelperExtensionsFixture
	{
		// TODO: add tests
		[Ignore]
		[Test]
		public void ShouldRenderControllerUrl()
		{
			var http = new Mock<HttpContextBase>();
			http.CallBase = false;
			var context = new ViewContext(
				http.Object, 
				new System.Web.Routing.RouteData(
					new Route("foo/{id}/{enabled}", 
						new Mock<IRouteHandler>().Object),
					new Mock<IRouteHandler>().Object), 
				new FooController(), 
				"Foo", 
				"Bar", 
				null, 
				new TempDataDictionary(http.Object));

			var helper = new UrlHelper(context);

			var url = helper.Action<FooController>(c => c.Register("5", false));
		}

		class FooController : Controller
		{
			public void Register(string id, bool enabled)
			{
			}
		}
	}
}
