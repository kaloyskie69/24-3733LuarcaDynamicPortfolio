<%@ Page Title="Sign In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="PersonalWebsiteProject.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-wrapper">
        <div class="card auth-card p-4">
            <div class="card-body">
                <h3 class="mb-3 text-center"><i class="bi bi-shield-lock"></i> Sign In</h3>
                <p class="text-muted text-center mb-4">You need to sign in before you can view this website.</p>

                <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false" />

                <div class="mb-3">
                    <label class="form-label">Username or Email</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter username or email" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUsername"
                        CssClass="text-danger small" Display="Dynamic" ErrorMessage="Username or email is required." ValidationGroup="Login" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter password" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword"
                        CssClass="text-danger small" Display="Dynamic" ErrorMessage="Password is required." ValidationGroup="Login" />
                </div>

                <div class="d-grid mb-3">
                    <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-dark" Text="Sign In"
                        OnClick="btnLogin_Click" ValidationGroup="Login" />
                </div>

                <p class="text-center mb-0">
                    Don't have an account? <a href="Register.aspx">Sign up here</a>
                </p>
            </div>
        </div>
    </div>
</asp:Content>
