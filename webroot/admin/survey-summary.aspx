<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" Inherits="survey_summary" Title="Owens Corning Web Services Admin" Codebehind="survey-summary.aspx.cs" %>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentMain" Runat="Server">
 <table>
        <tr>
            <td colspan="3">
                <h2>Survey Summary</h2>
                <hr />
            </td>
        </tr>
        <tr>
            <td style="width: 150px">
                Survey:</td>
            <td style="width: 300px">
                <asp:DropDownList ID="wf_FormTypes" runat="server">
                </asp:DropDownList></td>
            <td>
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
                <asp:Button ID="wf_DownloadExcel" runat="server" OnClick="wf_DownloadExcel_Click" Text="Download Excel File" Visible="false" /></td>
            <td>
            </td>
        </tr>
    </table>
    <hr />
    <br />
    <asp:Label ID="wf_PreviewDesc" runat="server" Text=""></asp:Label>

    <asp:Repeater ID="wf_Surveys" runat="server">    
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <div>
                <asp:LinkButton ID="wf_SurveyNameLink" runat="server" OnClick="wf_ViewData_Click" Text="<%# Container.DataItem %>"></asp:LinkButton> 
            </div>
            <asp:Repeater ID="wf_Questions" runat="server">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <div><%# Container.DataItem %></div>
                    <div>
                        <asp:GridView ID="wf_Responses" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#000000" GridLines="None" >
                            <Columns>
                                <asp:BoundField DataField="Answer" HeaderText="Answer" >
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Total" HeaderText="Total" >
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </ItemTemplate>
    </asp:Repeater>
    
    <asp:GridView ID="wf_SurveyDetails" runat="server" AutoGenerateColumns="true" CellPadding="4" ForeColor="#000000" GridLines="None">
    </asp:GridView>
</asp:Content>

