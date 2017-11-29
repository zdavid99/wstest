<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="template.aspx.cs" Inherits="NotificationServiceAdmin.email.template" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <style type=text/css>
    BODY {
        MARGIN-TOP: 10px; MARGIN-LEFT: 10px; WIDTH: 100%; HEIGHT: 100%; line-height: 12pt;
    }
    P {
        FONT-SIZE: 10pt; FONT-FAMILY: Arial, Helvetica, sans-serif
    }
    TD {
        FONT-SIZE: 9pt; FONT-FAMILY: Arial, Helvetica, sans-serif
    }
    a:link {
        COLOR: #ff0099; FONT-FAMILY: Arial, Helvetica, sans-serif; TEXT-DECORATION: underline
    }
    a:hover {
        COLOR: #666666; FONT-FAMILY: Arial, Helvetica, sans-serif; TEXT-DECORATION: none
    }
    .normal_small {
        FONT-SIZE: 8pt; FONT-FAMILY: Arial, Helvetica, sans-serif
    }
    em{ color: #666666; font-style: normal }
    table strong{ font-style: italic; font-weight: normal; font-size: 8.5pt; color:#666666 }
    </style>
    <form id="form1" runat="server">
        <table width="580" border="0" cellspacing="0" cellpadding="0" align="center">
            <tr>
                <td><img src="http://commercial.owenscorning.com/images/email/doc-update-header.jpg" width="600" height="126" /></td>
            </tr>
            <tr>
                <td style="background: #f8f8f8; padding: 10px">
                    <h2 style="FONT-SIZE: 20pt;COLOR:#333333;LINE-HEIGHT:26pt;FONT-FAMILY:Arial">Updated Literature</h2>
                    <p>Below is a list of documents that have either been added or updated from <asp:Label ID="lblLastBatch" runat="server" /> through <asp:Label ID="lblBatchRun" runat="server" />. The date of the last update is listed below each file name.</p>
                    <table width="100%" border="0" cellspacing="0" cellpadding="3" align="center">
                        <asp:Repeater ID="rptDocumentTypes" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td colspan="2"><h3 style="FONT-SIZE: 14pt;COLOR:#333333;LINE-HEIGHT:26pt;FONT-FAMILY:Arial;padding:0;margin:0"><%# Eval("documentType") %></h3></td>
                                </tr>
                                <asp:Repeater ID="rptDocuments" runat="server" DataSource="">
                                    <tr>
                                        <td width="60" align="center"><img src="http://commercial.owenscorning.com/images/email/icon-med-pdf.gif" alt="PDF Icon" width="28" height="29" /></td>
                                        <td>
                                            <a href='<%# Eval("url") %>'>
                                                <%# Eval("documentName") %>
                                            </a>
                                            <em>(<%# Eval("fileSize") %>, PDF)</em><br />
                                            <strong>Updated <%# Eval("dateUpdated") %></strong>
                                        </td>
                                    </tr>
                                </asp:Repeater>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="normal_small" style="font-size:8pt;FONT-FAMILY: Arial, Helvetica, sans-serif;border-top: 2px solid #CCCCCC;padding: 10px"><p class="normal_small">&copy; 1996&ndash;2009 Owens Corning Commercial Insulation. All rights reserved.<br />
                    This email was sent by: Owens Corning<br />
                    Owens Corning Parkway Toledo, OH, 43659, USA<br />
                    To unsubscribe from this email, please send an email to <a href="mailto:commercialupdate@owencorning.com">commercialupdate@owencorning.com</a></p>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
