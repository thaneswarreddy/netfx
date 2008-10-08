using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace NetFx.UnitTests.Collections.Generic
{
	[TestFixture]
	public class SerializableDictionaryFixture
	{
		[ExpectedException(typeof(XmlException))]
		[Test]
		public void ShouldThrowIfInvalidRoot()
		{
			var xml = @"
<foo>
	<entry>
		<key type='System.String'>foo</key>
		<value type='System.Boolean'>true</value>
	</entry>
</foo>";

			var dictionary = SerializableDictionary<object, object>.ReadXml(XmlReader.Create(new StringReader(xml)));
		}

		[Test]
		public void ShouldReadXml()
		{
			var xml = @"
<dictionary>
	<entry>
		<key type='System.String'>foo</key>
		<value type='System.Boolean'>true</value>
	</entry>
	<entry>
		<key type='System.Int32'>25</key>
		<value type='System.String'>foo</value>
	</entry>
</dictionary>";

			var serializer = new XmlSerializer(typeof(SerializableDictionary<object, object>));
			var dictionary = (Dictionary<object, object>)serializer.Deserialize(new StringReader(xml));

			Assert.AreEqual(true, dictionary["foo"]);
			Assert.AreEqual("foo", dictionary[25]);
		}
		
		[Test]
		public void ShouldRoundtripXml()
		{
			var dictionary = new SerializableDictionary<string, object>();

			dictionary.Add("foo", new Foo
			{
				Name = "name",
				Bar = new Bar
				{
					Value = "value",
				}, 
			});

			var serializer = new XmlSerializer(dictionary.GetType());

			using (var writer = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true }))
			{
				serializer.Serialize(writer, dictionary);
			}

			var mem = new MemoryStream();
			serializer.Serialize(mem, dictionary);

			mem.Position = 0;

			var deserialized = (Dictionary<string, object>)serializer.Deserialize(mem);

			Assert.AreEqual(dictionary.Count, deserialized.Count);
		}

		[Test]
		public void ShouldSerializeString()
		{
			var xml = "<key type='System.Int32'>25</key>";
			var reader = XmlReader.Create(new StringReader(xml));

			reader.MoveToContent();
			var type = Type.GetType(reader.GetAttribute("type"));

			var serializer = new XmlSerializer(type, new XmlRootAttribute("key"));

			using (var writer = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true}))
			{
				serializer.Serialize(writer, 25);				
			}

			var foo = serializer.Deserialize(reader);

			Assert.AreEqual(25, foo);
		}

		[Test]
		public void ShouldIgnoreElementsWithUnparseableTypes()
		{
			var xml = @"
<dictionary>
	<entry>
		<key type='FooBar'>foo</key>
		<value type='System.Boolean'>true</value>
	</entry>
	<entry>
		<key type='System.Int32'>25</key>
		<value type='System.String'>foo</value>
	</entry>
</dictionary>";

			var serializer = new XmlSerializer(typeof(SerializableDictionary<object, object>));
			var dictionary = (Dictionary<object, object>)serializer.Deserialize(new StringReader(xml));

			Assert.AreEqual(1, dictionary.Count);
			Assert.AreEqual("foo", dictionary[25]);
		}

		[Test]
		public void ShouldIgnoreElementsWithIncompatibleTypes()
		{
			var xml = @"
<dictionary>
	<entry>
		<key>foo</key>
		<value type='System.Boolean'>true</value>
	</entry>
	<entry>
		<key type='System.Int32'>25</key>
		<value type='System.String'>foo</value>
	</entry>
</dictionary>";

			var serializer = new XmlSerializer(typeof(SerializableDictionary<string, object>));
			var dictionary = (Dictionary<string, object>)serializer.Deserialize(new StringReader(xml));

			Assert.AreEqual(1, dictionary.Count);
			Assert.AreEqual(true, dictionary["foo"]);
		}

		public class Foo
		{
			public string Name { get; set; }
			public Bar Bar { get; set; }
		}

		public class Bar
		{
			public string Value { get; set; }
		}
	}
}
