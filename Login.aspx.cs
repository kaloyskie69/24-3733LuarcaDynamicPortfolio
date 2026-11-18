using System;
using System.Web;
using System.Web.Security;
using PersonalWebsiteProject.App_Code;

namespace PersonalWebsiteProject
{
    public partial class Login : System.Web.UI.Page
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

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string usernameOrEmail = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            UserAccount user = _userRepo.GetByUsernameOrEmail(usernameOrEmail);

            if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                ShowError("Invalid username/email or password.");
                _auditRepo.Log(null, "LOGIN_FAILED", string.Format("Failed login attempt for '{0}'", usernameOrEmail), Request.UserHostAddress);
                return;
            }

            if (!user.IsActive)
            {
                ShowError("Your account has been deactivated. Please contact the administrator.");
                _auditRepo.Log(user.UserId, "LOGIN_BLOCKED", "Inactive account attempted login", Request.UserHostAddress);
                return;
            }

            // Success: issue the Forms Authentication ticket, embedding the role in UserData
            // so Global.asax can reconstruct a role-aware principal on every request
            // (this is what makes <allow roles="Admin"/> in Web.config actually work).
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                user.Username,
                DateTime.Now,
                DateTime.Now.AddMinutes(60),
                false,
                user.RoleName,                 // UserData carries the role name
                FormsAuthentication.FormsCookiePath);

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            Response.Cookies.Add(authCookie);

            // Session convenience values for display purposes (e.g. "Hi, <name>")
            Session["UserId"] = user.UserId;
            Session["FullName"] = user.FullName;
            Session["RoleName"] = user.RoleName;

            _userRepo.UpdateLastLogin(user.UserId);
            _auditRepo.Log(user.UserId, "LOGIN_SUCCESS", "User logged in", Request.UserHostAddress);

            if (user.IsAdmin)
                Response.Redirect("~/Admin/AdminDashboard.aspx");
            else
                Response.Redirect("~/Default.aspx");
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlError.Controls.Clear();
            pnlError.Controls.Add(new System.Web.UI.LiteralControl(message));
        }
    }
}
