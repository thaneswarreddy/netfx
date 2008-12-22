<?xml version="1.0" encoding='iso-8859-1'?>
<xsl:stylesheet 
		xmlns:doc="http://xsltsl.org/xsl/documentation/1.0"
		xmlns:dt="http://xsltsl.org/date-time"
		xmlns:feed="http://purl.org/atom/ns#"
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  
		exclude-result-prefixes="dt doc feed"
		version = "1.0" >
	<xsl:import href = "rss.xsl" />

	<!-- ====================================================================== 

     ====================================================================== -->

     <doc:reference>
      <referenceinfo>

       <title>rss20.xsl</title>

       <abstract>
        <para>This is an <acronym>XSLT</acronym> stylesheet that defines
        templates used by the <filename>atom03-to-rss20.xsl</filename>.</para>
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
	<para>This is a shared library and not meant to used as a
       stand-alone stylesheet.</para>
       </section>
      </partintro>

     </doc:reference>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:include href = "../xsltsl-1.2.1/date-time.xsl" />

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "channel_creator">
      <xsl:if test = "/feed:feed/feed:author/feed:email">
       <managingEditor>
        <xsl:value-of select = "/feed:feed/feed:author/feed:email" />
       </managingEditor>
      </xsl:if>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "channel_lastbuilddate">
      <lastBuildDate>
       <xsl:call-template name = "w3cdtf_to_rfc822">
        <xsl:with-param name = "w3cdtf" select = "/feed:feed/feed:modified" />
       </xsl:call-template>
      </lastBuildDate>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "channel_generator">
      <xsl:if test = "/feed:feed/feed:generator">
       <generator>
        <xsl:value-of select = "/feed:feed/feed:generator" />
	<xsl:text> </xsl:text>
         <xsl:value-of select = "/feed:feed/feed:generator/@version" />
       </generator>
      </xsl:if>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "channel_license">
      <xsl:if test = "/feed:feed/feed:copyright">
       <copyright>
        <xsl:value-of select = "/feed:feed/feed:copyright" />
       </copyright>
      </xsl:if>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "item_creator">
      <xsl:if test = "feed:author/feed:email">
       <author>
        <xsl:value-of select = "feed:author/feed:email" />
	<xsl:text> </xsl:text>
	<xsl:text>(</xsl:text>
        <xsl:value-of select = "feed:author/feed:name" />
	<xsl:text>)</xsl:text>
       </author>
      </xsl:if>
     </xsl:template>

