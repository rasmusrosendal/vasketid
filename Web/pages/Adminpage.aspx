<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="Adminpage.aspx.cs" Inherits="Web.Adminpage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="SiteMasterContentPlaceHolder1" runat="server">
    <h2>Adminpage</h2>

    <asp:Button runat="server" ID="btnReturnToMainpage" Text="Tilbage" OnClick="btnReturnToMainpage_Click" />
    <asp:Button runat="server" ID="btnLogout" Text="Log ud" OnClick="btnLogout_Click" />

    <p>Opret ny bruger</p>
    <table>
        <tr>
            <td>Brugernavn</td>
            <td>
                <asp:TextBox ID="txbUsername" Text="" runat="server" />
            </td>
        </tr>
        <tr>
            <td>Adgangskode</td>
            <td>
                <asp:TextBox ID="txbPassword" Text="" runat="server" />
            </td>
        </tr>
        <tr>
            <td>Admin</td>
            <td><asp:CheckBox runat="server" ID="chkIsAdmin" /></td>
        </tr>
        <tr>
            <td>
                <asp:Button runat="server" ID="btnCreateUser" OnClick="btnCreateUser_Click" Width="100" Text="Opret bruger" />
            </td>
        </tr>
    </table>
    <p id="ErrorMessageCreateUser" runat="server"></p>

    <p>Nuværende brugere</p>
    <table>
        <tr>
            <td>Vælg bruger</td>
            <td>
                <asp:DropDownList runat="server" ID="drpUsers" OnSelectedIndexChanged="drpUsers_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>Username</td>
            <td>
                <asp:TextBox ID="txtCurrentUserName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Admin</td>
            <td>
                <asp:CheckBox id="chkCurrentUserIsAdmin" runat="server"/>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button runat="server" ID="btnUpdateCurrentUser" OnClick="btnUpdateCurrentUser_Click" Width="100" Text="Opdater" />
            </td>
            <td>
                <asp:Button runat="server" ID="btnDeleteCurrentUser" OnClick="btnDeleteCurrentUser_Click" Width="100" Text="Slet bruger" />
            </td>
        </tr>
    </table>
    <p id="ErrorMessageUpdateUser" runat="server"></p>
</asp:Content>
