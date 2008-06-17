using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NUnit.Framework;

namespace NetFx.UnitTests.Web
{
	[TestFixture]
	public class UriExtensionsFixture
	{
		[Test]
		public void HasValue()
		{
			var uri = new Uri("http://host?a=1&b=2&a=3");
			Assert.IsTrue(uri.HasValue("a", "1"));
			Assert.IsTrue(uri.HasValue("b", "2"));
			Assert.IsTrue(uri.HasValue("a", "3"));
			Assert.IsFalse(uri.HasValue("a", "2"));
			Assert.IsFalse(uri.HasValue("c", "42"));
		}

		[Test]
		public void OverrideNonExistingArgument()
		{
			var uri = new Uri("http://host/?a=1");
			var uri2 = uri.Override("b", "4");
			Assert.IsTrue(uri2.HasValue("a", "1"));
			Assert.IsTrue(uri2.HasValue("b", "4"));
		}

		[Test]
		public void OverrideExistingArgument()
		{
			var uri = new Uri("http://host/?c=3");
			var uri2 = uri.Override("c", "4");
			Assert.IsTrue(uri2.HasValue("c", "4"));
			Assert.IsFalse(uri2.HasValue("c", "3"));
		}

		[Test]
		public void AppendToUriWithoutQueryString()
		{
			var uri = new Uri("http://host");
			var uri2 = uri.Append("c", "4");
			Assert.IsTrue(uri2.HasValue("c", "4"));
		}

		[Test]
		public void AppendToExistingQueryStringArgument()
		{
			var uri = new Uri("http://host/?c=5");
			var uri2 = uri.Append("c", "4");
			Assert.IsTrue(uri2.HasValue("c", "5"));
			Assert.IsTrue(uri2.HasValue("c", "4"));
		}

		[Test]
		public void RemoveAllOcurrences()
		{
			var uri = new Uri("http://host/?c=5&b=1&c=2");
			var uri2 = uri.Remove("c");
			Assert.IsFalse(uri2.HasValue("c", "5"));
			Assert.IsTrue(uri2.HasValue("b", "1"));
			Assert.IsFalse(uri2.HasValue("c", "2"));
		}

		[Test]
		public void RemoveAllQueryString()
		{
			var uri = new Uri("http://host/?c=5&b=1&c=2");
			var uri2 = uri.WithoutQueryString();
			Assert.AreEqual(new Uri("http://host/"), uri2);
		}

		[Test]
		public void RemoveSingleOccurrence()
		{
			var uri = new Uri("http://host?a=1&b=2&a=3");
			var uri2 = uri.Remove("a", "1");
			Assert.IsFalse(uri2.HasValue("a", "1"));
			Assert.IsTrue(uri2.HasValue("b", "2"));
			Assert.IsTrue(uri2.HasValue("a", "3"));
		}
	}
}
