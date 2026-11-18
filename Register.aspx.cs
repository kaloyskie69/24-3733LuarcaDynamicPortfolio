using System;
using PersonalWebsiteProject.App_Code;

namespace PersonalWebsiteProject
{
    public partial class Register : System.Web.UI.Page
    {
        private readonly UserRepository _userRepo = new UserRepository();
        private readonly AuditRepository _auditRepo = new AuditRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void btnSignup_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (_userRepo.UsernameOrEmailExists(username, email))
            {
                ShowMessage("An account with that username or email already exists. Please sign in instead.", "alert-warning");
                return;
            }

            int newUserId = _userRepo.Signup(username, email, password, fullName);
            _auditRepo.Log(newUserId, "SIGNUP", string.Format("New account created for '{0}'", username), Request.UserHostAddress);

            ShowMessage("Account created successfully! You can now sign in.", "alert-success");

            txtFullName.Text = "";
            txtUsername.Text = "";
            txtEmail.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
        }

        private void ShowMessage(string message, string cssClass)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = "alert " + cssClass;
            pnlMessage.Controls.Clear();
            pnlMessage.Controls.Add(new System.Web.UI.LiteralControl(message));
        }
    }
}
