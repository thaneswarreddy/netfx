using System;
using System.Web.Mvc;
using NUnit.Framework;

namespace NetFx.UnitTests.Web.Mvc
{
	[TestFixture]
	public class ControllerExtensionsFixture
	{
		[ExpectedException(typeof(InvalidOperationException))]
		[Test]
		public void ShouldThrowIfRedirectWrongTarget()
		{
			var controller = new FooController();
			controller.RedirectToAction((BarController c) => Console.WriteLine(c.Value));
		}

		[Test]
		public void ShouldRedirectWithNoParameters()
		{
			// TODO
		}
		
		public class FooController : Controller
		{
		}

		public class BarController : Controller
		{
			public event EventHandler Handler;
			public string Value { get; set; }
		}
	}
}
