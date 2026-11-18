<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="UserProfile.aspx.cs" Inherits="PersonalWebsiteProject.UserProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center">
        <div class="col-md-7">

            <div class="card mb-4">
                <div class="card-header bg-dark text-white"><i class="bi bi-person-gear"></i> My Information</div>
                <div class="card-body">

                    <asp:Panel ID="pnlProfileMessage" runat="server" CssClass="alert" Visible="false" />

                    <div class="mb-3">
                        <label class="form-label">Username</label>
                        <asp:TextBox ID="txtUsernameDisplay" runat="server" CssClass="form-control" Enabled="false" />
                        <div class="form-text">Username cannot be changed.</div>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Full Name</label>
                        <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFullName"
                            CssClass="text-danger small" Display="Dynamic" ErrorMessage="Full name is required." ValidationGroup="Profile" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
                            CssClass="text-danger small" Display="Dynamic" ErrorMessage="Email is required." ValidationGroup="Profile" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Member Since</label>
                        <asp:TextBox ID="txtDateCreated" runat="server" CssClass="form-control" Enabled="false" />
                    </div>

                    <asp:Button ID="btnUpdateProfile" runat="server" CssClass="btn btn-dark" Text="Save Changes"
                        OnClick="btnUpdateProfile_Click" ValidationGroup="Profile" />
                </div>
            </div>

            <div class="card">
                <div class="card-header bg-secondary text-white"><i class="bi bi-key"></i> Change Password</div>
                <div class="card-body">

                    <asp:Panel ID="pnlPasswordMessage" runat="server" CssClass="alert" Visible="false" />

                    <div class="mb-3">
                        <label class="form-label">New Password</label>
                        <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtNewPassword"
                            CssClass="text-danger small" Display="Dynamic" ErrorMessage="Password must be at least 8 characters."
                            ValidationExpression=".{8,}" ValidationGroup="Password" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Confirm New Password</label>
                        <asp:TextBox ID="txtConfirmNewPassword" runat="server" CssClass="form-control" TextMode="Password" />
                        <asp:CompareValidator runat="server" ControlToValidate="txtConfirmNewPassword" ControlToCompare="txtNewPassword"
                            CssClass="text-danger small" Display="Dynamic" ErrorMessage="Passwords do not match." ValidationGroup="Password" />
                    </div>

                    <asp:Button ID="btnChangePassword" runat="server" CssClass="btn btn-secondary" Text="Update Password"
                        OnClick="btnChangePassword_Click" ValidationGroup="Password" />
                </div>
            </div>

        </div>
    </div>
</asp:Content>
