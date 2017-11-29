<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="/ContactForms">
      <xsl:variable name="solaceItems" select="SolaceSweepstakesForm" />
      
      <xsl:for-each select="$solaceItems">
        <xsl:element name="user">
          <xsl:element name="basefields">
            <xsl:element name="field">
              <xsl:attribute name="name">FirstName</xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="FirstName"/>
              </xsl:attribute>
            </xsl:element>
            <xsl:element name="field">
              <xsl:attribute name="name">LastName</xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="LastName"/>
              </xsl:attribute>
            </xsl:element>
            <xsl:element name="field">
              <xsl:attribute name="name">Email</xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="Email"/>
              </xsl:attribute>
            </xsl:element>
            <xsl:element name="field">
              <xsl:attribute name="name">DayPhone</xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="DayPhone"/>
              </xsl:attribute>
            </xsl:element>
            <xsl:element name="field">
              <xsl:attribute name="name">EveningPhone</xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="EveningPhone"/>
              </xsl:attribute>
            </xsl:element>
            <xsl:element name="field">
              <xsl:attribute name="name">Interest_Roofing</xsl:attribute>
              <xsl:attribute name="value">false</xsl:attribute>
            </xsl:element>           
          </xsl:element>
          
          <xsl:element name="addlists">
            <xsl:element name="list">
              <xsl:attribute name="id">2000</xsl:attribute>
              <xsl:attribute name="name">First List</xsl:attribute>
            </xsl:element>
            <xsl:element name="list">
              <xsl:attribute name="id">2001</xsl:attribute>
              <xsl:attribute name="name">Second List</xsl:attribute>
            </xsl:element>
            <xsl:element name="list">
              <xsl:attribute name="id">2002</xsl:attribute>
              <xsl:attribute name="name">Third List</xsl:attribute>
            </xsl:element>
          </xsl:element>

          <xsl:element name="addextension">
            <xsl:attribute name="name">Address</xsl:attribute>
            <xsl:element name="field">
              <xsl:attribute name="name">Address1</xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="Address1"/>
              </xsl:attribute>
            </xsl:element>
            <xsl:element name="field">
              <xsl:attribute name="name">Address2</xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="Address2"/>
              </xsl:attribute>
            </xsl:element>
            <xsl:element name="field">
              <xsl:attribute name="name">City</xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="City"/>
              </xsl:attribute>
            </xsl:element>
            <xsl:element name="field">
              <xsl:attribute name="name">State</xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="State/Abbreviation"/>
              </xsl:attribute>
            </xsl:element>
            <xsl:element name="field">
              <xsl:attribute name="name">Zip</xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="Zip"/>
              </xsl:attribute>
            </xsl:element>
          </xsl:element>
        </xsl:element>
      </xsl:for-each>
    </xsl:template>
</xsl:stylesheet>
