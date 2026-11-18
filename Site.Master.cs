using System;
using System.Web.Security;

namespace PersonalWebsiteProject
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                pnlLoggedIn.Visible = true;
                string name = Context.User.Identity.Name;
                litWelcomeName.Text = Server.HtmlEncode(name);
                litAvatarInitial.Text = string.IsNullOrEmpty(name) ? "?" : Server.HtmlEncode(name.Substring(0, 1).ToUpper());
                lnkAdmin.Visible = Context.User.IsInRole("Admin");
            }
            else
            {
                pnlLoggedIn.Visible = false;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}
