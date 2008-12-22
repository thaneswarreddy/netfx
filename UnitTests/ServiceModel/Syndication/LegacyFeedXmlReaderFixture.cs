using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Xml;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml.Xsl;
using NetFx.ServiceModel.Syndication;

namespace NetFx.UnitTests.ServiceModel.Syndication
{
	[TestFixture]
	public class LegacyFeedXmlReaderFixture
	{
		[Test]
		public void ShouldUpgradeFeed()
		{
			using (var feedFile = File.OpenRead(".\\ServiceModel\\Syndication\\Atom0.3-GoogleNews.xml"))
			{
				using (var baseReader = XmlReader.Create(feedFile))
				{
					var atomReader = new LegacyFeedXmlReader(baseReader);

					var feed = SyndicationFeed.Load(atomReader);

					Assert.IsNotNull(feed);
					Assert.AreEqual("Google News Argentina", feed.Title.Text);
					Assert.AreEqual("news-feedback@google.com", feed.Authors[0].Email);
					Assert.AreEqual(26, feed.Items.Count());
				}
			}
		}

		[Test]
		public void ShouldUpgradeFeedXslt()
		{
			var xslt = new XslCompiledTransform();
			xslt.Load(typeof(Atom03ToRss20XslTransform));

			using (var feedFile = File.OpenRead(".\\ServiceModel\\Syndication\\Atom0.3-GoogleNews.xml"))
			{
				using (var baseReader = XmlReader.Create(feedFile))
				{
					using (var writer = XmlWriter.Create("..\\..\\ServiceModel\\Syndication\\Atom1.0-GoogleNews.xml", new XmlWriterSettings { Indent = true }))
					{
						xslt.Transform(baseReader, writer);
					}
				}
			}
		}
	}
}
