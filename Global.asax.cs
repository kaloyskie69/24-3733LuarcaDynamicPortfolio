using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace PersonalWebsiteProject
{
    public partial class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Runs on every request after the user has been authenticated by the Forms
        /// Authentication module. We pull the role we stored in the ticket's UserData
        /// (see Login.aspx.cs) and attach it to a GenericPrincipal so that:
        ///   - User.IsInRole("Admin") works in code-behind
        ///   - <allow roles="Admin"/> works in Web.config <location> sections
        /// </summary>
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null) return;

            try
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket == null || ticket.Expired) return;

                string role = ticket.UserData; // role name was stored here at login
                string[] roles = string.IsNullOrEmpty(role) ? new string[0] : new[] { role };

                var identity = new FormsIdentity(ticket);
                var principal = new GenericPrincipal(identity, roles);
                Context.User = principal;
                System.Threading.Thread.CurrentPrincipal = principal;
            }
            catch
            {
                // Malformed or tampered cookie: treat as unauthenticated.
                FormsAuthentication.SignOut();
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // In production, log Server.GetLastError() to a file or the AuditLogs table.
        }
    }
}
