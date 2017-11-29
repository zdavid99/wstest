<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="contact-list-report.aspx.cs" Inherits="com.ocwebservice.admin.contact_list_report" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="wf_Status" runat="server" ForeColor="Red" Visible="False"></asp:Label>
        <asp:GridView ID="gvResults" runat="server">
            <HeaderStyle BackColor="Gray" />
        </asp:GridView>
    </div>
    </form>
</body>
</html>
