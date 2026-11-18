using System;
using PersonalWebsiteProject.App_Code;

namespace PersonalWebsiteProject
{
    public partial class _Default : System.Web.UI.Page
    {
        private readonly ContentRepository _contentRepo = new ContentRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Web.config already denies anonymous users site-wide, but we double check here
            // since this page displays personal data.
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                litPageGreetingName.Text = Server.HtmlEncode(Context.User.Identity.Name);
                BindPersonalDetails();
                BindEducation();
                BindSkills();
                BindHobbies();
                BindProjects();
            }
        }

        /// <summary>
        /// Splits a comma-separated tech-stack string (e.g. "ASP.NET, C#, SQL Server")
        /// into individual HTML-encoded pill spans for display.
        /// </summary>
        protected string RenderTechTags(object techTagsRaw)
        {
            if (techTagsRaw == null || techTagsRaw == DBNull.Value) return string.Empty;

            string[] tags = techTagsRaw.ToString().Split(',');
            var sb = new System.Text.StringBuilder();

            foreach (string tag in tags)
            {
                string trimmed = tag.Trim();
                if (trimmed.Length == 0) continue;
                sb.Append("<span class=\"tech-tag\">");
                sb.Append(Server.HtmlEncode(trimmed));
                sb.Append("</span>");
            }

            return sb.ToString();
        }

        private void BindPersonalDetails()
        {
            var row = _contentRepo.GetPersonalDetails();
            if (row == null) return;

            litFullName.Text = Server.HtmlEncode(row["FullName"].ToString());
            litAvatarInitialBig.Text = row["FullName"].ToString().Length > 0
                ? Server.HtmlEncode(row["FullName"].ToString().Substring(0, 1).ToUpper())
                : "?";
            litAddress.Text = Server.HtmlEncode(row["Address"].ToString());
            litContact.Text = Server.HtmlEncode(row["ContactNumber"].ToString());
            litEmail.Text = Server.HtmlEncode(row["Email"].ToString());
            litBio.Text = Server.HtmlEncode(row["Bio"].ToString());
        }

        private void BindEducation()
        {
            rptEducation.DataSource = _contentRepo.GetEducation();
            rptEducation.DataBind();
        }

        private void BindSkills()
        {
            rptSkills.DataSource = _contentRepo.GetSkills();
            rptSkills.DataBind();
        }

        private void BindHobbies()
        {
            rptHobbies.DataSource = _contentRepo.GetHobbies();
            rptHobbies.DataBind();
        }

        private void BindProjects()
        {
            rptProjects.DataSource = _contentRepo.GetProjects();
            rptProjects.DataBind();
        }
    }
}
