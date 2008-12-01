using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection.Emit;
using System.Reflection;

namespace System.Collections.Generic
{
	/// <summary>
	/// Implements a <see cref="Dictionary{TKey, TValue}"/> that can be safely 
	/// serialized to XML and deserialized back, preserving type information.
	/// </summary>
	/// <remarks>
	/// The serialization format will attempt to write the minimal information possible. 
	/// Typical format is as follows:
	/// <code>
	/// <dictionary>
	///   <entry>
	///     <key>foo</key>
	///     <value>25</value>
	///   </entry>
	///   <entry>
	///     <key>bar</key>
	///     <value>30</value>
	///   </entry>
	/// </dictionary>
	/// </code>
	/// The type of the key and the value are the same as the ones for 
	/// <typeparamref name="TKey"/> and <typeparamref name="TValue"/>. 
	/// If the type of a value for either one is a derived type, the 
	/// type information will be written in a <c>type</c> attribute, 
	/// which will be used to deserialize the XML with the appropriate 
	/// <see cref="XmlSerializer"/>. The serialized type name does not 
	/// include assembly version information, to make it more version-resilient.
	/// </remarks>
	[XmlRoot("dictionary")]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
	{
#if NET20
		internal delegate T Func<T>();
#endif

		private static readonly Dictionary<CacheKey, Func<XmlSerializer>> keySerializers = new Dictionary<CacheKey, Func<XmlSerializer>>();
		private static readonly Dictionary<CacheKey, Func<XmlSerializer>> valueSerializers = new Dictionary<CacheKey, Func<XmlSerializer>>();

		private static readonly XmlSerializerFactory serializerFactory = new XmlSerializerFactory();
		private static readonly XmlSerializerNamespaces serializerNamespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

		/// <summary>
		/// Initializes an instance of the dictionary, setting the 
		/// <see cref="XmlRoot"/> namespace to an empty string.
		/// </summary>
		public SerializableDictionary()
		{
			XmlRoot = new XmlRootAttribute("dictionary") { Namespace = "" };
		}

		/// <summary>
		/// Allows overriding of the xml namespace used to serialize 
		/// child elements.
		/// </summary>
		public XmlRootAttribute XmlRoot { get; set; }

		/// <summary>
		/// Tests whether an extensions dictionary can be read from the current
		/// <see cref="XmlReader"/> position using the default empty XML namespace Uri.
		/// </summary>
		/// <remarks>
		/// If the reader is in the <see cref="ReadState.Initial"/>, it's advanced 
		/// to the content for the check.
		/// </remarks>
		public static bool CanRead(XmlReader reader)
		{
			return CanRead(reader, new XmlRootAttribute("dictionary"));
		}

		/// <summary>
		/// Tests whether an extensions dictionary can be read from the current
		/// <see cref="XmlReader"/> position using the given root element information.
		/// </summary>
		/// <remarks>
		/// If the reader is in the <see cref="ReadState.Initial"/>, it's advanced 
		/// to the content for the check.
		/// </remarks>
		public static bool CanRead(XmlReader reader, XmlRootAttribute xmlRoot)
		{
			if (reader.ReadState == ReadState.Initial)
				reader.MoveToContent();

			return reader.NodeType == XmlNodeType.Element &&
				reader.LocalName == xmlRoot.ElementName &&
				reader.NamespaceURI == xmlRoot.Namespace;
		}

		/// <summary>
		/// Reads the dictionary using the default root element name and namespace.
		/// </summary>
		public static SerializableDictionary<TKey, TValue> ReadXml(XmlReader reader)
		{
			return ReadXml(reader, new XmlRootAttribute("dictionary"));
		}

		/// <summary>
		/// Reads the dictionary using the given root element override.
		/// </summary>
		public static SerializableDictionary<TKey, TValue> ReadXml(XmlReader reader, XmlRootAttribute xmlRoot)
		{
			if (!CanRead(reader, xmlRoot))
				XmlExceptions.ThrowXmlException(String.Format(
					"Unexpected element <{0} xmlns='{1}' ...>.",
					reader.LocalName, reader.NamespaceURI),
					reader);

			var extensions = new SerializableDictionary<TKey, TValue>();
			((IXmlSerializable)extensions).ReadXml(reader);

			return extensions;
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(XmlRoot.ElementName, XmlRoot.Namespace);
			((IXmlSerializable)this).WriteXml(writer);
			writer.WriteEndElement();
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			var rootNamespaceUri = reader.NamespaceURI;
			if (reader.ReadToDescendant("entry", rootNamespaceUri))
			{
				var depth = reader.Depth;
				do
				{
					using (var entry = reader.ReadSubtree())
					{
						if (reader.ReadToDescendant("key", rootNamespaceUri))
						{
							bool skip = false;
							var tKey = ReadType(reader, typeof(TKey), out skip);
							if (skip) continue;

							var keySerializer = GetSerializer(keySerializers, tKey, new XmlRootAttribute("key") { Namespace = rootNamespaceUri });
							var key = keySerializer.Deserialize(reader.ReadSubtree());

							if (reader.ReadToNextSibling("value", rootNamespaceUri))
							{
								var tValue = ReadType(reader, typeof(TValue), out skip);
								if (skip) continue;

								var valueSerializer = GetSerializer(valueSerializers, tValue, new XmlRootAttribute("value") { Namespace = rootNamespaceUri });
								var value = valueSerializer.Deserialize(reader.ReadSubtree());

								Add((TKey)key, (TValue)value);
							}
							else
							{
								Add((TKey)key, default(TValue));
							}
						}
					}
				} while (reader.Read() && reader.MoveToContent() == XmlNodeType.Element && reader.Depth >= depth);
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			var tKey = typeof(TKey);
			var tValue = typeof(TValue);
			foreach (var item in this)
			{
				try
				{
					// Ensure we can create the serializers first for the types
					var keyType = item.Key.GetType();
					var valueType = item.Value != null ? item.Value.GetType() : tValue;
					
					// Optimize if we know the data is not serializable up front.
					if (!
						(keyType.IsPublic || keyType.IsNestedPublic) && 
						(valueType.IsPublic || valueType.IsNestedPublic))
						continue;
		
					var keyWriter = keyType == tKey ? writer : new TypeWriter(writer, keyType);
					keyWriter = new NonXsiXmlWriter(keyWriter);
					var keySerializer = GetSerializer(keySerializers, keyType, new XmlRootAttribute("key") { Namespace = XmlRoot.Namespace });

					if (item.Value != null)
					{
						var valueWriter = valueType == tValue ? writer : new TypeWriter(writer, valueType);
						valueWriter = new NonXsiXmlWriter(valueWriter);
						var valueSerializer = GetSerializer(valueSerializers, valueType, new XmlRootAttribute("value") { Namespace = XmlRoot.Namespace });

						writer.WriteStartElement("entry");
						// Serialize Key
						keySerializer.Serialize(keyWriter, item.Key, serializerNamespaces);
						keyWriter.Flush();

						// Serialize Value
						valueSerializer.Serialize(valueWriter, item.Value, serializerNamespaces);
						valueWriter.Flush();

						// Make sure we close the entry tag.
						writer.WriteEndElement();
					}
					else
					{
						writer.WriteStartElement("entry");
						// Serialize Key
						keySerializer.Serialize(keyWriter, item.Key, serializerNamespaces);
						keyWriter.Flush();

						// Make sure we close the entry tag.
						writer.WriteEndElement();
					}
				}
				catch (Exception)
				{
				}
			}
		}

		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		private Type ReadType(XmlReader reader, Type defaultType, out bool shouldSkip)
		{
			var result = defaultType;
			shouldSkip = false;

			var typeName = reader.GetAttribute("type");
			if (!String.IsNullOrEmpty(typeName))
			{
				var type = Type.GetType(typeName);

				if (type == null || !defaultType.IsAssignableFrom(type) || 
					!(type.IsPublic || type.IsNestedPublic))
					shouldSkip = true;

				if (type != null)
					result = type;
			}

			return result;
		}

		private XmlSerializer GetSerializer(Dictionary<CacheKey, Func<XmlSerializer>> cache, Type forType, XmlRootAttribute root)
		{
			Func<XmlSerializer> factory;
			var key = new CacheKey { Type = forType, Root = root };
			if (!cache.TryGetValue(key, out factory))
			{
				var serializer = serializerFactory.CreateSerializer(forType, root);
				factory = BuildSerializerFactory(serializer.GetType());
				cache[key] = factory;
			}

			return factory();
		}

		private Func<XmlSerializer> BuildSerializerFactory(Type serializerType)
		{
			// generate new serializerType() anonymous factory function.
			var method = new DynamicMethod("KeyString", serializerType, null);

			// Preparing Reflection instances
			var ctor = serializerType.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, new Type[] { }, null);
			// Setting return type
			// Adding parameters
			ILGenerator gen = method.GetILGenerator();
			// Preparing locals
			LocalBuilder lb = gen.DeclareLocal(serializerType);
			// Preparing labels
			Label label9 = gen.DefineLabel();
			// Writing body
			gen.Emit(OpCodes.Nop);
			gen.Emit(OpCodes.Newobj, ctor);
			gen.Emit(OpCodes.Stloc_0);
			gen.Emit(OpCodes.Br_S, label9);
			gen.MarkLabel(label9);
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Ret);
			// finished

			return (Func<XmlSerializer>)method.CreateDelegate(typeof(Func<XmlSerializer>));
		}

