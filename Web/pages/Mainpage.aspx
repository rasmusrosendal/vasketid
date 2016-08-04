<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="Mainpage.aspx.cs" Inherits="Web.Welcome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="SiteMasterContentPlaceHolder1" runat="server">
    
    <h2 runat="server" id="pWelcomeUser"></h2>
    <asp:Button Visible="false" runat="server" ID="btnGoToAdminpage" OnClick="btnGoToAdminpage_Click" Text="Gå til adminside"/>
    <asp:Button runat="server" ID="btnLogout" Text="Log ud" OnClick="btnLogout_Click" />

    <p>Vælg en dato</p>
    <script>
        $(function () {
            $("#<%= datepicker.ClientID %>").datepicker({ dayNamesMin: ["Sø", "Ma", "Ti", "On", "To", "Fr", "Lø"], firstDay: 1, dateFormat: "d/m/yy" });
            });
    </script>
    <p>
        Dato:
            <input type="text" id="datepicker" runat="server"/>
        <input type="submit" value="Opdater"/>
    </p>
    
    <p runat="server" id="ShowBookingsDate"></p>
    <asp:GridView ID="grvBookings" runat="server">

    </asp:GridView>

    <p>Book en tid</p>
    <p runat="server" id="ErrorMessage" style="color:red"></p>
    Fra: <asp:DropDownList ID="ddlFrom" runat="server"/>
    Til: <asp:DropDownList ID="ddlTo" runat="server" />
    <asp:button Text="Book disse tider" runat="server" ID="btnBook" OnClick="btnBook_Click"/>
    <asp:Button Text="Slet bookinger på disse tider" runat="server" ID="btnDeleteBookings" OnClick="btnDeleteBookings_Click" />

    <p>Tilføj note til booking</p>
    <asp:TextBox runat="server" ID="txbNote" TextMode="MultiLine" Columns="50" Rows="5" />
    
</asp:Content>
