<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="notification.aspx.cs" Inherits="OwensCorning.NotificationServiceAdmin._Default" %>

<%--<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>--%>
<%@ Import Namespace="OwensCorning.NotificationService" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div>
        <asp:UpdatePanel ID="upSelectors" runat="server">
            <ContentTemplate>
                <label for="<% =ddlSite.ClientID %>">Site: </label>
                <asp:DropDownList ID="ddlSite" runat="server"
                    AutoPostBack="True" OnSelectedIndexChanged="ddlSite_SelectedIndexChanged">
                </asp:DropDownList>
                
                <br />
                <label for="<% =ddlBatch.ClientID %>">Batch: </label>
                <asp:DropDownList ID="ddlBatch" runat="server" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlBatch_SelectedIndexChanged">
                </asp:DropDownList>
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <asp:UpdatePanel ID="upDetails" runat="server">
            <ContentTemplate>
                <asp:LinkButton ID="lbDocuments" runat="server" onclick="lbDocuments_Click">Documents List</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
                <asp:LinkButton ID="lbSentTo" runat="server" onclick="lbSentTo_Click">Sent To</asp:LinkButton>
                <br />
                <asp:GridView ID="gvDocuments" runat="server" AutoGenerateColumns="False" AllowPaging="true" PageSize="50">
                    <Columns>
                        <asp:BoundField DataField="documentName" HeaderText="Document Name" />
                    </Columns>
                </asp:GridView>
                <asp:GridView ID="gvSendTo" runat="server" AutoGenerateColumns="False" 
                    AllowPaging="True" PageSize="50" AllowSorting="True" 
                    onsorting="gvSendTo_Sorting" onrowcommand="gvSendTo_RowCommand" DataKeyNames="subscriptionEmail">
                    <Columns>
                        <asp:TemplateField HeaderText="First Name" SortExpression="Subscription.firstName">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# ((Subscription)Eval("Subscription")).firstName %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Last Name" SortExpression="Subscription.lastName">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# ((Subscription)Eval("Subscription")).lastName %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Email" DataField="subscriptionEmail" />
                        <asp:BoundField HeaderText="Date Sent" DataField="lastSendDate" />
                        <asp:BoundField DataField="status" HeaderText="Current Status" />
                        <asp:ButtonField HeaderText="Resend Email" Text="Resend" CommandName="Resend" />
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="ddlBatch" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