<!-- ====================================================================== 

     ====================================================================== -->

     <xsl:template name = "w3cdtf_to_rfc822">
      <xsl:param name = "w3cdtf" />

      <!--          1         2     -->
      <!-- 1234 67 90 23456 890     -->
      <!-- 2003-12-13T18:30:02Z     -->
      <!-- 1234 67 90 23456 8901234 -->
      <!-- 2003-12-13T18:30:02-0400 -->

      <!-- declared as variables because you just know
	   this one is going to generate bug reports
	   and it makes it easier to debug... -->

      <xsl:variable name = "year"     select = "substring($w3cdtf,1,4)" />
      <xsl:variable name = "month"    select = "substring($w3cdtf,6,2)" />
      <xsl:variable name = "day"      select = "substring($w3cdtf,9,2)" />
      <xsl:variable name = "hour"     select = "substring($w3cdtf,12,2)" />
      <xsl:variable name = "minute"   select = "substring($w3cdtf,15,2)" />
      <xsl:variable name = "second"   select = "substring($w3cdtf,18,2)" />
      <xsl:variable name = "timezone">
       <xsl:choose>
        <xsl:when test = "substring($w3cdtf,21,1) != ''">
	 <xsl:call-template name = "tz_offset_to_string">
	  <xsl:with-param name = "offset" select = "substring($w3cdtf,20,6)" />
	 </xsl:call-template>
        </xsl:when>
	<xsl:otherwise>
         <xsl:value-of select = "substring($w3cdtf,20,1)" />
	</xsl:otherwise>
       </xsl:choose>
      </xsl:variable>
      <xsl:variable name = "rfc822"   select = "'%a, %d %b %Y %H:%M:%S %z'" />

      <!--
      <xsl:text>year: '</xsl:text>
      <xsl:value-of select = "$year" />
      <xsl:text>' month: '</xsl:text>
      <xsl:value-of select = "$month" />
      <xsl:text>' day: '</xsl:text>
      <xsl:value-of select = "$day" />
      <xsl:text>' hour: '</xsl:text>
      <xsl:value-of select = "$hour" />
      <xsl:text>' minute: '</xsl:text>
      <xsl:value-of select = "$minute" />
      <xsl:text>' second: '</xsl:text>
      <xsl:value-of select = "$second" />
      <xsl:text>' timezone: '</xsl:text>
      <xsl:value-of select = "$timezone" />
      <xsl:text>' format: '</xsl:text>
      <xsl:value-of select = "$rfc822" />
      <xsl:text>'</xsl:text>
      -->

      <xsl:call-template name="dt:format-date-time">
       <xsl:with-param name="year"      select = "$year" />
       <xsl:with-param name="month"     select = "$month" />
       <xsl:with-param name="hour"      select = "$hour" />
       <xsl:with-param name="day"       select = "$day" />
       <xsl:with-param name="minute"    select = "$minute" />
       <xsl:with-param name="second"    select = "$second" />
       <xsl:with-param name="time-zone" select = "$timezone" />
       <xsl:with-param name="format"    select = "$rfc822" />
      </xsl:call-template>

     </xsl:template>

     <xsl:template name = "tz_offset_to_string">
      <xsl:param name = "offset" />
      <xsl:choose>
       <xsl:when test = "$offset = '+01:00'">
        <xsl:text>A</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+02:00'">
        <xsl:text>B</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+03:00'">
        <xsl:text>C</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+03:30'">
        <xsl:text>C*</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+04:00'">
        <xsl:text>D</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+04:30'">
        <xsl:text>D*</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+05:00'">
        <xsl:text>E</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+05:30'">
        <xsl:text>E*</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+06:00'">
        <xsl:text>F</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+06:30'">
        <xsl:text>F*</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+07:00'">
        <xsl:text>G</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+08:00'">
        <xsl:text>H</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+09:00'">
        <xsl:text>I</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+10:00'">
        <xsl:text>K</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+10:30'">
        <xsl:text>K*</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+11:00'">
        <xsl:text>L</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+11:30'">
        <xsl:text>L*</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+12:00'">
        <xsl:text>M</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+13:00'">
        <xsl:text>M*</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '+14:00'">
        <xsl:text>M+</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-01:00'">
        <xsl:text>N</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-02:00'">
        <xsl:text>O</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-03:00'">
        <xsl:text>P</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-03:30'">
        <xsl:text>P*</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-04:00'">
        <xsl:text>Q</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-05:00'">
        <xsl:text>R</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-06:00'">
        <xsl:text>S</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-07:00'">
        <xsl:text>T</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-08:00'">
        <xsl:text>U</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-08:30'">
        <xsl:text>U*</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-09:00'">
        <xsl:text>V</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-09:30'">
        <xsl:text>V*</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-10:00'">
        <xsl:text>W</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-11:00'">
        <xsl:text>X</xsl:text>
       </xsl:when>
       <xsl:when test = "$offset = '-12:00'">
        <xsl:text>Y</xsl:text>
       </xsl:when>
       <xsl:otherwise>
        <xsl:text>Z</xsl:text>
       </xsl:otherwise>
      </xsl:choose>
     </xsl:template>

</xsl:stylesheet>

<!-- ====================================================================== 
     FIN // $Id: rss20.xsl,v 1.6 2004/01/25 20:47:32 asc Exp $
     ====================================================================== -->
