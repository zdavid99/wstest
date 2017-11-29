<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeBehind="energy-complete-contacts.aspx.cs" Inherits="com.ocwebservice.admin.energy_complete_contacts" %>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentMain" runat="server">
 <div id="admin-wrapper">
 
 
 <table>
        <tr>
            <td colspan="3">
                <h2>Contacts List</h2>
                <hr />
            </td>
        </tr>
        <tr>
            <td style="width: 150px">
                Starting From:</td>
            <td style="width: 300px">
                <asp:TextBox ID="wf_StartingFrom" runat="server"></asp:TextBox>&nbsp;</td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 150px">
                Ending At:</td>
            <td style="width: 300px">
                <asp:TextBox ID="wf_EndingOn" runat="server"></asp:TextBox>&nbsp;</td>
            <td>
            </td>
        </tr>
        
        <tr>
            <td style="width: 150px">
            </td>
            <td style="width: 300px">
                <asp:Label ID="wf_Status" runat="server" ForeColor="Red" Visible="False"></asp:Label></td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 150px">
            </td>
            <td style="width: 300px">
                <asp:Button ID="wf_ViewData" runat="server" OnClick="wf_ViewData_Click" Text="Preview" />
                &nbsp;
                <asp:Button ID="wf_DownloadXML" runat="server" OnClick="wf_DownloadXML_Click" Text="Download XML File" /></td>
            <td>
            </td>
        </tr>
    </table>
    <hr />
    <br />
    <asp:Label ID="wf_PreviewDesc" runat="server" Text="">Energy Complete Contacts (Combined)</asp:Label>

    <asp:GridView ID="wf_Contacts" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#000000" GridLines="None" >

    <Columns>
        <asp:BoundField DataField="Id" HeaderText="ID" />
        <asp:BoundField DataField="DateCreated" HeaderText="Created" >
            <HeaderStyle HorizontalAlign="Left" />
        </asp:BoundField>
        <asp:BoundField DataField="SourceFormName" HeaderText="Form Name" >
            <HeaderStyle HorizontalAlign="Left" />
        </asp:BoundField>
        <asp:BoundField DataField="Company" HeaderText="Company" NullDisplayText="N/A" >
            <HeaderStyle HorizontalAlign="Left" />
        </asp:BoundField>
        <asp:BoundField DataField="Name" HeaderText="Contact Name" >
            <HeaderStyle HorizontalAlign="Left" />
        </asp:BoundField>
        <asp:BoundField DataField="Phone" HeaderText="Phone">
            <HeaderStyle HorizontalAlign="Left" />
        </asp:BoundField>
    </Columns>
        <FooterStyle BackColor="#FCF0F5" Font-Bold="True" ForeColor="#000000" />
        <RowStyle BackColor="#f0f0f0" ForeColor="#000000" />
        <PagerStyle BackColor="#FCF0F5" ForeColor="#000000" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#000000" />
        <HeaderStyle BackColor="#FCF0F5" Font-Bold="True" ForeColor="#000000" />
        <EditRowStyle BackColor="#999999" />
        <AlternatingRowStyle BackColor="White" ForeColor="#000000" />
    </asp:GridView>  
    <br />

    </div>
</asp:Content>