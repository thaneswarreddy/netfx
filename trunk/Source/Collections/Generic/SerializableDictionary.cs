using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

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
		private static readonly XmlSerializerFactory serializerFactory = new XmlSerializerFactory();
		private static readonly XmlSerializerNamespaces serializerNamespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

		/// <summary>
		/// Initializes an instance of the dictionary, setting the 
		/// <see cref="XmlNamespaceURI"/> to an empty string.
		/// </summary>
		public SerializableDictionary()
		{
			XmlNamespaceURI = "";
		}

		/// <summary>
		/// Allows overriding of the xml namespace used to serialize 
		/// child elements.
		/// </summary>
		public string XmlNamespaceURI { get; set; }

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
			if (reader.ReadState == ReadState.Initial)
				reader.MoveToContent();

			return reader.NodeType == XmlNodeType.Element &&
				reader.LocalName == "dictionary" &&
				reader.NamespaceURI == "";
		}

		public static SerializableDictionary<TKey, TValue> ReadXml(XmlReader reader)
		{
			if (!CanRead(reader))
				XmlExceptions.ThrowXmlException(String.Format(
					"Expected element <{0} xmlns='{1}' ...>.", 
					reader.LocalName, reader.NamespaceURI),
					reader);

			// TODO: we could add a constraint to verify that the TKey and TValue types 
			// match (or are base classes of?) the same attributes on the xml, so as to 
			// guarantee successful deserialization.
			//var keyType = Type.GetType(reader.GetAttribute(XmlNames.AttributeNames.TKey));
			//var valueType = Type.GetType(reader.GetAttribute(XmlNames.AttributeNames.TValue));

			var extensions = new SerializableDictionary<TKey, TValue>();
			((IXmlSerializable)extensions).ReadXml(reader);

			return extensions;
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("dictionary", this.XmlNamespaceURI);
			((IXmlSerializable)this).WriteXml(writer);
			writer.WriteEndElement();
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader.ReadToDescendant("entry", XmlNamespaceURI))
			{
				var depth = reader.Depth;
				do
				{
					using (var entry = reader.ReadSubtree())
					{
						if (reader.ReadToDescendant("key", XmlNamespaceURI))
						{
							bool skip = false;
							var tKey = ReadType(reader, typeof(TKey), out skip);
							if (skip) continue;

							var keySerializer = serializerFactory.CreateSerializer(tKey, new XmlRootAttribute("key"));
							var key = keySerializer.Deserialize(reader.ReadSubtree());

							if (reader.ReadToNextSibling("value", XmlNamespaceURI))
							{
								var tValue = ReadType(reader, typeof(TValue), out skip);
								if (skip) continue;

								var valueSerializer = serializerFactory.CreateSerializer(tValue, new XmlRootAttribute("value"));
								var value = valueSerializer.Deserialize(reader.ReadSubtree());

								Add((TKey)key, (TValue)value);
							}
						}
					}
				} while (reader.Read() && reader.Depth >= depth);
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			var tKey = typeof(TKey);
			var tValue = typeof(TValue);
			foreach (var item in this)
			{
				writer.WriteStartElement("entry");

				// Serialize Key
				var keyType = item.Key.GetType();
				var keyWriter = keyType == tKey ? writer : new TypeWriter(writer, keyType);
				var keySerializer = serializerFactory.CreateSerializer(keyType, new XmlRootAttribute("key") { Namespace = XmlNamespaceURI });
				keySerializer.Serialize(keyWriter, item.Key, serializerNamespaces);

				// Serialize Value
				var valueType = item.Value != null ? item.Value.GetType() : tValue;
				var valueWriter = valueType == tValue ? writer : new TypeWriter(writer, valueType);
				var valueSerializer = serializerFactory.CreateSerializer(valueType, new XmlRootAttribute("value") { Namespace = XmlNamespaceURI });
				valueSerializer.Serialize(valueWriter, item.Value, serializerNamespaces);

				writer.WriteEndElement();
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

				if (type == null || !defaultType.IsAssignableFrom(type))
					shouldSkip = true;

				if (type != null)
					result = type;
			}

			return result;
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
	}
}
