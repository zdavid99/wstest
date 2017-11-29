<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" Inherits="admin_login" Title="Owens Corning | Admin | Login" Codebehind="login.aspx.cs" %>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentMain" Runat="server">
    
	<div id="admin-wrapper">
	
		<p style="color:#ff0000;"><asp:Label ID="wf_Status" runat="server" Visible="false" Text="*"></asp:Label></p>
		<p>Username: <asp:textbox id="wf_Username" cssclass="text" runat="server" style="width:7em;" /></p>
		<p>Password: &nbsp;<asp:textbox id="wf_Password" textmode="Password" cssclass="text" runat="server" style="width:7em;" /></p>

		<asp:button id="login_button" onclick="Login_Click" text="  Login  " cssclass="button" runat="server"/>	
	</div>

</asp:Content>

