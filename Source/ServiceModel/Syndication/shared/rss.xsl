<?xml version="1.0" encoding='iso-8859-1'?>
<xsl:stylesheet xmlns:admin="http://webns.net/mvcb/"
		xmlns:content="http://purl.org/rss/1.0/modules/content/"
		xmlns:dc="http://purl.org/dc/elements/1.1/" 
		xmlns:doc="http://xsltsl.org/xsl/documentation/1.0"
		xmlns:feed="http://purl.org/atom/ns#"
		xmlns:foaf="http://xmlns.com/foaf/0.1/"
		xmlns:markup='http://xsltsl.org/markup'
		xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" 
                xmlns:xml="http://www.w3.org/XML/1998/namespace"
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
		exclude-result-prefixes="markup doc"
		version = "1.0" >
	<xsl:import href = "../xsltsl-1.2.1/markup.xsl" />

	<!-- ====================================================================== 

     ====================================================================== -->

	<doc:reference>
		<referenceinfo>

			<title>rss.xsl</title>

			<abstract>
				<para>
					This is an <acronym>XSLT</acronym> stylesheet that defines
					shared templates used by the
					<filename>atom03-to-rss10.xsl</filename> and
					<filename>atom03-to-rss20</filename> stylesheets.
				</para>
			</abstract>

			<author>
				<surname>Cope</surname>
				<firstname>Aaron</firstname>
				<othername>Straup</othername>
			</author>

			<legalnotice>
				<para>
					This work is licensed under the Creative Commons
					Attribution-ShareAlike License. To view a copy of this
					license, visit http://creativecommons.org/licenses/by-sa/1.0/
					or send a letter to Creative Commons, 559 Nathan Abbott Way,
					Stanford, California 94305, USA.
				</para>
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
				<para>
					This is a shared library and not meant to used as a
					stand-alone stylesheet.
				</para>
			</section>

			<section>
				<title>To do</title>
				<itemizedlist>
					<listitem>
						<para>
							The uri_for_type template needs to be fleshed out to
							contain a more complete list of (common)
							<acronym>URI</acronym>s for MIME types.
						</para>
					</listitem>
				</itemizedlist>
			</section>

		</partintro>

	</doc:reference>

	<!-- ====================================================================== 

     note : the one and only template we import from this stylesheet 
	    (markup:cdata-section) is, in fact, busted and overridden
	    below. this is temporary until a bug-report can be filed
	    with the developer.
     ====================================================================== -->


	<xsl:template name='markup:cdata-section'>
      <xsl:param name='text'/>

      <xsl:if test="contains($text, ']]&gt;')">
       <xsl:message terminate="yes">CDATA section contains "]]&gt;"</xsl:message>
      </xsl:if>

      <xsl:text disable-output-escaping='yes'>&lt;![CDATA[</xsl:text>
      <xsl:copy-of select='$text'/>
      <xsl:text disable-output-escaping='yes'>]]&gt;</xsl:text>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:output method = "xml" />
     <xsl:output indent = "yes" />

<!-- ====================================================================== 

     this doesn't work yet - fuct if I can figure out why :-(
     ====================================================================== -->

     <xsl:key name = "key_authors" match = "feed:author" use = "feed:name" />

