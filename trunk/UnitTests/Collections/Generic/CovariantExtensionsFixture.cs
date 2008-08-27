using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace NetFx.UnitTests.Collections.Generic
{
	[TestFixture]
	public class CovariantExtensionsFixture
	{
		[Test]
		public void ShouldConvertCollections()
		{
			var barcol = new Collection<IBar>();
			IList<IFoo> foocol = barcol.ToCovariant<IBar, IFoo>();
			ICollection<IFoo> foo2 = barcol.ToCovariant<IBar, IFoo>();
			IEnumerable<IFoo> foo3 = barcol.ToCovariant<IBar, IFoo>();
			
		}

		interface IFoo { }
		interface IBar : IFoo { }
	}
}
