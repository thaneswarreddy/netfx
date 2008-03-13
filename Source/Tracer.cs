/* 
 * Dependencies: System.Diagnostics
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetFx
{
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
	}
}