		private class TypeWriter : XmlWrappingWriter
		{
			bool root = true;
			Type type;

			public TypeWriter(XmlWriter baseWriter, Type type)
				: base(baseWriter)
			{
				this.type = type;
			}

			public override void WriteStartElement(string prefix, string localName, string ns)
			{
				base.WriteStartElement(prefix, localName, ns);

				if (root)
				{
					var typeName = Type.GetType(type.FullName) != null ?
						type.FullName :
						type.FullName + ", " + type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(",")).Trim();

					base.WriteAttributeString("type", typeName);
					root = false;
				}
			}
		}

		private class NonXsiXmlWriter : XmlWrappingWriter
		{
			bool skip = false;

			public NonXsiXmlWriter(XmlWriter baseWriter)
				: base(baseWriter)
			{
			}

			public override void WriteStartAttribute(string prefix, string localName, string ns)
			{
				if (prefix == "xmlns" && (localName == "xsd" || localName == "xsi"))
				{
					skip = true;
					return;
				}

				base.WriteStartAttribute(prefix, localName, ns);
			}

			public override void WriteString(string text)
			{
				if (skip)
					return;

				base.WriteString(text);
			}

			public override void WriteEndAttribute()
			{
				if (skip)
				{
					skip = false;
					return;
				}

				base.WriteEndAttribute();
			}
		}

		private static class XmlExceptions
		{
			internal static void ThrowIfAttributeMissing(string attributeName, XmlReader reader)
			{
				if (String.IsNullOrEmpty(reader.GetAttribute(attributeName)))
				{
					ThrowXmlException(
						String.Format(
							"Attribute '{0}' is required.",
							attributeName),
						reader);
				}
			}

			internal static void ThrowXmlException(string message, XmlReader reader)
			{
				int lineNumber = -1;
				int linePosition = -1;

				var info = reader as IXmlLineInfo;
				if (info != null && info.HasLineInfo())
				{
					lineNumber = info.LineNumber;
					linePosition = info.LinePosition;
				}

				var summary = new StringBuilder();
				if (reader.NodeType != XmlNodeType.Element)
					reader.MoveToElement();

				using (XmlWriter w = XmlWriter.Create(summary, new XmlWriterSettings { OmitXmlDeclaration = true }))
				{
					w.WriteNode(new SummaryXmlReader(
						reader.LocalName, reader.NamespaceURI, reader.ReadSubtree()),
						false);
				}

				throw new XmlException(
					String.Format(
						@"{0}
There is an error in the XML document or fragment:
{1}",
						message,
						summary.ToString().Substring(0, summary.ToString().Length - 2).Trim() + ">"),
					null,
					lineNumber,
					linePosition);
			}

			class SummaryXmlReader : XmlWrappingReader
			{
				string emptyLocalName;
				string namespaceUri;
				bool eof;

				public SummaryXmlReader(string emptyLocalName, string namespaceUri, XmlReader baseReader)
					: base(baseReader)
				{
					this.emptyLocalName = emptyLocalName;
					this.namespaceUri = namespaceUri;
				}

				public new XmlReader MoveToContent()
				{
					base.MoveToContent();
					return this;
				}

				public override bool Read()
				{
					if (LocalName == emptyLocalName &&
						NamespaceURI == namespaceUri &&
						NodeType == XmlNodeType.Element)
					{
						eof = true;
					}

					return !eof && base.Read();
				}

				public override bool IsEmptyElement
				{
					get
					{
						if (LocalName == emptyLocalName && NamespaceURI == namespaceUri)
							return true;
						else
							return base.IsEmptyElement;
					}
				}
			}
		}

		private class CacheKey
		{
			public Type Type;
			public XmlRootAttribute Root;

			public override bool Equals(object obj)
			{
				if (Object.ReferenceEquals(this, obj)) return true;

				var x = this;
				var y = obj as CacheKey;

				if (Object.Equals(null, y)) return false;

				return x.Type == y.Type &&
					x.Root.ElementName == y.Root.ElementName &&
					x.Root.Namespace == y.Root.Namespace;
			}

			public override int GetHashCode()
			{
				return this.Type.GetHashCode() ^ this.Root.ElementName.GetHashCode() ^ this.Root.Namespace.GetHashCode();
			}
		}
	}
}
