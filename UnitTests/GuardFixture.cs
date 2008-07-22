using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NetFx.UnitTests
{
	[TestFixture]
	public class GuardQFixture
	{
		[ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: x")]
		[Test]
		public void ShouldThrowIfArgumentIsNull()
		{
			object x = null;
			Guard.NotNull(() => x);
		}

		[ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: a.m")]
		[Test]
		public void ShouldThrowIfExpressionIsNull()
		{
			P a = new P() { m = null };
			Guard.NotNull(() => a.m);
		}

		[ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: x")]
		[Test]
		public void ShouldThrowIfStringArgumentIsNull()
		{
			string x = null;
			Guard.NotNullOrEmpty(() => x);
		}

		[ExpectedException(typeof(ArgumentException), ExpectedMessage = "Value cannot be null or an empty string.\r\nParameter name: x")]
		[Test]
		public void ShouldThrowIfStringArgumentIsEmpty()
		{
			string x = "";
			Guard.NotNullOrEmpty(() => x);
		}

		[ExpectedException(typeof(ArgumentException), ExpectedMessage = "Value cannot be null or an empty string.\r\nParameter name: a.m")]
		[Test]
		public void ShouldThrowIfStringExpressionIsNull()
		{
			P a = new P() { m = "" };
			Guard.NotNullOrEmpty(() => a.m);
		}


		class P { public string m; }
	}
}