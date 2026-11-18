<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="AdminDashboard.aspx.cs" Inherits="PersonalWebsiteProject.Admin.AdminDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h3 class="page-title mb-1"><i class="bi bi-speedometer2"></i> Admin <span class="accent-text">Dashboard</span></h3>
    <p class="page-subtitle mb-4">Manage users, monitor activity, and edit your public portfolio content.</p>

    <!-- Remembers which tab was open so a postback (e.g. saving content) never
         bounces the admin back to the Users tab. Restored/updated by the script
         at the bottom of this page. -->
    <asp:HiddenField ID="hdnActiveTab" runat="server" Value="tabUsers" />

    <ul class="nav nav-tabs mb-4" id="adminTabs" role="tablist">
        <li class="nav-item" role="presentation">
            <button class="nav-link active" data-bs-toggle="tab" data-bs-target="#tabUsers" type="button">
                <i class="bi bi-people"></i> Users
            </button>
        </li>
        <li class="nav-item" role="presentation">
            <button class="nav-link" data-bs-toggle="tab" data-bs-target="#tabStats" type="button">
                <i class="bi bi-bar-chart"></i> Statistics
            </button>
        </li>
        <li class="nav-item" role="presentation">
            <button class="nav-link" data-bs-toggle="tab" data-bs-target="#tabContent" type="button">
                <i class="bi bi-pencil-square"></i> Website Content
            </button>
        </li>
    </ul>

    <div class="tab-content">

        <!-- ============ USERS TAB ============ -->
        <div class="tab-pane fade show active" id="tabUsers">
            <!--
                This whole tab lives inside an UpdatePanel, so Deactivate/Activate/Delete
                trigger an async partial postback instead of a full page reload — the
                Users table updates in place instantly with no browser navigation at all.
                UpdateMode="Always" guarantees this panel re-renders on every postback on
                the page, so it can never show stale data after an action.
            -->
            <asp:UpdatePanel ID="upUsers" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <div class="card">
                        <div class="card-body">
                            <asp:Panel ID="pnlUserMessage" runat="server" CssClass="alert" Visible="false" />

                            <div class="table-responsive">
                                <asp:GridView ID="gvUsers" runat="server" CssClass="table table-hover align-middle" AutoGenerateColumns="false"
                                    DataKeyNames="UserId" OnRowCommand="gvUsers_RowCommand" EmptyDataText="No users found.">
                                    <Columns>
                                        <asp:BoundField DataField="Username" HeaderText="Username" />
                                        <asp:BoundField DataField="FullName" HeaderText="Full Name" />
                                        <asp:BoundField DataField="Email" HeaderText="Email" />
                                        <asp:BoundField DataField="RoleName" HeaderText="Role" />
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <span class='badge <%# (bool)Eval("IsActive") ? "badge-active" : "badge-inactive" %>'>
                                                    <%# (bool)Eval("IsActive") ? "Active" : "Inactive" %>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="DateCreated" HeaderText="Joined" DataFormatString="{0:MMM dd, yyyy}" />
                                        <asp:TemplateField HeaderText="Last Login">
                                            <ItemTemplate>
                                                <%# Eval("LastLogin") == DBNull.Value ? "Never" : string.Format("{0:MMM dd, yyyy}", Eval("LastLogin")) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Actions">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary me-1"
                                                    CommandName="ToggleStatus" CommandArgument='<%# Eval("UserId") + "|" + Eval("IsActive") %>'>
                                                    <%# (bool)Eval("IsActive") ? "Deactivate" : "Activate" %>
                                                </asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger"
                                                    CommandName="DeleteUser" CommandArgument='<%# Eval("UserId") %>'
                                                    OnClientClick="return confirm('Permanently delete this user? This cannot be undone.');">
                                                    Delete
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>

                            <asp:UpdateProgress ID="upgUsers" runat="server" AssociatedUpdatePanelID="upUsers">
                                <ProgressTemplate>
                                    <div class="pw-update-overlay">
                                        <span class="pw-spinner"></span> Updating...
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- ============ STATISTICS TAB ============ -->
        <div class="tab-pane fade" id="tabStats">
            <asp:UpdatePanel ID="upStats" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <div class="row mb-4">
                        <div class="col-md-2 col-6 mb-3">
                            <div class="card stat-card">
                                <div class="stat-icon stat-icon-blue"><i class="bi bi-people-fill"></i></div>
                                <div class="stat-number stat-number-blue"><asp:Literal ID="litTotalUsers" runat="server" /></div>
                                <div class="text-muted">Total Users</div>
                            </div>
                        </div>
                        <div class="col-md-2 col-6 mb-3">
                            <div class="card stat-card">
                                <div class="stat-icon stat-icon-green"><i class="bi bi-check-circle-fill"></i></div>
                                <div class="stat-number stat-number-green"><asp:Literal ID="litActiveUsers" runat="server" /></div>
                                <div class="text-muted">Active</div>
                            </div>
                        </div>
                        <div class="col-md-2 col-6 mb-3">
                            <div class="card stat-card">
                                <div class="stat-icon stat-icon-gray"><i class="bi bi-pause-circle-fill"></i></div>
                                <div class="stat-number stat-number-gray"><asp:Literal ID="litInactiveUsers" runat="server" /></div>
                                <div class="text-muted">Inactive</div>
                            </div>
                        </div>
                        <div class="col-md-2 col-6 mb-3">
                            <div class="card stat-card">
                                <div class="stat-icon stat-icon-purple"><i class="bi bi-shield-lock-fill"></i></div>
                                <div class="stat-number stat-number-purple"><asp:Literal ID="litAdminCount" runat="server" /></div>
                                <div class="text-muted">Admins</div>
                            </div>
                        </div>
                        <div class="col-md-2 col-6 mb-3">
                            <div class="card stat-card">
                                <div class="stat-icon stat-icon-teal"><i class="bi bi-person-fill"></i></div>
                                <div class="stat-number stat-number-teal"><asp:Literal ID="litRegularCount" runat="server" /></div>
                                <div class="text-muted">Regular Users</div>
                            </div>
                        </div>
                        <div class="col-md-2 col-6 mb-3">
                            <div class="card stat-card">
                                <div class="stat-icon stat-icon-amber"><i class="bi bi-graph-up-arrow"></i></div>
                                <div class="stat-number stat-number-amber"><asp:Literal ID="litSignups7Days" runat="server" /></div>
                                <div class="text-muted">New (7 days)</div>
                            </div>
                        </div>
                    </div>

                    <div class="card">
                        <div class="card-header section-header"><span class="section-icon section-icon-slate"><i class="bi bi-clock-history"></i></span> Recent Activity</div>
                        <div class="card-body">
                            <div class="table-responsive">
                                <asp:GridView ID="gvActivity" runat="server" CssClass="table table-sm" AutoGenerateColumns="false"
                                    EmptyDataText="No recent activity.">
                                    <Columns>
                                        <asp:BoundField DataField="ActionDate" HeaderText="When" DataFormatString="{0:MMM dd, yyyy hh:mm tt}" />
                                        <asp:BoundField DataField="Username" HeaderText="User" />
                                        <asp:BoundField DataField="ActionType" HeaderText="Action" />
                                        <asp:BoundField DataField="ActionDetail" HeaderText="Detail" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- ============ CONTENT EDITOR TAB ============ -->
        <div class="tab-pane fade" id="tabContent">
            <!--
                Same AJAX pattern as the Users tab: every Save/Add/Remove action here
                triggers only an async partial postback, so the page never navigates
                and the Content tab stays open and instantly reflects the change.
            -->
            <asp:UpdatePanel ID="upContent" runat="server" UpdateMode="Always">
                <ContentTemplate>

                    <!-- Personal Information -->
                    <div class="card mb-4">
                        <div class="card-header section-header"><span class="section-icon section-icon-slate"><i class="bi bi-person-vcard"></i></span> Personal Information</div>
                        <div class="card-body">
                            <asp:Panel ID="pnlContentMessage" runat="server" CssClass="alert" Visible="false" />
                            <asp:HiddenField ID="hdnPersonalDetailId" runat="server" />

                            <div class="mb-3">
                                <label class="form-label">Full Name</label>
                                <asp:TextBox ID="txtCFullName" runat="server" CssClass="form-control" />
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Address</label>
                                <asp:TextBox ID="txtCAddress" runat="server" CssClass="form-control" />
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Contact Number</label>
                                <asp:TextBox ID="txtCContact" runat="server" CssClass="form-control" />
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Email</label>
                                <asp:TextBox ID="txtCEmail" runat="server" CssClass="form-control" />
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Bio / Summary</label>
                                <asp:TextBox ID="txtCBio" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" />
                            </div>
                            <asp:Button ID="btnSavePersonal" runat="server" CssClass="btn btn-dark" Text="Save Personal Information" OnClick="btnSavePersonal_Click" />
                        </div>
                    </div>

                    <div class="row">
                        <!-- Education -->
                        <div class="col-md-6 mb-4">
                            <div class="card h-100">
                                <div class="card-header section-header"><span class="section-icon section-icon-blue"><i class="bi bi-mortarboard"></i></span> Educational Attainment</div>
                                <div class="card-body">
                                    <asp:GridView ID="gvEducation" runat="server" CssClass="table table-sm" AutoGenerateColumns="false"
                                        DataKeyNames="EducationId" OnRowCommand="gvEducation_RowCommand" EmptyDataText="No education entries.">
                                        <Columns>
                                            <asp:BoundField DataField="Level" HeaderText="Level" />
                                            <asp:BoundField DataField="SchoolName" HeaderText="School" />
                                            <asp:TemplateField HeaderText="Years">
                                                <ItemTemplate><%# Eval("YearFrom") %>-<%# Eval("YearTo") %></ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger"
                                                        CommandName="DeleteEdu" CommandArgument='<%# Eval("EducationId") %>'
                                                        OnClientClick="return confirm('Remove this entry?');">Remove</asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <hr />
                                    <div class="row g-2">
                                        <div class="col-6"><asp:TextBox ID="txtEduLevel" runat="server" CssClass="form-control form-control-sm" placeholder="Level (e.g. College)" /></div>
                                        <div class="col-6"><asp:TextBox ID="txtEduSchool" runat="server" CssClass="form-control form-control-sm" placeholder="School Name" /></div>
                                        <div class="col-6"><asp:TextBox ID="txtEduCourse" runat="server" CssClass="form-control form-control-sm" placeholder="Course (optional)" /></div>
                                        <div class="col-3"><asp:TextBox ID="txtEduYearFrom" runat="server" CssClass="form-control form-control-sm" placeholder="Year From" /></div>
                                        <div class="col-3"><asp:TextBox ID="txtEduYearTo" runat="server" CssClass="form-control form-control-sm" placeholder="Year To" /></div>
                                        <div class="col-12">
                                            <asp:Button ID="btnAddEducation" runat="server" CssClass="btn btn-sm btn-primary mt-2" Text="Add Entry" OnClick="btnAddEducation_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Skills -->
                        <div class="col-md-6 mb-4">
                            <div class="card h-100">
                                <div class="card-header section-header"><span class="section-icon section-icon-green"><i class="bi bi-stars"></i></span> Skills</div>
                                <div class="card-body">
                                    <asp:GridView ID="gvSkills" runat="server" CssClass="table table-sm" AutoGenerateColumns="false"
                                        DataKeyNames="SkillId" OnRowCommand="gvSkills_RowCommand" EmptyDataText="No skills yet.">
                                        <Columns>
                                            <asp:BoundField DataField="SkillName" HeaderText="Skill" />
                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger"
                                                        CommandName="DeleteSkill" CommandArgument='<%# Eval("SkillId") %>'
                                                        OnClientClick="return confirm('Remove this skill?');">Remove</asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <hr />
                                    <div class="input-group input-group-sm">
                                        <asp:TextBox ID="txtNewSkill" runat="server" CssClass="form-control" placeholder="New skill" />
                                        <asp:Button ID="btnAddSkill" runat="server" CssClass="btn btn-primary" Text="Add" OnClick="btnAddSkill_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Hobbies -->
                    <div class="card mb-4">
                        <div class="card-header section-header"><span class="section-icon section-icon-amber"><i class="bi bi-controller"></i></span> Hobbies and Interest</div>
                        <div class="card-body">
                            <asp:GridView ID="gvHobbies" runat="server" CssClass="table table-sm" AutoGenerateColumns="false"
                                DataKeyNames="HobbyId" OnRowCommand="gvHobbies_RowCommand" EmptyDataText="No hobbies yet.">
                                <Columns>
                                    <asp:BoundField DataField="HobbyName" HeaderText="Hobby" />
                                    <asp:TemplateField HeaderText="">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger"
                                                CommandName="DeleteHobby" CommandArgument='<%# Eval("HobbyId") %>'
                                                OnClientClick="return confirm('Remove this hobby?');">Remove</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <hr />
                            <div class="input-group input-group-sm" style="max-width: 400px;">
                                <asp:TextBox ID="txtNewHobby" runat="server" CssClass="form-control" placeholder="New hobby" />
                                <asp:Button ID="btnAddHobby" runat="server" CssClass="btn btn-primary" Text="Add" OnClick="btnAddHobby_Click" />
                            </div>
                        </div>
                    </div>

                    <!-- My Projects -->
                    <div class="card mb-4">
                        <div class="card-header section-header"><span class="section-icon section-icon-purple"><i class="bi bi-kanban"></i></span> My Projects</div>
                        <div class="card-body">
                            <asp:GridView ID="gvProjects" runat="server" CssClass="table table-sm" AutoGenerateColumns="false"
                                DataKeyNames="ProjectId" OnRowCommand="gvProjects_RowCommand" EmptyDataText="No projects yet.">
                                <Columns>
                                    <asp:BoundField DataField="Title" HeaderText="Title" />
                                    <asp:BoundField DataField="Description" HeaderText="Description" />
                                    <asp:BoundField DataField="Tech" HeaderText="Tech" />
                                    <asp:TemplateField HeaderText="">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger"
                                                CommandName="DeleteProject" CommandArgument='<%# Eval("ProjectId") %>'
                                                OnClientClick="return confirm('Remove this project?');">Remove</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <hr />
                            <div class="row g-2">
                                <div class="col-md-6"><asp:TextBox ID="txtProjectTitle" runat="server" CssClass="form-control form-control-sm" placeholder="Project Title" /></div>
                                <div class="col-md-6"><asp:TextBox ID="txtProjectTech" runat="server" CssClass="form-control form-control-sm" placeholder="Tech (comma-separated, e.g. ASP.NET, C#, SQL Server)" /></div>
                                <div class="col-12"><asp:TextBox ID="txtProjectDescription" runat="server" CssClass="form-control form-control-sm" placeholder="Short description" TextMode="MultiLine" Rows="2" /></div>
                                <div class="col-12"><asp:TextBox ID="txtProjectUrl" runat="server" CssClass="form-control form-control-sm" placeholder="Link (optional — live demo or repo URL)" /></div>
                                <div class="col-12">
                                    <asp:Button ID="btnAddProject" runat="server" CssClass="btn btn-sm btn-primary mt-1" Text="Add Project" OnClick="btnAddProject_Click" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <asp:UpdateProgress ID="upgContent" runat="server" AssociatedUpdatePanelID="upContent">
                        <ProgressTemplate>
                            <div class="pw-update-overlay">
                                <span class="pw-spinner"></span> Saving...
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <script type="text/javascript">
        (function () {
            var hiddenField = document.getElementById('<%= hdnActiveTab.ClientID %>');

            function activateTab(tabId) {
                if (!tabId) return;
                var trigger = document.querySelector('#adminTabs button[data-bs-target="#' + tabId + '"]');
                if (trigger && window.bootstrap) {
                    var tabInstance = window.bootstrap.Tab.getOrCreateInstance(trigger);
                    tabInstance.show();
                }
            }

            // Restore whichever tab was active before this postback (full or async).
            activateTab(hiddenField.value);

            // Remember the tab the admin switches to, so the next postback (Save,
            // Add, Remove, Deactivate, Delete, etc.) reopens on the same tab.
            document.querySelectorAll('#adminTabs button[data-bs-toggle="tab"]').forEach(function (btn) {
                btn.addEventListener('shown.bs.tab', function (evt) {
                    var targetId = evt.target.getAttribute('data-bs-target').replace('#', '');
                    hiddenField.value = targetId;
                });
            });

            // Re-run tab restoration after every async (UpdatePanel) partial postback too.
            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    activateTab(hiddenField.value);
                });
            }
        })();
    </script>

</asp:Content>
