using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Reflection.Emit;
using System.Reflection;

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

            using (var writer = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true }))
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

        [Test]
        public void ShouldOverrideElementAndNamespace()
        {
            var root = new Root
            {
                Extensions =
				{
					{ "foo", 25 },
					{ "bar", true },
				}
            };

            var serializer = new XmlSerializer(typeof(Root));
            var mem = new MemoryStream();

            using (var writer = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true }))
            {
                serializer.Serialize(writer, root);
            }

			Console.WriteLine(new string('-', 50));

            using (var writer = XmlWriter.Create(mem, new XmlWriterSettings { Indent = true }))
            {
                serializer.Serialize(writer, root);
            }

            mem.Position = 0;
            var doc = new XmlDocument();
            doc.Load(mem);

            var mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("m", "xml-mvp");

            Assert.AreEqual(2, doc.SelectNodes("/m:root/m:extensions/m:entry/m:key", mgr).Count);
            Assert.AreEqual(2, doc.SelectNodes("/m:root/m:extensions/m:entry/m:value", mgr).Count);
        }

		[Test]
		public void ShouldNotLeakMemory()
		{
			var root = new Root
			{
				Extensions =
				{
					{ "foo", 25 },
					{ "bar", true },
					{ "baz", "hello" },
					{ "1", EnvironmentVariableTarget.Machine },
					{ "2", PlatformID.Xbox },
				}
			};

			var serializer = new XmlSerializer(typeof(Root));
			var mem = new MemoryStream();

			var total = GC.GetTotalMemory(true);

			using (var writer = XmlWriter.Create(mem, new XmlWriterSettings { Indent = true }))
			{
				serializer.Serialize(writer, root);
			}

			var total2 = GC.GetTotalMemory(true);

			for (int i = 0; i < 1000; i++)
			{
				mem = new MemoryStream();
				using (var writer = XmlWriter.Create(mem, new XmlWriterSettings { Indent = true }))
				{
					serializer.Serialize(writer, root);
				}
			}

			var total3 = GC.GetTotalMemory(true);

			// Initial memory preassure on first serialization creation should not be bigger than 10% initial.
			Assert.That(total2 < (total * 1.1));
			// Increment on memory usage after 1k runs should not be greater than 5%
			Assert.That(Math.Abs(total3 - total2) < (total2 * 0.05) );
		}

        [Test]
        public void ShouldOverrideElementAndNamespaceForDeserialization()
        {
            var xml = @"<extensions xmlns='foo-xml'>
    <entry>
      <key>foo</key>
      <value type='System.Int32'>25</value>
    </entry>
    <entry>
      <key>bar</key>
      <value type='System.Boolean'>true</value>
    </entry>
  </extensions>";

            var serializer = new XmlSerializer(typeof(SerializableDictionary<string, object>), new XmlRootAttribute("extensions") { Namespace = "foo-xml" });
            var dictionary = (SerializableDictionary<string, object>)serializer.Deserialize(new StringReader(xml));

            Assert.AreEqual(2, dictionary.Count);
        }

        [Test]
        public void ShouldIgnorePrivateClassExtension()
        {
            var dictionary = new SerializableDictionary<string, object>();

            dictionary.Add("foo", new PrivateClass());

            var serializer = new XmlSerializer(dictionary.GetType());

            using (var writer = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true }))
            {
                serializer.Serialize(writer, dictionary);
            }

            var mem = new MemoryStream();
            serializer.Serialize(mem, dictionary);

            mem.Position = 0;

            var deserialized = (Dictionary<string, object>)serializer.Deserialize(mem);

            Assert.AreEqual(0, deserialized.Count);
        }

        [Test]
        public void ShouldIgnorePublicClassNoParameterlessCtor()
        {
            var dictionary = new SerializableDictionary<string, object>();

            dictionary.Add("foo", new PublicClassNoCtor(true));

            var serializer = new XmlSerializer(dictionary.GetType());

            using (var writer = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true }))
            {
                serializer.Serialize(writer, dictionary);
            }

            var mem = new MemoryStream();
            serializer.Serialize(mem, dictionary);

            mem.Position = 0;

            var deserialized = (Dictionary<string, object>)serializer.Deserialize(mem);

            Assert.AreEqual(0, deserialized.Count);
        }

        [XmlRoot("root", Namespace = "xml-mvp")]
        public class Root
        {
            public Root()
            {
                Extensions = new SerializableDictionary<string, object>();
                Extensions.XmlRoot.Namespace = "xml-mvp";
            }

            [XmlElement("extensions", Namespace = "xml-mvp")]
            public SerializableDictionary<string, object> Extensions { get; set; }
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

        private class PrivateClass
        {
        }

        public class PublicClassNoCtor
        {
            public PublicClassNoCtor(bool value)
            {
            }
        }
    }
}
