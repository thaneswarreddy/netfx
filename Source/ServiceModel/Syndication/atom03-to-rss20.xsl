<?xml version="1.0" encoding='iso-8859-1'?>
<xsl:stylesheet 
	        xmlns:admin="http://webns.net/mvcb/"
		xmlns:content="http://purl.org/rss/1.0/modules/content/"
		xmlns:dc="http://purl.org/dc/elements/1.1/" 
		xmlns:doc="http://xsltsl.org/xsl/documentation/1.0"
		xmlns:feed="http://purl.org/atom/ns#"
		xmlns:foaf="http://xmlns.com/foaf/0.1/"
		xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" 
                xmlns:xml="http://www.w3.org/XML/1998/namespace"
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
		exclude-result-prefixes=" doc feed"
		version = "1.0" >

<!-- ====================================================================== 

     ====================================================================== -->

     <doc:reference>
      <referenceinfo>

       <title>atom03-to-rss20.xsl</title>

       <abstract>
        <para>This is an <acronym>XSLT</acronym> stylesheet for transforming an
        Atom 0.3 document in to a <acronym>RSS</acronym> 2.0 document.</para>
       </abstract>

       <author>
	<surname>Cope</surname>
	<firstname>Aaron</firstname>
	<othername>Straup</othername>
       </author>

       <legalnotice>
        <para>This work is licensed under the Creative Commons
        Attribution-ShareAlike License. To view a copy of this
        license, visit http://creativecommons.org/licenses/by-sa/1.0/
        or send a letter to Creative Commons, 559 Nathan Abbott Way,
        Stanford, California 94305, USA.</para> 
       </legalnotice>

       <revhistory>
        <revision>
	 <revnumber>0.99</revnumber>
	 <date>January 25, 2004</date>
	 <revremark>
	  <para>Initial release. It works but the docs are incomplete.</para>
         </revremark>
	</revision>
       </revhistory>

      </referenceinfo>

      <partintro>
       <section>
        <title>Usage</title>
	<para>Consult the documentation for your favourite
       <acronym>XSLT</acronym> processor. This template does not
       accept any parameters.</para>
       </section>

       <section>
        <title>Requirements</title>

	<para>This stylesheet requires version 1.1 of the <ulink href =
        "http://xsltsl.sourceforge.net"><acronym>XSLTSL</acronym>
        Standard Library</ulink> templates:</para>

	<itemizedlist>
	 <listitem>
  	  <para>In <filename>shared/rss.xsl</filename>, you will
	 need to, manually, update the path to the
	 <filename>markup.xsl</filename> stylesheet to reflect its
	 location on the machine processing this stylesheet.</para> 
         </listitem>
	 <listitem>
  	  <para>In <filename>shared/rss20.xsl</filename>, you will
	 need to, manually, update the path to the
	 <filename>date-time.xsl</filename> stylesheet to reflect its
	 location on the machine processing this stylesheet.</para> 
         </listitem>
	</itemizedlist>

       </section>

      </partintro>

     </doc:reference>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:output method = "xml" />
     <xsl:output indent = "yes" />

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:include href = "shared/rss20.xsl" />

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template match="/">
      <rss>
       <xsl:attribute name = "version">2.0</xsl:attribute>

       <channel>
        <xsl:call-template name = "Channel" />
        <xsl:call-template name = "Items" />
       </channel>
      </rss>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "Channel">

      <title>
       <xsl:call-template name = "channel_title" />
      </title>

      <link>
       <xsl:call-template name = "channel_link" />
      </link>

      <description>
       <xsl:call-template name = "channel_description" />
      </description>

      <xsl:call-template name = "channel_creator" />

      <xsl:call-template name = "channel_lastbuilddate" />

      <xsl:call-template name = "channel_generator" />
      <xsl:call-template name = "channel_license" />

     </xsl:template>

     <xsl:template name = "Items">
      <xsl:for-each select = "/feed:feed/feed:entry">

      <item>

      <title>
       <xsl:call-template name = "item_title" />
      </title>

      <link>
       <xsl:call-template name = "item_link" />
      </link>

      <description>
       <xsl:call-template name = "item_description" />
      </description>

      <guid>
       <xsl:value-of select = "feed:id" />
      </guid>

      <pubDate>
       <xsl:call-template name = "w3cdtf_to_rfc822">
        <xsl:with-param name = "w3cdtf" select = "feed:issued" />
       </xsl:call-template>
      </pubDate>
				   
      <xsl:call-template name = "item_created" />
      <xsl:call-template name = "item_lastmodified" />

      <xsl:call-template name = "item_creator" />
      <xsl:call-template name = "item_contributors" />
      <xsl:call-template name = "item_content" />

     </item>

    </xsl:for-each>
   </xsl:template>

</xsl:stylesheet>

<!-- ====================================================================== 
     FIN // $Id: atom03-to-rss20.xsl,v 1.6 2004/01/25 20:47:32 asc Exp $
     ====================================================================== -->
