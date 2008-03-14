using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace System.Diagnostics.UnitTests
{
	[TestFixture]
	public class TracerFixture
	{
		[Test]
		public void ShouldGetTraceSourceForType()
		{
			var source = Tracer.GetSourceFor<TracerFixture>();
			
			Assert.IsNotNull(source);
		}
	}
}
