<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" Inherits="admin_index" Title="Owens Corning Web Services Admin" Codebehind="index.aspx.cs" %>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentMain" Runat="Server">
<div id="admin-wrapper">


<h2>Contact List</h2>
<p>
Contact leads created via the Owens Corning WebService are available through the "<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/admin/contact-list.aspx">Contacts</asp:HyperLink>" link (above). The XML files downloaded from the Contacts page should be opened with
    Excel.&nbsp; To open with Excel:</p>
    <ul>
        <li>Download XML file to a location on your computer (ex. Desktop)</li>
        <li>Start Excel</li>
        <li>Open File (File | Open | Desktop... and select the file with an <strong>.xml</strong>
            ending)</li>
        <li>Excel will open a dialog box named "Open XML" and ask you "how to open the file",
            Select "<strong>As an XML table</strong>"</li>
        <li>Excel will show a message that "source does not refer to a schema", this is OK,
            press "<strong>OK</strong>"</li>
    </ul>
    <p>The data you have filtered should now be loaded into Excel. You can copy and paste this data to another location or save this data as an Excel file using the  "<strong>Save
            As</strong>" option.</p>

</div>

</asp:Content>

