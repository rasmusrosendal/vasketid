<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Web.Index" %>

<asp:Content ID="Content2" ContentPlaceHolderID="SiteMasterContentPlaceHolder1" runat="server">
    <style>
        .customRow{
            margin:15px 0px;
        }
    </style>
            <div class="row customRow">
                <div class="col-sm-2 col-sm-offset-3">
                    Brugernavn
                </div>
                <div class="col-sm-2">
                    <asp:TextBox ID="txbUsername" Text="" runat="server" />
                </div>
            </div>
            <div class="row customRow">
                <div class="col-sm-2 col-sm-offset-3">
                    Password
                </div>
                <div class="col-sm-2">
                    <asp:TextBox ID="txbPassword" Text="" TextMode="Password" runat="server" />
                </div>
            </div>
            <div class="row customRow">
                <div class="col-sm-2 col-sm-offset-3">
                    <asp:Button runat="server" ID="btnLogin" Width="100" Text="Log ind" OnClick="btnLogin_Click" class="btn-default"/>
                </div>
                <div class="col-sm-2">
                    <asp:Button runat="server" ID="btnNewUser" Width="200" Text="Sign up, og log ind" OnClick="btnNewUser_Click" class="btn-info"/>
                </div>
            </div>
            <div class="row customRow">
                <div class="col-sm-6 col-sm-offset-3">
                    <p runat="server" id="errorMessage"></p>
                </div>
            </div>
</asp:Content>
