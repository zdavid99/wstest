<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/WebConrols/SurveyForm.ascx.cs" Inherits="OwensCorning.Survey.WebControls.SurveyForm" %>
<style type="text/css">
    .hidden
    {
    	display: none;
    }
</style>
<script language="javascript" type="text/javascript">
    $(document).ready(function() { $('#tForm_p2-Products').hide('slow') });
    
    function SetDisplay(id, val) {
        if (val) {
            $('#' + id).show('fast');
        }
        else {
            $('#' + id).hide('slow');
        }
    }
</script>

<asp:Panel ID="pForm" DefaultButton="bSubmit" runat="server">
    <asp:PlaceHolder ID="phForm" runat="server"></asp:PlaceHolder>
    <div style="clear: both;"/>
    <div class="contactusform-div submitbuttons">
	<asp:ImageButton ID="bSubmit" OnClick="bSubmit_Click" ImageUrl="/Content/images/ui/btn-submit.jpg" AlternateText="Submit" CssClass="" runat="server" CausesValidation="true" />
	<a href="#" onclick="document.forms[0].reset();"><img src="/Content/images/ui/btn-clear.jpg" /></a>
    </div>
</asp:Panel>
<asp:Panel ID="pThankYou" Visible="false" runat="server">
    <asp:Label ID="lXml" runat="server"></asp:Label>
    <% = ThankYouText %>
</asp:Panel>