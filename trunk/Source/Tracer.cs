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
	/// Provides uniformity for tracing.
	/// </summary>
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

			additionalListeners.Add(listener);
		}

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

		[Conditional("TRACE")]
		public static void TraceData(Type sourceType, TraceEventType eventType, int id, params object[] data)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceData(eventType, id, data);
			entry.TypeSource.TraceData(eventType, id, data);
		}

		[Conditional("TRACE")]
		public static void TraceData(Type sourceType, TraceEventType eventType, int id, object data)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceData(eventType, id, data);
			entry.TypeSource.TraceData(eventType, id, data);
		}

		[Conditional("TRACE")]
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceEvent(eventType, id);
			entry.TypeSource.TraceEvent(eventType, id);
		}

		[Conditional("TRACE")]
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id, string message)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceEvent(eventType, id, message);
			entry.TypeSource.TraceEvent(eventType, id, message);
		}

		[Conditional("TRACE")]
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id, string format, params object[] args)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceEvent(eventType, id, format, args);
			entry.TypeSource.TraceEvent(eventType, id, format, args);
		}

		[Conditional("TRACE")]
		public static void TraceInformation(Type sourceType, string message)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceInformation(message);
			entry.TypeSource.TraceInformation(message);
		}

		[Conditional("TRACE")]
		public static void TraceInformation(Type sourceType, string format, params object[] args)
		{
			SourceEntry entry = GetEntry(sourceType);
			entry.NamespaceSource.TraceInformation(format, args);
			entry.TypeSource.TraceInformation(format, args);
		}

		[Conditional("TRACE")]
		public static void TraceError(Type sourceType, Exception exception, string message)
		{
			SourceEntry entry = GetEntry(sourceType);

			string logmessage = message + Environment.NewLine + exception.ToString();

			entry.NamespaceSource.TraceEvent(TraceEventType.Error, 0, logmessage);
			entry.TypeSource.TraceEvent(TraceEventType.Error, 0, logmessage);
		}

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
