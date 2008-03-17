/* 
 * Dependencies: System.Diagnostics
 * Authors: Daniel Cazzulino - daniel@cazzulino.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;

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
		[EditorBrowsable(EditorBrowsableState.Never)]
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
		/// <summary>
		/// Traces the given exception, using the format and arguments.
		/// </summary>
		void TraceError(Exception exception, string format, params object[] args);
		/// <summary>
		/// Traces the given exception and its corresponding message.
		/// </summary>
		void TraceError(Exception exception, string message);
		/// <summary>
		/// Traces a warning, using the format and arguments to build the message.
		/// </summary>
		void TraceWarning(string format, params object[] args);
		/// <summary>
		/// Traces a warning with the given message.
		/// </summary>
		void TraceWarning(string message);
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
		static Dictionary<Type, ITraceSource> cachedCompositeSources = new Dictionary<Type, ITraceSource>();
		static Dictionary<string, List<TraceListener>> additionalListeners = new Dictionary<string, List<TraceListener>>();
		static Dictionary<string, TraceSource> cachedBaseSources = new Dictionary<string, TraceSource>();

		/// <summary>
		/// See TraceSource.TraceData.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceData(Type sourceType, TraceEventType eventType, int id, params object[] data)
		{
			GetSourceFor(sourceType).TraceData(eventType, id, data);
		}

		/// <summary>
		/// See TraceSource.TraceData.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceData(Type sourceType, TraceEventType eventType, int id, object data)
		{
			GetSourceFor(sourceType).TraceData(eventType, id, data);
		}

		/// <summary>
		/// See TraceSource.TraceEvent.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id)
		{
			GetSourceFor(sourceType).TraceEvent(eventType, id);
		}

		/// <summary>
		/// See TraceSource.TraceEvent.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id, string message)
		{
			GetSourceFor(sourceType).TraceEvent(eventType, id, message);
		}

		/// <summary>
		/// See TraceSource.TraceEvent.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id, string format, params object[] args)
		{
			GetSourceFor(sourceType).TraceEvent(eventType, id, format, args);
		}

		/// <summary>
		/// See TraceSource.TraceInformation.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceInformation(Type sourceType, string message)
		{
			GetSourceFor(sourceType).TraceInformation(message);
		}

		/// <summary>
		/// See TraceSource.TraceInformation.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceInformation(Type sourceType, string format, params object[] args)
		{
			GetSourceFor(sourceType).TraceInformation(format, args);
		}

		/// <summary>
		/// See TraceSource.TraceError.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceError(Type sourceType, Exception exception, string message)
		{
			string logmessage = message + Environment.NewLine + exception.ToString();

			GetSourceFor(sourceType).TraceEvent(TraceEventType.Error, 0, logmessage);
		}

		/// <summary>
		/// See TraceSource.TraceError.
		/// </summary>
		[Conditional("TRACE")]
		public static void TraceError(Type sourceType, Exception exception, string format, params object[] args)
		{
		    string logmessage = format + Environment.NewLine + exception.ToString();

			GetSourceFor(sourceType).TraceEvent(TraceEventType.Error, 0, logmessage, args);
		}

		/// <summary>
		/// Retrieves a <see cref="ITraceSource"/> that can be 
		/// used by component of type <paramref name="sourceType"/> to issue 
		/// trace statements.
		/// </summary>
		internal static ITraceSource GetSourceFor(Type sourceType)
		{
			ITraceSource source;

			if (!cachedCompositeSources.TryGetValue(sourceType, out source))
			{
				lock (cachedCompositeSources)
				{
					if (!cachedCompositeSources.TryGetValue(sourceType, out source))
					{
						var parts = sourceType.FullName.Split('.');

						string sourceName = parts[0];
						var innerSources = new List<TraceSource>();

						innerSources.Add(GetTraceSource(sourceName));

						for (int i = 1; i < parts.Length; i++)
						{
							sourceName += "." + parts[i];
							innerSources.Add(GetTraceSource(sourceName));
						}

						source = new CompositeTraceSource(innerSources);
						cachedCompositeSources.Add(sourceType, source);
					}
				}
			}

			return source;
		}

		/// <summary>
		/// Retrieves a <see cref="ITraceSource"/> that can be 
		/// used by component <typeparamref name="T"/> to issue 
		/// trace statements.
		/// </summary>
		/// <typeparam name="T">Type of the component that will perform the logging.</typeparam>
		internal static ITraceSource GetSourceFor<T>()
		{
			return GetSourceFor(typeof(T));
		}

		private static TraceSource GetTraceSource(string sourceName)
		{
			TraceSource source;
			if (!cachedBaseSources.TryGetValue(sourceName, out source))
			{
				lock (cachedBaseSources)
				{
					if (!cachedBaseSources.TryGetValue(sourceName, out source))
					{
						source = AddAdditionalListeners(new TraceSource(sourceName));
						cachedBaseSources.Add(sourceName, source);
					}
				}
			}

			return source;
		}

		private static TraceSource AddAdditionalListeners(TraceSource source)
		{
			List<TraceListener> additional;
			if (additionalListeners.TryGetValue(source.Name, out additional))
			{
				additional.ForEach(listener => source.Listeners.Add(listener));
			}

			return source;
		}

		internal static void AddListener(string sourceName, TraceListener listener)
		{
			var query = (from keyPair in cachedCompositeSources
						where keyPair.Key.FullName.StartsWith(sourceName)
						from source in keyPair.Value.Sources
						where source.Name == sourceName
						select source).ToList();

			query.ForEach(source => source.Listeners.Add(listener));

			List<TraceListener> listeners;

			if (!additionalListeners.TryGetValue(sourceName, out listeners))
			{
				lock (additionalListeners)
				{
					if (!additionalListeners.TryGetValue(sourceName, out listeners))
					{
						listeners = new List<TraceListener>();
						additionalListeners.Add(sourceName, listeners);
					}
				}
			}

			listeners.Add(listener);
		}

		internal static void SetLoggingLevel(string sourceName, SourceLevels sourceLevels)
		{
			TraceSource source = GetTraceSource(sourceName);

			source.Switch.Level = sourceLevels;
		}

		private class CompositeTraceSource : ITraceSource
		{
			List<TraceSource> sources;

			public CompositeTraceSource(List<TraceSource> sources)
			{
				this.sources = sources;
			}

			public IEnumerable<TraceSource> Sources
			{
				get { return sources; }
			}

			public void Flush()
			{
				sources.ForEach(source => source.Flush());
			}

			public void TraceData(TraceEventType eventType, int id, object data)
			{
				sources.ForEach(source => source.TraceData(eventType, id, data));
			}

			public void TraceData(TraceEventType eventType, int id, params object[] data)
			{
				sources.ForEach(source => source.TraceData(eventType, id, data));
			}

			public void TraceEvent(TraceEventType eventType, int id)
			{
				sources.ForEach(source => source.TraceEvent(eventType, id));
			}

			public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
			{
				sources.ForEach(source => source.TraceEvent(eventType, id, format, args));
			}

			public void TraceEvent(TraceEventType eventType, int id, string message)
			{
				sources.ForEach(source => source.TraceEvent(eventType, id, message));
			}

			public void TraceInformation(string message)
			{
				sources.ForEach(source => source.TraceInformation(message));
			}

			public void TraceInformation(string format, params object[] args)
			{
				sources.ForEach(source => source.TraceInformation(format, args));
			}

			public void TraceTransfer(int id, string message, Guid relatedActivityId)
			{
				sources.ForEach(source => source.TraceTransfer(id, message, relatedActivityId));
			}

			public void TraceError(Exception exception, string message)
			{
				string logmessage = message + Environment.NewLine + exception.ToString();

				sources.ForEach(source => source.TraceEvent(TraceEventType.Error, 0, logmessage));
			}

			public void TraceError(Exception exception, string format, params object[] args)
			{
				string logmessage = format + Environment.NewLine + exception.ToString();

				sources.ForEach(source => source.TraceEvent(TraceEventType.Error, 0, logmessage, args));
			}

			public void TraceWarning(string format, params object[] args)
			{
				sources.ForEach(source => source.TraceEvent(TraceEventType.Warning, 0, format, args));
			}

			public void TraceWarning(string message)
			{
				sources.ForEach(source => source.TraceEvent(TraceEventType.Warning, 0, message));
			}
		}
	}
}
