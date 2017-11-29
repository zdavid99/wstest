<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="notification.aspx.cs" Inherits="NotificationServiceAdmin._Default" MasterPageFile="admin.master" %>

<%@ Import Namespace="OwensCorning.NotificationService" %>

<%@ Register assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ExtraCSS" runat="server">
<style type="text/css" media="all" >
	table.chart{ margin: 10px 0; width: 100% }
	table.chart td, table.chart th{ padding: 4px }
	table.chart th{ background-color: #FCF0F5; }
	label{ display: block; float: left; width: 150px; }
	hr{ margin: 0 0 15px 0 }
	.odd{ background-color:#F0F0F0; }
	.sort a{ background: url(/images/sort-arrows.gif) no-repeat left center; padding-left: 14px; }
</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <h2>Notification Service</h2>
	<hr />
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
		<br />
		*Note: Only the documents modified BEFORE the batch date are included in each batch.
		<br />
		<br />
		<hr />
		<br />
        <asp:UpdatePanel ID="upDetails" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                Options: <asp:LinkButton ID="lbDocuments" runat="server" onclick="lbDocuments_Click">Documents List</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
                <asp:LinkButton ID="lbSentTo" runat="server" onclick="lbSentTo_Click">Sent To</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
                <asp:LinkButton ID="lbRunNewBatch" runat="server" OnClick="lbRunNewBatch_Click">Run new batch for this site</asp:LinkButton>
                <br />
                <asp:GridView ID="gvDocuments" runat="server" AutoGenerateColumns="False" AlternatingRowStyle-CssClass="odd" GridLines="None" CssClass="chart">
                    <Columns>
                        <asp:BoundField DataField="documentName" HeaderText="Document Name" >
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                    </Columns>
                    <AlternatingRowStyle CssClass="odd" />
                </asp:GridView>
                <asp:GridView ID="gvSendTo" runat="server" AutoGenerateColumns="False" 
                    AllowSorting="True" onsorting="gvSendTo_Sorting" onrowcommand="gvSendTo_RowCommand"
                    DataKeyNames="subscriptionEmail" AlternatingRowStyle-CssClass="odd"  CssClass="chart" GridLines="None">
                    <Columns>
                        <asp:TemplateField HeaderText="First Name" SortExpression="Subscription.firstName" HeaderStyle-CssClass="sort" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# ((Subscription)Eval("Subscription")).firstName %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle CssClass="sort" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Last Name" SortExpression="Subscription.lastName" HeaderStyle-CssClass="sort" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# ((Subscription)Eval("Subscription")).lastName %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle CssClass="sort" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Email" SortExpression="subscriptionEmail" HeaderStyle-CssClass="sort">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("subscriptionEmail") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle CssClass="sort" HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Date Sent" DataField="lastSendDate" >
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="status" HeaderText="Current Status" >
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:ButtonField HeaderText="Resend Email" Text="Resend" CommandName="Resend" >
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:ButtonField>
                    </Columns>
                    <AlternatingRowStyle CssClass="odd" />
                </asp:GridView>
                <asp:Panel ID="pnlRunNewBatch" Visible="false" runat="server">
                    <asp:Panel ID="pnlNoNewBatchDocuments" visible="false" runat="server">
                        <br />There are no documents that have changed since the last batch was run, therefore a batch will not be run at this time.<br /><br />
                    </asp:Panel>
                    <asp:Panel ID="pnlNewBatchDocumentsHead" Visible="false" runat="server">
                        <br />By clicking the Submit button you will be running a new batch that will span from <asp:Literal runat="server" ID="lLastBatchRun"/> through <%= DateTime.Now.ToString("g") %>. Please un-select any documents that should NOT be included in the Notification Email ...<br /><br />
                    </asp:Panel>
                    <asp:Repeater ID="rNewBatchDocuments" runat="server">
                        <ItemTemplate>
                            <asp:CheckBox ID="cbIncludeInNewBatch" runat="server" />
                            <asp:Literal ID="lNewBatchDocumentName" runat="server" />
                            <br />
                        </ItemTemplate>
                        <FooterTemplate><br /></FooterTemplate>
                    </asp:Repeater>
                    <asp:Button ID="bRunBatch" OnClick="bRunBatchSubmit_Click" Text="Submit" Visible="false" runat="server" />
					<br /><br />
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="ddlBatch" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Content>