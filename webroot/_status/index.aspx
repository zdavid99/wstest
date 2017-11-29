<%@ Page Language="C#" AutoEventWireup="true" Inherits="StatusView" Codebehind="index.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
	<title>webservices.owencorning.com/ContactList/ - Status Page</title>
    <link href="style.css" rel="STYLESHEET" type="text/css" />
</head>
<body>
	<form runat="server">
    <center>
 		<br/>
		<table cellspacing="4" cellpadding="4"> 
			<tr>
				<td valign="top" nowrap><i><a href="#info">Info</a></i></td>
				<td valign="top" nowrap><i><a href="#appsettings">Application</a></i></td>
				<td valign="top" nowrap><i><a href="#customerror">Custom Errors</a></i></td>
				<td valign="top" nowrap><i><a href="#database">Database</a></i></td>
				<%--<td valign="top" nowrap><i><a href="#mail">Email</a></i></td>--%>
				<td valign="top" nowrap><i><a href="#status">Status</a></i></td>
			</tr>
		</table>

		<br/><hr width="800" />
		<br/>

		<h2><a name="info">Info</a></h2>
		<table style="text-align:left;"> 
			<tr class="outnormal">
				<td valign="top" nowrap="nowrap" colspan="2">&nbsp;</td>
			</tr>
			<tr>
				<td valign="top" nowrap="nowrap"><b>System Time:</b></td>
				<td valign="top" nowrap="nowrap">[<asp:Label ID="LDateTime" runat="server" />]</td>
			</tr>
			<tr>
				<td valign="top" nowrap="nowrap"><b>Machine Name:</b></td>
				<td valign="top" nowrap="nowrap">[<asp:Label ID="LMachineName" runat="server" />]</td>
			</tr>
			<tr>
				<td valign="top" nowrap="nowrap"><b>ASP.NET version:</b></td>
				<td valign="top" nowrap="nowrap">[<asp:Label ID="LNetVersion" runat="server" />]</td>
			</tr>
			<tr>
				<td valign="top" nowrap="nowrap"><b>Server Up Time:</b></td>
				<td valign="top" nowrap="nowrap">[<asp:Label ID="LUpTime" runat="server" />]</td>
			</tr>
			<tr>
				<td valign="top" nowrap="nowrap"><b>OS:</b></td>
				<td valign="top" nowrap="nowrap">[<asp:Label ID="LOSVersion" runat="server" />]</td>
			</tr>
		   <tr class="outnormal">
				<td valign="top" nowrap="nowrap" colspan="2"><b><asp:Label ID="LEnvironmentStatus" runat="server" /></b></td>
			</tr>
		</table>

		<br/><hr width="800" />
		<br/>

		<h2><a name="appsettings">Application Settings</a></h2>
		<table style="text-align:left;"> 
			<tr class="outnormal">
				<td valign="top" nowrap="nowrap" colspan="2">&nbsp;</td>
			</tr>
