<?xml version="1.0" encoding='iso-8859-1'?>
<xsl:stylesheet xmlns:doc="http://xsltsl.org/xsl/documentation/1.0"
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  
		exclude-result-prefixes="doc"
		version = "1.0" >

<!-- ====================================================================== 

     ====================================================================== -->

     <doc:reference>
      <referenceinfo>

       <title>rss10.xsl</title>

       <abstract>
        <para>This is an <acronym>XSLT</acronym> stylesheet that defines
        templates used by the <filename>atom03-to-rss10.xsl</filename>.</para>
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

     <xsl:include href = "rss.xsl" />

</xsl:stylesheet>

<!-- ====================================================================== 
     FIN // $Id: rss10.xsl,v 1.5 2004/01/25 20:47:32 asc Exp $
     ====================================================================== -->
