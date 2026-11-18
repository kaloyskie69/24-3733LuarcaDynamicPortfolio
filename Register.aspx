<%@ Page Title="Sign Up" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="PersonalWebsiteProject.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-wrapper">
        <div class="card auth-card p-4">
            <div class="card-body">
                <h3 class="mb-3 text-center"><i class="bi bi-person-plus"></i> Create an Account</h3>
                <p class="text-muted text-center mb-4">You don't have an account yet. Sign up to continue.</p>

                <asp:Panel ID="pnlMessage" runat="server" CssClass="alert" Visible="false" />

                <div class="mb-3">
                    <label class="form-label">Full Name</label>
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Juan Dela Cruz" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFullName"
                        CssClass="text-danger small" Display="Dynamic" ErrorMessage="Full name is required." ValidationGroup="Signup" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Username</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Choose a username" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUsername"
                        CssClass="text-danger small" Display="Dynamic" ErrorMessage="Username is required." ValidationGroup="Signup" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="you@example.com" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
                        CssClass="text-danger small" Display="Dynamic" ErrorMessage="Email is required." ValidationGroup="Signup" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail"
                        CssClass="text-danger small" Display="Dynamic" ErrorMessage="Enter a valid email address."
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" ValidationGroup="Signup" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="At least 8 characters" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword"
                        CssClass="text-danger small" Display="Dynamic" ErrorMessage="Password is required." ValidationGroup="Signup" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPassword"
                        CssClass="text-danger small" Display="Dynamic" ErrorMessage="Password must be at least 8 characters."
                        ValidationExpression=".{8,}" ValidationGroup="Signup" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Confirm Password</label>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Re-enter password" />
                    <asp:CompareValidator runat="server" ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword"
                        CssClass="text-danger small" Display="Dynamic" ErrorMessage="Passwords do not match." ValidationGroup="Signup" />
                </div>

                <div class="d-grid mb-3">
                    <asp:Button ID="btnSignup" runat="server" CssClass="btn btn-dark" Text="Sign Up"
                        OnClick="btnSignup_Click" ValidationGroup="Signup" />
                </div>

                <p class="text-center mb-0">
                    Already have an account? <a href="Login.aspx">Sign in</a>
                </p>
            </div>
        </div>
    </div>
</asp:Content>
