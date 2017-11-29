<%@ Control Language="C#" AutoEventWireup="true" Inherits="admin_AdminMenu" Codebehind="AdminMenu.ascx.cs" %>
<table border="0">
    <tr>
        <td valign="top">
<asp:Menu ID="Menu1" runat="server" BackColor="#FFFFFF" DynamicHorizontalOffset="2" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#000000" Orientation="Horizontal" StaticSubMenuIndent="10px">
    <StaticSelectedStyle BackColor="#FFFFFF" />
    <StaticMenuItemStyle HorizontalPadding="8px" VerticalPadding="10px" Font-Size="11px" />
    <DynamicHoverStyle BackColor="#FFFFFF" ForeColor="White" />
    <DynamicMenuStyle BackColor="#FFFFFF" />
    <DynamicSelectedStyle BackColor="#FFFFFF" />
    <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
    <StaticHoverStyle BackColor="#FFFFFF" ForeColor="#000000" />
    <Items>
        <asp:MenuItem NavigateUrl="~/admin/index.aspx" Text="Admin Home" Value="Admin Home"></asp:MenuItem>
        <asp:MenuItem Text="Contacts" Value="Contacts" NavigateUrl="~/admin/contact-list.aspx"></asp:MenuItem>
        <asp:MenuItem Text="Notification Service" Value="Notification Service" NavigateUrl="~/admin/notification.aspx"></asp:MenuItem>
    </Items>
    <StaticMenuStyle HorizontalPadding="2px" VerticalPadding="2px" />
</asp:Menu>
        </td>
        <td valign="middle">&nbsp;&nbsp;
            <asp:LinkButton ID="wf_Logout" runat="server" OnClick="wf_Logout_Click">Logout</asp:LinkButton>&nbsp;</td>
    </tr>
</table>
