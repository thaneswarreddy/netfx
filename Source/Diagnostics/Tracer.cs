/* 
 * Dependencies: System.Diagnostics
 * Authors: Daniel Cazzulino - daniel@cazzulino.com
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Diagnostics
{
	/// <summary>
	/// Abstraction of <see cref="TraceSource"/> used by 
	/// components that want to statically cache their 
	/// own instances of the trace source.
	/// </summary>
	/// <remarks>
	/// If you're using <see cref="Tracer"/> tracing methods 
	/// directly, you don't need to deal with this interface.
	/// </remarks>
	/// <example>
	/// In this example, a component is caching statically 
	/// an instance of an <see cref="ITraceSource"/> to 
	/// reuse whenever it needs to trace information:
	/// <code>
	/// public class MyComponent
	/// {
	///		static readonly ITraceSource traceSource = Tracer.GetSource(typeof(MyComponent));
	///		
	///		public MyComponent()
	///		{
	///			traceSource.TraceInformation("MyComponent constructed!");
	///		}
	/// }
	/// </code>
	/// </example>
	internal interface ITraceSource
	{
		/// <summary>
		/// Supports the unit tests, do not use directly.
		/// </summary>
		/// <remarks>
		/// Exposes the undelying <see cref="TraceSource"/>s composed 
		/// by this source to enable testing of the <see cref="Tracer"/> class.
		/// </remarks>
		IEnumerable<TraceSource> Sources { get; }
		/// <summary>
		/// See <see cref="TraceSource.Name"/>.
		/// </summary>
		void Flush();
		/// <summary>
		/// See <see cref="TraceData(TraceEventType, int, object)"/>.
		/// </summary>
		void TraceData(TraceEventType eventType, int id, object data);
		/// <summary>
		/// See <see cref="TraceSource.TraceData(TraceEventType, int, params object[])"/>.
		/// </summary>
		void TraceData(TraceEventType eventType, int id, params object[] data);
		/// <summary>
		/// See <see cref="TraceSource.TraceEvent(TraceEventType, int)"/>.
		/// </summary>
		void TraceEvent(TraceEventType eventType, int id);
		/// <summary>
		/// See <see cref="TraceSource.TraceEvent(TraceEventType, int, string, params object[])"/>.
		/// </summary>
		void TraceEvent(TraceEventType eventType, int id, string format, params object[] args);
		/// <summary>
		/// See <see cref="TraceSource.TraceEvent(TraceEventType, int, string)"/>.
		/// </summary>
		void TraceEvent(TraceEventType eventType, int id, string message);
		/// <summary>
		/// See <see cref="TraceSource.TraceInformation(string)"/>.
		/// </summary>
		void TraceInformation(string message);
		/// <summary>
		/// See <see cref="TraceSource.TraceInformation(string, params object[])"/>.
		/// </summary>
		void TraceInformation(string format, params object[] args);
		/// <summary>
		/// See <see cref="TraceSource.TraceTransfer(int, string, Guid)"/>.
		/// </summary>
		void TraceTransfer(int id, string message, Guid relatedActivityId);
	}

	/// <summary>
	/// Provides uniformity for tracing, by providing a consistent way of 
	/// logging and leverage System.Diagnostics support.
	/// </summary>
	/// <remarks>
	/// This class basically exposes all the same members as <see cref="TraceSource"/> but 
	/// with a new parameter <c>Type sourceType</c> which is used to name the trace sources 
	/// for logging. 
	/// <para>
	/// At this moment, we provide a namespace-level trace source and a class-level one. They 
	/// can be configured separately.
	/// </para>
	/// <para>
	/// The <see cref="SetLoggingLevel"/> and <see cref="AddListener"/> methods provide 
	/// dynamic updates to the trace sources, unlike built-in .NET which does not allow this. Calls 
	/// to both methods will cause updates on trace sources already created and new ones created 
	/// from that point on.
	/// </para>
	/// </remarks>
	internal static class Tracer
	{
		static Dictionary<Type, SourceEntry> sources = new Dictionary<Type, SourceEntry>();
		static List<TraceListener> additionalListeners = new List<TraceListener>();
		static Dictionary<string, SourceLevels> defaultLevels = new Dictionary<string, SourceLevels>();

		class SourceEntry
		{
			public TraceSource TypeSource;
			public TraceSource NamespaceSource;
		}

		/// <summary>
		/// Sets the logging level of the given trace source.
		/// </summary>
		/// <param name="sourceName">Name of the trace source to change.</param>
		/// <param name="level">The new logging level.</param>
		internal static void SetLoggingLevel(string sourceName, SourceLevels level)
		{
			foreach (var item in sources)
			{
				if (item.Key.FullName == sourceName)
				{
					item.Value.TypeSource.Switch.Level = level;
				}
				else if (item.Key.Namespace == sourceName)
				{
					item.Value.NamespaceSource.Switch.Level = level;
				}
			}

			defaultLevels[sourceName] = level;
		}

		/// <summary>
		/// Adds a new listener to existing and new trace sources.
		/// </summary>
		/// <param name="sourceName">Name of the existing trace source to add the listener to.</param>
		/// <param name="listener">The new listener to register.</param>
		internal static void AddListener(string sourceName, TraceListener listener)
		{
			foreach (var item in sources)
			{
				if (item.Key.FullName == sourceName)
				{
					item.Value.TypeSource.Listeners.Add(listener);
				}
				else if (item.Key.Namespace == sourceName)
				{
					item.Value.NamespaceSource.Listeners.Add(listener);
				}
			}

			// TODO: this will cause the listener to be added to any new 
			// source, not only the one specified as the sourceName.
			additionalListeners.Add(listener);
		}

		/// <summary>
		/// Retrieves a source entry from the cache, or creates a new one.
		/// </summary>
		private static SourceEntry GetEntry(Type sourceType)
		{
			SourceEntry entry;
			if (!sources.TryGetValue(sourceType, out entry))
			{
				entry = new SourceEntry();
				entry.TypeSource = new TraceSource(sourceType.FullName);

				entry.NamespaceSource = new TraceSource(sourceType.Namespace);
				sources.Add(sourceType, entry);

				// See if we should change default level
				if (defaultLevels.ContainsKey(sourceType.FullName))
				{
					entry.TypeSource.Switch.Level = defaultLevels[sourceType.FullName];
				}
				if (defaultLevels.ContainsKey(sourceType.Namespace))
				{
					entry.NamespaceSource.Switch.Level = defaultLevels[sourceType.Namespace];
				}

				foreach (var item in additionalListeners)
				{
					entry.NamespaceSource.Listeners.Add(item);
					entry.TypeSource.Listeners.Add(item);
				}
			}

			return entry;
		}

		/// <summary>
		/// See TraceSource.TraceData.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceData(Type sourceType, TraceEventType eventType, int id, params object[] data)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceData(eventType, id, data);
			entry.TypeSource.TraceData(eventType, id, data);
		}

		/// <summary>
		/// See TraceSource.TraceData.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceData(Type sourceType, TraceEventType eventType, int id, object data)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceData(eventType, id, data);
			entry.TypeSource.TraceData(eventType, id, data);
		}

		/// <summary>
		/// See TraceSource.TraceEvent.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceEvent(eventType, id);
			entry.TypeSource.TraceEvent(eventType, id);
		}

		/// <summary>
		/// See TraceSource.TraceEvent.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id, string message)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceEvent(eventType, id, message);
			entry.TypeSource.TraceEvent(eventType, id, message);
		}

		/// <summary>
		/// See TraceSource.TraceEvent.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id, string format, params object[] args)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceEvent(eventType, id, format, args);
			entry.TypeSource.TraceEvent(eventType, id, format, args);
		}

		/// <summary>
		/// See TraceSource.TraceInformation.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceInformation(Type sourceType, string message)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceInformation(message);
			entry.TypeSource.TraceInformation(message);
		}

		/// <summary>
		/// See TraceSource.TraceInformation.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceInformation(Type sourceType, string format, params object[] args)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceInformation(format, args);
			entry.TypeSource.TraceInformation(format, args);
		}

		/// <summary>
		/// See TraceSource.TraceError.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceError(Type sourceType, Exception exception, string message)
		{
			SourceEntry entry = GetEntry(sourceType);

			string logmessage = message + Environment.NewLine + exception.ToString();

			entry.NamespaceSource.TraceEvent(TraceEventType.Error, 0, logmessage);
			entry.TypeSource.TraceEvent(TraceEventType.Error, 0, logmessage);
		}

		/// <summary>
		/// See TraceSource.TraceError.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceError(Type sourceType, Exception exception, string format, params object[] args)
		{
			SourceEntry entry = GetEntry(sourceType);

			string logmessage = format + Environment.NewLine + exception.ToString();

			entry.NamespaceSource.TraceEvent(TraceEventType.Error, 0, logmessage);
			entry.TypeSource.TraceEvent(TraceEventType.Error, 0, logmessage);
		}

		/// <summary>
		/// Retrieves a <see cref="ITraceSource"/> that can be 
		/// used by component <typeparamref name="T"/> to issue 
		/// trace statements.
		/// </summary>
		/// <typeparam name="T">Type of the component that will perform the logging.</typeparam>
		internal static ITraceSource GetSourceFor<T>()
		{
			return new CompositeTraceSource();
		}

		private class CompositeTraceSource : ITraceSource
		{
			public IEnumerable<TraceSource> Sources
			{
				get { throw new NotImplementedException(); }
			}

			public void Flush()
			{
				throw new NotImplementedException();
			}

			public void TraceData(TraceEventType eventType, int id, object data)
			{
				throw new NotImplementedException();
			}

			public void TraceData(TraceEventType eventType, int id, params object[] data)
			{
				throw new NotImplementedException();
			}

			public void TraceEvent(TraceEventType eventType, int id)
			{
				throw new NotImplementedException();
			}

			public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public void TraceEvent(TraceEventType eventType, int id, string message)
			{
				throw new NotImplementedException();
			}

			public void TraceInformation(string message)
			{
				throw new NotImplementedException();
			}

			public void TraceInformation(string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public void TraceTransfer(int id, string message, Guid relatedActivityId)
			{
				throw new NotImplementedException();
			}
		}
	}
}
