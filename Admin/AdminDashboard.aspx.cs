using System;
using System.Web.UI.WebControls;
using PersonalWebsiteProject.App_Code;

namespace PersonalWebsiteProject.Admin
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        private readonly UserRepository _userRepo = new UserRepository();
        private readonly ContentRepository _contentRepo = new ContentRepository();
        private readonly AuditRepository _auditRepo = new AuditRepository();

        // Nullable on purpose: 0 is never a valid UserId (IDENTITY starts at 1), and the
        // FK on PersonalDetails.LastModifiedBy / AuditLogs.UserId would reject it. Returning
        // null when we truly can't resolve an admin lets callers pass DBNull instead of a
        // bogus 0 that would crash the save.
        private int? CurrentAdminId
        {
            get { return Session["UserId"] != null ? (int?)Convert.ToInt32(Session["UserId"]) : null; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Belt-and-suspenders check in addition to the Web.config <location path="Admin"> role restriction.
            if (!Request.IsAuthenticated || !User.IsInRole("Admin"))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            // If the Session state was lost (server restart, expired session, etc.) while the
            // login cookie is still valid, re-derive the admin's UserId from their username
            // instead of silently falling back to an invalid value. This is what previously
            // caused a foreign-key crash when saving content — CurrentAdminId came back as 0.
            if (Session["UserId"] == null)
            {
                RehydrateSessionFromUsername();
            }

            if (!IsPostBack)
            {
                BindUsers();
                BindStats();
                BindPersonalDetails();
                BindEducation();
                BindSkills();
                BindHobbies();
                BindProjects();
            }
        }

        private void RehydrateSessionFromUsername()
        {
            var user = _userRepo.GetByUsernameOrEmail(User.Identity.Name);
            if (user != null)
            {
                Session["UserId"] = user.UserId;
                Session["FullName"] = user.FullName;
                Session["RoleName"] = user.RoleName;
            }
        }

        // ==================== USERS TAB ====================

        private void BindUsers()
        {
            gvUsers.DataSource = _userRepo.GetAllUsers();
            gvUsers.DataBind();
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                int userId = Convert.ToInt32(args[0]);
                bool currentlyActive = Convert.ToBoolean(args[1]);

                _userRepo.SetUserActiveStatus(userId, !currentlyActive);
                _auditRepo.Log(CurrentAdminId, "ADMIN_TOGGLE_STATUS",
                    string.Format("Set UserId {0} to {1}", userId, (!currentlyActive ? "Active" : "Inactive")), Request.UserHostAddress);

                ShowMessage(pnlUserMessage, "User status updated.", "alert-success");
                BindUsers();
                BindStats();
            }
            else if (e.CommandName == "DeleteUser")
            {
                int userId = Convert.ToInt32(e.CommandArgument);

                _userRepo.DeleteUser(userId);
                _auditRepo.Log(CurrentAdminId, "ADMIN_DELETE_USER", string.Format("Deleted UserId {0}", userId), Request.UserHostAddress);

                ShowMessage(pnlUserMessage, "User deleted.", "alert-success");
                BindUsers();
                BindStats();
            }
        }

        // ==================== STATISTICS TAB ====================

        private void BindStats()
        {
            var ds = _userRepo.GetUserStats();
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return;

            var row = ds.Tables[0].Rows[0];
            litTotalUsers.Text = row["TotalUsers"].ToString();
            litActiveUsers.Text = row["ActiveUsers"].ToString();
            litInactiveUsers.Text = row["InactiveUsers"].ToString();
            litAdminCount.Text = row["AdminCount"].ToString();
            litRegularCount.Text = row["RegularUserCount"].ToString();
            litSignups7Days.Text = row["SignupsLast7Days"].ToString();

            if (ds.Tables.Count > 2)
            {
                gvActivity.DataSource = ds.Tables[2];
                gvActivity.DataBind();
            }
        }

        // ==================== CONTENT: PERSONAL INFO ====================

        private void BindPersonalDetails()
        {
            var row = _contentRepo.GetPersonalDetails();
            if (row == null) return;

            hdnPersonalDetailId.Value = row["PersonalDetailId"].ToString();
            txtCFullName.Text = row["FullName"].ToString();
            txtCAddress.Text = row["Address"].ToString();
            txtCContact.Text = row["ContactNumber"].ToString();
            txtCEmail.Text = row["Email"].ToString();
            txtCBio.Text = row["Bio"].ToString();
        }

        protected void btnSavePersonal_Click(object sender, EventArgs e)
        {
            int personalDetailId = Convert.ToInt32(hdnPersonalDetailId.Value);

            _contentRepo.UpdatePersonalDetails(
                personalDetailId,
                txtCFullName.Text.Trim(),
                txtCAddress.Text.Trim(),
                txtCContact.Text.Trim(),
                txtCEmail.Text.Trim(),
                txtCBio.Text.Trim(),
                CurrentAdminId);

            _auditRepo.Log(CurrentAdminId, "ADMIN_UPDATE_CONTENT", "Updated personal information content", Request.UserHostAddress);

            ShowMessage(pnlContentMessage, "Personal information updated.", "alert-success");
        }

        // ==================== CONTENT: EDUCATION ====================

        private void BindEducation()
        {
            gvEducation.DataSource = _contentRepo.GetEducation();
            gvEducation.DataBind();
        }

        protected void btnAddEducation_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEduLevel.Text) || string.IsNullOrWhiteSpace(txtEduSchool.Text))
            {
                ShowMessage(pnlContentMessage, "Level and School Name are required for an education entry.", "alert-warning");
                return;
            }

            int? yearFrom = null;
            int yf;
            if (int.TryParse(txtEduYearFrom.Text.Trim(), out yf)) yearFrom = yf;

            _contentRepo.UpsertEducation(null, txtEduLevel.Text.Trim(), txtEduSchool.Text.Trim(),
                string.IsNullOrWhiteSpace(txtEduCourse.Text) ? null : txtEduCourse.Text.Trim(),
                yearFrom, txtEduYearTo.Text.Trim(), 0);

            _auditRepo.Log(CurrentAdminId, "ADMIN_UPDATE_CONTENT", "Added an education entry", Request.UserHostAddress);

            txtEduLevel.Text = ""; txtEduSchool.Text = ""; txtEduCourse.Text = "";
            txtEduYearFrom.Text = ""; txtEduYearTo.Text = "";

            BindEducation();
            ShowMessage(pnlContentMessage, "Education entry added.", "alert-success");
        }

        protected void gvEducation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteEdu")
            {
                int educationId = Convert.ToInt32(e.CommandArgument);
                _contentRepo.DeleteEducation(educationId);
                _auditRepo.Log(CurrentAdminId, "ADMIN_UPDATE_CONTENT", string.Format("Removed education entry {0}", educationId), Request.UserHostAddress);
                BindEducation();
                ShowMessage(pnlContentMessage, "Education entry removed.", "alert-success");
            }
        }

        // ==================== CONTENT: SKILLS ====================

        private void BindSkills()
        {
            gvSkills.DataSource = _contentRepo.GetSkills();
            gvSkills.DataBind();
        }

        protected void btnAddSkill_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewSkill.Text)) return;

            _contentRepo.UpsertSkill(null, txtNewSkill.Text.Trim(), 0);
            _auditRepo.Log(CurrentAdminId, "ADMIN_UPDATE_CONTENT", "Added a skill", Request.UserHostAddress);

            txtNewSkill.Text = "";
            BindSkills();
            ShowMessage(pnlContentMessage, "Skill added.", "alert-success");
        }

        protected void gvSkills_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteSkill")
            {
                int skillId = Convert.ToInt32(e.CommandArgument);
                _contentRepo.DeleteSkill(skillId);
                _auditRepo.Log(CurrentAdminId, "ADMIN_UPDATE_CONTENT", string.Format("Removed skill {0}", skillId), Request.UserHostAddress);
                BindSkills();
                ShowMessage(pnlContentMessage, "Skill removed.", "alert-success");
            }
        }

        // ==================== CONTENT: HOBBIES ====================

        private void BindHobbies()
        {
            gvHobbies.DataSource = _contentRepo.GetHobbies();
            gvHobbies.DataBind();
        }

        protected void btnAddHobby_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewHobby.Text)) return;

            _contentRepo.UpsertHobby(null, txtNewHobby.Text.Trim(), 0);
            _auditRepo.Log(CurrentAdminId, "ADMIN_UPDATE_CONTENT", "Added a hobby", Request.UserHostAddress);

            txtNewHobby.Text = "";
            BindHobbies();
            ShowMessage(pnlContentMessage, "Hobby added.", "alert-success");
        }

        protected void gvHobbies_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteHobby")
            {
                int hobbyId = Convert.ToInt32(e.CommandArgument);
                _contentRepo.DeleteHobby(hobbyId);
                _auditRepo.Log(CurrentAdminId, "ADMIN_UPDATE_CONTENT", string.Format("Removed hobby {0}", hobbyId), Request.UserHostAddress);
                BindHobbies();
                ShowMessage(pnlContentMessage, "Hobby removed.", "alert-success");
            }
        }

        // ==================== CONTENT: MY PROJECTS ====================

        private void BindProjects()
        {
            gvProjects.DataSource = _contentRepo.GetProjects();
            gvProjects.DataBind();
        }

        protected void btnAddProject_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProjectTitle.Text) || string.IsNullOrWhiteSpace(txtProjectDescription.Text))
            {
                ShowMessage(pnlContentMessage, "Title and description are required for a project.", "alert-warning");
                return;
            }

            _contentRepo.UpsertProject(null, txtProjectTitle.Text.Trim(), txtProjectDescription.Text.Trim(),
                string.IsNullOrWhiteSpace(txtProjectTech.Text) ? null : txtProjectTech.Text.Trim(),
                string.IsNullOrWhiteSpace(txtProjectUrl.Text) ? null : txtProjectUrl.Text.Trim(), 0);

            _auditRepo.Log(CurrentAdminId, "ADMIN_UPDATE_CONTENT", "Added a project", Request.UserHostAddress);

            txtProjectTitle.Text = "";
            txtProjectDescription.Text = "";
            txtProjectTech.Text = "";
            txtProjectUrl.Text = "";

            BindProjects();
            ShowMessage(pnlContentMessage, "Project added.", "alert-success");
        }

        protected void gvProjects_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteProject")
            {
                int projectId = Convert.ToInt32(e.CommandArgument);
                _contentRepo.DeleteProject(projectId);
                _auditRepo.Log(CurrentAdminId, "ADMIN_UPDATE_CONTENT", string.Format("Removed project {0}", projectId), Request.UserHostAddress);
                BindProjects();
                ShowMessage(pnlContentMessage, "Project removed.", "alert-success");
            }
        }

        // ==================== HELPERS ====================

        private void ShowMessage(Panel panel, string message, string cssClass)
        {
            panel.Visible = true;
            panel.CssClass = "alert " + cssClass;
            panel.Controls.Clear();
            panel.Controls.Add(new System.Web.UI.LiteralControl(message));
        }
    }
}