<%--			 <tr>
				<td valign="top" nowrap="nowrap"><b>Session Timeout:</b></td>
				<td valign="top" nowrap="nowrap">[<asp:Label ID="LSessionTimeout" runat="server" /> minutes]</td>
			</tr>--%>
			<%--<tr>
                <td valign="top" nowrap="nowrap"><b>System Mail Debug:</b></td>
                <td valign="top" nowrap="nowrap">[<asp:Label ID="LMailDebug" runat="server" />]</td>
            </tr>--%>
            <tr>
                <td valign="top" nowrap="nowrap"><b>connection string:</b></td>
                <td valign="top" nowrap="nowrap">[<asp:Label ID="lConnectionStringCheck" runat="server" />]</td>
            </tr>
			<tr class="outnormal">
				<td valign="top" nowrap="nowrap" colspan="2"><b><asp:Label ID="LAppConfigurationStatus" runat="server" /></b></td>
			</tr>
		</table>

		<br/><hr width="800" />
		<br/>

		<h2><a name="customerror">Custom Error Settings</a></h2>
		<h3>(web.config)</h3>
		<table style="text-align:left;"> 
			<tr class="outnormal">
				<td valign="top" nowrap="nowrap" colspan="2">&nbsp;</td>
			</tr>
			 <tr>
				<td valign="top" nowrap="nowrap"><b>Default Redirect:</b></td>
				<td valign="top" nowrap="nowrap">[<asp:HyperLink ID="hDefaultRedirect" runat="server" />]</td>
			</tr>
			<tr>
				<td valign="top" nowrap="nowrap"><b>Custom Error Mode:</b></td>
				<td valign="top" nowrap="nowrap">[<asp:Label ID="LCustomErrorsMode" runat="server" />]</td>
			</tr>
			<asp:Repeater ID="RCustomErrorCodes" runat="server">
				<HeaderTemplate>
					 <tr>
						<td valign="top" colspan="2"><hr /></td>
					</tr>
					<tr>
						<td valign="top" colspan="2">
							<table>
								<tr>
									<td valign="top" nowrap="nowrap"><b>Code</b></td>
									<td valign="top" nowrap="nowrap"><b>Redirect</b></td>
								</tr>
				</HeaderTemplate>
				<ItemTemplate>
					<tr>
						<td valign="top" nowrap="nowrap">[<%# Eval("StatusCode")%>]</td>
						<td valign="top" nowrap="nowrap">[<%# Eval("Redirect")%>]</td>
					</tr>
				</ItemTemplate>
				<FooterTemplate>
							</table>
						</td>
					</tr>
				</FooterTemplate>
			</asp:Repeater>
			<tr class="outnormal">
				<td valign="top" colspan="2"><b><asp:Label ID="LCustomErrorStatus" runat="server" /></b></td>
			</tr>
		</table>

		<br/><hr width="800" />
		<br/>

		<h2><a name="database">Database</a></h2>
		<table style="text-align:left;" border="1" cellpadding="1" cellspacing="0"> 
			<tr class="outnormal">
				<td valign="top" nowrap="nowrap"><b>DAO</b></td>
				<td valign="top" nowrap="nowrap"><b>Record Count</b></td>
				<td valign="top" nowrap="nowrap"><b>Pass/Fail</b></td>
			</tr>
			<asp:Repeater ID="RDAO" runat="server">
				<ItemTemplate>
					<tr>
						<td valign="top" nowrap="nowrap"><%# Eval("Name") %>&nbsp;</td>
						<td valign="top" nowrap="nowrap"><%# Eval("RecordCountTest") %></td>
						<td valign="top" nowrap="nowrap"><asp:Label ID="TDaoPassFail" runat="server" /></td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
			<tr class="outnormal">
				<td valign="top" nowrap="nowrap" colspan="3"><b><asp:Label ID="LDatabaseStatus" runat="server" /></b></td>
			</tr>
		</table>            

		<br/><hr width="800" />
		<br/>


		<%--<a name="mail"></a>
        <h2>Email</h2>
        <table style="text-align:left;"> 
            <tr class="outnormal">
                <td valign="top" nowrap="nowrap" colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td valign="top" nowrap="nowrap"><b>Send Email:</b></td>
                <td valign="top" nowrap="nowrap">[<asp:Label ID="LEmailSent" runat="server" />]</td>
            </tr>
            <tr class="outnormal">
                <td valign="top" nowrap="nowrap" colspan="2"><b><asp:Label ID="LSendEmailStatus" runat="server" /></b></td>
            </tr>
        </table>

        <br/><hr width="800" />
        <br/>--%>

		<a name="status"></a>
		<asp:Label ID="LErrorsOnPage" runat="server" ><h2>Errors on Page!</h2></asp:Label>
		<asp:Label ID="LStatusOK" runat="server" ><h2>Status is OK</h2></asp:Label>
		<asp:Label ID="LWarningsOnPage" runat="server" ><h2>Warnings on Page!</h2></asp:Label>
		<asp:Label ID="LWarningStatusOK" runat="server" ><h2>No Warnings Found</h2></asp:Label>	
		<br />
    </center>
	</form>
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
	<br />
</body>
</html>
