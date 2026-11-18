using System;
using System.Web.Security;
using PersonalWebsiteProject.App_Code;

namespace PersonalWebsiteProject
{
    public partial class UserProfile : System.Web.UI.Page
    {
        private readonly UserRepository _userRepo = new UserRepository();
        private readonly AuditRepository _auditRepo = new AuditRepository();

        private int CurrentUserId { get { return Convert.ToInt32(Session["UserId"]); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Guard against a recycled app pool clearing Session while the auth cookie is still valid.
            if (Session["UserId"] == null)
            {
                RehydrateSessionFromUsername();
            }

            if (!IsPostBack)
            {
                LoadProfile();
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

        private void LoadProfile()
        {
            var user = _userRepo.GetById(CurrentUserId);
            if (user == null) return;

            txtUsernameDisplay.Text = user.Username;
            txtFullName.Text = user.FullName;
            txtEmail.Text = user.Email;
            txtDateCreated.Text = user.DateCreated.ToString("MMMM dd, yyyy");
        }

        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            _userRepo.UpdateProfile(CurrentUserId, txtFullName.Text.Trim(), txtEmail.Text.Trim());
            _auditRepo.Log(CurrentUserId, "UPDATE_PROFILE", "User updated their own profile information", Request.UserHostAddress);

            ShowMessage(pnlProfileMessage, "Profile updated successfully.", "alert-success");
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                ShowMessage(pnlPasswordMessage, "Please enter a new password.", "alert-warning");
                return;
            }

            _userRepo.ChangePassword(CurrentUserId, txtNewPassword.Text);
            _auditRepo.Log(CurrentUserId, "CHANGE_PASSWORD", "User changed their password", Request.UserHostAddress);

            txtNewPassword.Text = "";
            txtConfirmNewPassword.Text = "";

            ShowMessage(pnlPasswordMessage, "Password updated successfully.", "alert-success");
        }

        private void ShowMessage(System.Web.UI.WebControls.Panel panel, string message, string cssClass)
        {
            panel.Visible = true;
            panel.CssClass = "alert " + cssClass;
            panel.Controls.Clear();
            panel.Controls.Add(new System.Web.UI.LiteralControl(message));
        }
    }
}
