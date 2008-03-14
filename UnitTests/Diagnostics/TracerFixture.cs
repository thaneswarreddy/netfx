using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

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

		[Test]
		public void ShouldContainSplitSources()
		{
			var source = Tracer.GetSourceFor<TracerFixture>();

			var innerSources = source.Sources.ToList();

			Assert.AreEqual(4, innerSources.Count);
			Assert.IsNotNull(innerSources.FirstOrDefault(s => s.Name == "System"));
			Assert.IsNotNull(innerSources.FirstOrDefault(s => s.Name == "System.Diagnostics"));
			Assert.IsNotNull(innerSources.FirstOrDefault(s => s.Name == "System.Diagnostics.UnitTests"));
			Assert.IsNotNull(innerSources.FirstOrDefault(s => s.Name == "System.Diagnostics.UnitTests.TracerFixture"));
		}

		[Test]
		public void ShouldSplitOKWithoutNamespace()
		{
			var source = Tracer.GetSourceFor<FooWithoutNamespace>();

			Assert.AreEqual(1, source.Sources.Count());
			Assert.AreEqual(typeof(FooWithoutNamespace).Name, source.Sources.First().Name);
		}

		[Test]
		public void ShouldCacheTraceSource()
		{
			var source1 = Tracer.GetSourceFor<Foo>();
			var source2 = Tracer.GetSourceFor<Foo>();

			Assert.AreSame(source1, source2);
		}

		[Test]
		public void ShouldAddListenerToExisting()
		{
			var source = Tracer.GetSourceFor<Foo>();

			var mock = new Mock<TraceListener>();

			Tracer.AddListener("System.Diagnostics", mock.Object);

			// No other sources should have the listener
			Assert.AreEqual(0,
				(from ts in source.Sources
				 where ts.Name != "System.Diagnostics"
				 from ls in ts.Listeners.OfType<TraceListener>()
				 where ls == mock.Object
				 select ts)
				 .Count());

			Assert.AreEqual(1,
				(from ts in source.Sources
				 where ts.Name == "System.Diagnostics"
				 from ls in ts.Listeners.OfType<TraceListener>()
				 where ls == mock.Object
				 select ls)
				 .Count()
				 );
		}

		[Test]
		public void ShouldAddListenerToNewlyCreated()
		{
			var mock = new Mock<TraceListener>();

			Tracer.AddListener("System.Diagnostics", mock.Object);

			// Creation happens after adding now.
			var source = Tracer.GetSourceFor<Foo>();

			// No other sources should have the listener
			Assert.AreEqual(0,
				(from ts in source.Sources
				 where ts.Name != "System.Diagnostics"
				 from ls in ts.Listeners.OfType<TraceListener>()
				 where ls == mock.Object
				 select ts)
				 .Count());

			Assert.AreEqual(1,
				(from ts in source.Sources
				 where ts.Name == "System.Diagnostics"
				 from ls in ts.Listeners.OfType<TraceListener>()
				 where ls == mock.Object
				 select ls)
				 .Count()
				 );
		}

		[Test]
		public void ShouldReuseTraceSourcesAcrossComposites()
		{
			var fooSource = Tracer.GetSourceFor<Foo>();
			var barSource = Tracer.GetSourceFor<Bar>();

			Assert.AreSame(
				fooSource.Sources.First(ts => ts.Name == "System.Diagnostics"),
				barSource.Sources.First(ts => ts.Name == "System.Diagnostics"));
		}

		[Test]
		public void ShouldSetLoggingLevelToExistingAndNew()
		{
			var fooSource = Tracer.GetSourceFor<Foo>();
			
			Tracer.SetLoggingLevel("System.Diagnostics", SourceLevels.Information);

			var barSource = Tracer.GetSourceFor<Bar>();
			
			Assert.AreEqual(SourceLevels.Information, 
				fooSource.Sources.Where(ts => ts.Name == "System.Diagnostics").First().Switch.Level);

			Assert.AreEqual(SourceLevels.Information,
				barSource.Sources.Where(ts => ts.Name == "System.Diagnostics").First().Switch.Level);
		}

		[Test]
		public void ShouldInvokeAddedListeners()
		{
			var mock1 = new MockListener();
			var mock2 = new MockListener();

			Tracer.AddListener("System.Diagnostics", mock1);
			Tracer.AddListener("System.Diagnostics", mock2);

			var source = Tracer.GetSourceFor<Foo>();

			source.Sources.ForEach(ts => ts.Switch.Level = SourceLevels.Information);
	
			source.TraceInformation("foo");
			source.Flush();

			Assert.IsTrue(mock1.Invoked);
			Assert.IsTrue(mock2.Invoked);
		}

		[Test]
		public void ShouldSetLoggingLevelOnNewObjects()
		{
			var mock1 = new MockListener();

			Tracer.SetLoggingLevel("System.Diagnostics", SourceLevels.Information);

			var source = Tracer.GetSourceFor<Foo>();
			
			Tracer.AddListener("System.Diagnostics", mock1);

			source.TraceInformation("foo");
			source.Flush();

			Assert.IsTrue(mock1.Invoked);
		}
		// ShouldSetLoggingLevelOnNewObjects
	}

	public class MockListener : TraceListener
	{
		public bool Invoked;
		public string Message;

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			base.TraceEvent(eventCache, source, eventType, id, format, args);

			Invoked = true;
		}

		public override void Write(string message)
		{
			Message = message;
		}

		public override void WriteLine(string message)
		{
			Message = message;
		}
	}

	public class Foo
	{
		static readonly ITraceSource source = Tracer.GetSourceFor<Foo>();

		public Foo()
		{
		}
	}

	public class Bar { }
}

public class FooWithoutNamespace { }