<!-- ====================================================================== 

     ====================================================================== -->
     
     <xsl:template name = "channel_title">
      <xsl:value-of select = "/feed:feed/feed:title" />
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "channel_link">
      <xsl:value-of select = "/feed:feed/feed:link/@href" />
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "channel_description">
      <xsl:if test = "/feed:feed/feed:tagline">
       <xsl:value-of select = "/feed:feed/feed:tagline" />
      </xsl:if>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "channel_creator">
      <dc:creator>
      <xsl:choose>

       <xsl:when test = "/feed:feed/feed:author">
        <xsl:call-template name = "munge_author">
         <xsl:with-param name = "author" select = "/feed:feed/feed:author" />
        </xsl:call-template>
       </xsl:when>

       <!-- this does not work yet -->

       <xsl:when test = "count(key('key_authors',feed:name)) = 1">
        <xsl:for-each select = "/feed:feed/feed:entry/feed:author[count(.|key('key_authors',feed:name)[1]) = 1]">
         <xsl:call-template name = "munge_author">
	  <xsl:with-param name = "author" select = "." />
	 </xsl:call-template>
	</xsl:for-each>
       </xsl:when>

       <xsl:otherwise>
        <rdf:Bag>
        <xsl:for-each select = "/feed:feed/feed:entry">
	 <rdf:li>
         <xsl:call-template name = "munge_author">
          <xsl:with-param name = "author" select = "feed:author" />
         </xsl:call-template>
	 </rdf:li>
	</xsl:for-each>
        </rdf:Bag>
       </xsl:otherwise>

      </xsl:choose>       
      </dc:creator>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "channel_lastmod">
      <dc:date-x-metadatalastmodified>
       <xsl:value-of select = "/feed:feed/feed:modified" />
      </dc:date-x-metadatalastmodified>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "channel_generator">
      <xsl:if test = "/feed:feed/feed:generator">
       <admin:generatorAgent>
        <xsl:attribute name = "rdf:resource">
         <xsl:value-of select = "/feed:feed/feed:generator/@url" />
	</xsl:attribute>
       </admin:generatorAgent>
      </xsl:if>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "channel_license">
      <xsl:if test = "/feed:feed/feed:copyright">
       <dc:rights>
        <xsl:value-of select = "/feed:feed/feed:copyright" />
       </dc:rights>
      </xsl:if>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "item_title">
      <xsl:value-of select = "feed:title" />
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "item_link">
      <xsl:value-of select = "feed:link/@href" />
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "item_description">
      <xsl:if test = "feed:summary">
       <xsl:value-of select = "feed:summary" />
      </xsl:if>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "item_creator">
      <xsl:if test = "feed:author">
       <dc:creator>
        <xsl:call-template name = "munge_author">
         <xsl:with-param name = "author" select = "feed:author" />
        </xsl:call-template>
       </dc:creator>
      </xsl:if>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "item_contributors">
      <xsl:variable name = "count" select = "count(feed:contributor)" />
      <xsl:choose>

       <xsl:when test = "$count > 1">
        <dc:contributor>
         <rdf:Bag>
          <xsl:for-each select = "feed:contributor">
	   <rdf:li>
            <xsl:call-template name = "munge_author">
             <xsl:with-param name = "author" select = "." />
            </xsl:call-template>
	   </rdf:li>
          </xsl:for-each>
	 </rdf:Bag>
        </dc:contributor>
       </xsl:when>

       <xsl:when test = "$count = 1">
        <dc:contributor>
         <xsl:call-template name = "munge_author">
          <xsl:with-param name = "author" select = "." />
         </xsl:call-template>
        </dc:contributor>
       </xsl:when>

       <xsl:otherwise />

      </xsl:choose>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "item_created">
      <xsl:if test = "feed:created">
       <dc:created>
        <xsl:value-of select = "feed:created" />
       </dc:created>
      </xsl:if>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "item_lastmodified">
      <dc:date-x-metadatalastmodified>
       <xsl:value-of select = "feed:modified" />
      </dc:date-x-metadatalastmodified>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "item_content">
      <xsl:variable name = "count" select = "count(feed:content)" />

      <xsl:if test = "$count > 0">
       <content:items>
        <rdf:Bag>
	 <xsl:for-each select = "feed:content">

	 <xsl:choose>	

	  <xsl:when test = "@type = 'multipart/alternative' and $count > 1">
     	   <rdf:li>
           <rdf:Bag>
	    <rdf:li>		    
	     <xsl:for-each select = "feed:content">
	      <xsl:call-template name = "content_item" />
	     </xsl:for-each>
	    </rdf:li>
	   </rdf:Bag>
	   </rdf:li>
	  </xsl:when>

	  <xsl:when test = "@type = 'multipart/alternative'">
	   <xsl:for-each select = "feed:content">
	    <rdf:li>	   
	     <xsl:call-template name = "content_item" />
	    </rdf:li>
	   </xsl:for-each>
	  </xsl:when>

	  <xsl:otherwise> 
 	   <rdf:li>
            <xsl:call-template name = "content_item" />
	   </rdf:li>
	  </xsl:otherwise>

	 </xsl:choose>

	 </xsl:for-each>
	</rdf:Bag>
       </content:items>
      </xsl:if>

     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "content_item">
	  <content:item>
	   <xsl:if test = "@xml:base">
 	    <xsl:attribute name = "xml:base">
	     <xsl:value-of select = "@xml:base" />
	    </xsl:attribute>
	   </xsl:if>
	   <content:format>
	    <xsl:attribute name = "rdf:resource">
	     <xsl:call-template name = "uri_for_type">
	      <xsl:with-param name = "type" select = "@type" />
	     </xsl:call-template>
	    </xsl:attribute>
	   </content:format>

	   <xsl:variable name = "url_encoding">
	    <xsl:call-template name = "uri_for_mode">
	     <xsl:with-param name = "mode" select = "@mode" />
	    </xsl:call-template>
	   </xsl:variable>

	   <xsl:if test = "$url_encoding">
 	    <content:encoding>
	     <xsl:attribute name = "rdf:resource">
	      <xsl:value-of select = "$url_encoding" />
	     </xsl:attribute>
	    </content:encoding>
	   </xsl:if>

	   <xsl:if test = "@xml:lang">
 	    <dc:language>
	     <xsl:value-of select = "@xml:lang" />
	    </dc:language>
	   </xsl:if>
	   <rdf:value>
	    <xsl:call-template name = "markup:cdata-section">
	     <xsl:with-param name = "text" select = "child::node()" />
	    </xsl:call-template>
            <!--<xsl:copy-of select = "child::node()" />-->
	   </rdf:value>
	  </content:item>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "uri_for_type">
      <xsl:param name = "type" />
      <xsl:choose>
       <xsl:when test = "$type = 'application/xhtml+xml'">
        <xsl:text>http://www.w3.org/1999/xhtml</xsl:text>
       </xsl:when>
       <xsl:otherwise>
        <xsl:text>http://example.com/content:format#</xsl:text>
	<xsl:value-of select = "$type" />
       </xsl:otherwise>
      </xsl:choose>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "uri_for_mode">
      <xsl:param name = "mode" />
      <xsl:text>http://example.com/content:encoding#unknown</xsl:text>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "munge_author">
      <xsl:param name = "author" />

      <foaf:Person>
       <foaf:name>      
        <xsl:value-of select = "$author/feed:name" />
       </foaf:name>

      <xsl:if test = "$author/feed:url">
       <foaf:page>
       <xsl:if test = "$author/feed:url/@xml:base">
        <xsl:value-of select = "$author/feed:url/@xml:base" />
       </xsl:if>
       <xsl:value-of select = "$author/feed:url" />
       </foaf:page>
      </xsl:if>

      <xsl:if test = "$author/feed:email">
       <foaf:mbox><xsl:value-of select = "$author/feed:email" /></foaf:mbox>
      </xsl:if>    

     </foaf:Person>

  </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

</xsl:stylesheet>

<!-- ====================================================================== 
     FIN // $Id: rss.xsl,v 1.6 2004/01/25 21:27:07 asc Exp $
     ====================================================================== -->
