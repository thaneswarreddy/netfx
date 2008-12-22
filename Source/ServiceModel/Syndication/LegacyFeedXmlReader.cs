// Depends on:
//		- \Xml\XmlWrappingReader
//		- \ServiceModel\Syndication\Atom03ToRss20.dll
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using NetFx.ServiceModel.Syndication;
using System.IO;

namespace System.ServiceModel.Syndication
{
#if NetFx	
	public class LegacyFeedXmlReader : XmlWrappingReader
#else
	internal class LegacyFeedXmlReader : XmlWrappingReader
#endif
	{
		static XslCompiledTransform atom03ToRss20;
		MemoryStream mem;

		static LegacyFeedXmlReader()
		{
			atom03ToRss20 = new XslCompiledTransform();
			atom03ToRss20.Load(typeof(Atom03ToRss20XslTransform));
		}

		public LegacyFeedXmlReader(XmlReader baseReader)
			: base(baseReader)
		{
			if (baseReader.ReadState == ReadState.Initial)
				baseReader.MoveToContent();

			UpgradeReader();
		}

		private void UpgradeReader()
		{
			if (BaseReader.NamespaceURI == "http://purl.org/atom/ns#")
			{
				var doc = new XPathDocument(BaseReader);
				mem = new MemoryStream();
				using (var writer = XmlWriter.Create(mem))
				{
					atom03ToRss20.Transform(doc, writer);
				}

				mem.Position = 0;
				BaseReader = XmlReader.Create(mem, BaseReader.Settings);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (mem != null)
			{
				mem.Dispose();
			}
		}
	}
}
