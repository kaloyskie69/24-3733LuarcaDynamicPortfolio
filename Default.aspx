<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="PersonalWebsiteProject._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h3 class="page-title mb-1">Welcome, <span class="accent-text"><asp:Literal ID="litPageGreetingName" runat="server" /></span></h3>
    <p class="page-subtitle mb-4">Here's the portfolio content currently published on this site.</p>

    <!-- Personal Information -->
    <div class="card mb-4">
        <div class="card-header section-header">
            <span class="section-icon section-icon-slate"><i class="bi bi-person-vcard"></i></span> Personal Information
        </div>
        <div class="card-body">
            <div class="d-flex align-items-center gap-3 mb-3">
                <span class="account-avatar profile-avatar"><asp:Literal ID="litAvatarInitialBig" runat="server" /></span>
                <h4 class="mb-0"><asp:Literal ID="litFullName" runat="server" /></h4>
            </div>
            <p class="mb-1"><i class="bi bi-geo-alt"></i> <asp:Literal ID="litAddress" runat="server" /></p>
            <p class="mb-1"><i class="bi bi-telephone"></i> <asp:Literal ID="litContact" runat="server" /></p>
            <p class="mb-3"><i class="bi bi-envelope"></i> <asp:Literal ID="litEmail" runat="server" /></p>
            <p class="text-muted"><asp:Literal ID="litBio" runat="server" /></p>
        </div>
    </div>

    <div class="row">
        <!-- Educational Attainment -->
        <div class="col-md-6 mb-4">
            <div class="card h-100">
                <div class="card-header section-header">
                    <span class="section-icon section-icon-blue"><i class="bi bi-mortarboard"></i></span> Educational Attainment
                </div>
                <div class="card-body">
                    <asp:Repeater ID="rptEducation" runat="server">
                        <ItemTemplate>
                            <div class="mb-3 border-start border-3 border-primary ps-3">
                                <div class="fw-semibold"><%# Eval("Level") %></div>
                                <div><%# Eval("SchoolName") %></div>
                                <asp:PlaceHolder runat="server" Visible='<%# Eval("Course") != DBNull.Value %>'>
                                    <div class="text-muted small"><%# Eval("Course") %></div>
                                </asp:PlaceHolder>
                                <div class="text-muted small"><%# Eval("YearFrom") %> - <%# Eval("YearTo") %></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>

        <!-- Skills -->
        <div class="col-md-6 mb-4">
            <div class="card h-100">
                <div class="card-header section-header">
                    <span class="section-icon section-icon-green"><i class="bi bi-stars"></i></span> Skills
                </div>
                <div class="card-body">
                    <ul class="list-group list-group-flush">
                        <asp:Repeater ID="rptSkills" runat="server">
                            <ItemTemplate>
                                <li class="list-group-item"><i class="bi bi-check2-circle text-success"></i> <%# Eval("SkillName") %></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
            </div>
        </div>
    </div>

    <!-- Hobbies and Interest -->
    <div class="card mb-4">
        <div class="card-header section-header">
            <span class="section-icon section-icon-amber"><i class="bi bi-controller"></i></span> Hobbies and Interest
        </div>
        <div class="card-body">
            <asp:Repeater ID="rptHobbies" runat="server">
                <ItemTemplate>
                    <span class="badge rounded-pill hobby-badge"><%# Eval("HobbyName") %></span>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <!-- My Projects -->
    <div class="card mb-4">
        <div class="card-header section-header">
            <span class="section-icon section-icon-purple"><i class="bi bi-kanban"></i></span> My Projects
        </div>
        <div class="card-body">
            <p class="text-muted mb-3">A few things I've built recently.</p>
            <div class="project-grid">
                <asp:Repeater ID="rptProjects" runat="server">
                    <ItemTemplate>
                        <div class="project-card">
                            <h5>
                                <%# Eval("Title") %>
                                <asp:PlaceHolder runat="server" Visible='<%# Eval("ProjectUrl") != DBNull.Value && Eval("ProjectUrl").ToString() != "" %>'>
                                    <a href='<%# Eval("ProjectUrl") %>' target="_blank" rel="noopener" class="project-link" title="View project">
                                        <i class="bi bi-box-arrow-up-right"></i>
                                    </a>
                                </asp:PlaceHolder>
                            </h5>
                            <p class="text-muted small mb-2"><%# Eval("Description") %></p>
                            <asp:PlaceHolder runat="server" Visible='<%# Eval("Tech") != DBNull.Value && Eval("Tech").ToString() != "" %>'>
                                <div class="tech-tags">
                                    <%# RenderTechTags(Eval("Tech")) %>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>

</asp:Content>
