using System;
using System.Data;
using System.Data.SqlClient;

namespace PersonalWebsiteProject.App_Code
{
    /// <summary>
    /// Wraps stored procedures for the dynamic, admin-editable website content:
    /// Personal Information, Educational Attainment, Skills, Hobbies and Interest.
    /// </summary>
    public class ContentRepository
    {
        // ---------------- PERSONAL DETAILS ----------------

        public DataRow GetPersonalDetails()
        {
            DataSet ds = DbHelper.ExecuteReader("sp_Content_GetPersonalDetails");
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;
            return ds.Tables[0].Rows[0];
        }

        public void UpdatePersonalDetails(int personalDetailId, string fullName, string address,
            string contactNumber, string email, string bio, int? modifiedByUserId)
        {
            // modifiedByUserId is nullable on purpose: LastModifiedBy has a foreign key to
            // Users(UserId), so passing 0 (or any id that doesn't exist) would violate that
            // constraint and crash the save. DBNull.Value is always safe there since the
            // column allows NULL.
            DbHelper.ExecuteNonQuery("sp_Content_UpdatePersonalDetails",
                new SqlParameter("@PersonalDetailId", personalDetailId),
                new SqlParameter("@FullName", fullName),
                new SqlParameter("@Address", address),
                new SqlParameter("@ContactNumber", contactNumber),
                new SqlParameter("@Email", email),
                new SqlParameter("@Bio", bio),
                new SqlParameter("@ModifiedByUserId", (object)modifiedByUserId ?? DBNull.Value));
        }

        // ---------------- EDUCATION ----------------

        public DataTable GetEducation()
        {
            DataSet ds = DbHelper.ExecuteReader("sp_Content_GetEducation");
            return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        public void UpsertEducation(int? educationId, string level, string schoolName,
            string course, int? yearFrom, string yearTo, int displayOrder)
        {
            DbHelper.ExecuteNonQuery("sp_Content_UpsertEducation",
                new SqlParameter("@EducationId", (object)educationId ?? DBNull.Value),
                new SqlParameter("@Level", level),
                new SqlParameter("@SchoolName", schoolName),
                new SqlParameter("@Course", (object)course ?? DBNull.Value),
                new SqlParameter("@YearFrom", (object)yearFrom ?? DBNull.Value),
                new SqlParameter("@YearTo", (object)yearTo ?? DBNull.Value),
                new SqlParameter("@DisplayOrder", displayOrder));
        }

        public void DeleteEducation(int educationId)
        {
            DbHelper.ExecuteNonQuery("sp_Content_DeleteEducation", new SqlParameter("@EducationId", educationId));
        }

        // ---------------- SKILLS ----------------

        public DataTable GetSkills()
        {
            DataSet ds = DbHelper.ExecuteReader("sp_Content_GetSkills");
            return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        public void UpsertSkill(int? skillId, string skillName, int displayOrder)
        {
            DbHelper.ExecuteNonQuery("sp_Content_UpsertSkill",
                new SqlParameter("@SkillId", (object)skillId ?? DBNull.Value),
                new SqlParameter("@SkillName", skillName),
                new SqlParameter("@DisplayOrder", displayOrder));
        }

        public void DeleteSkill(int skillId)
        {
            DbHelper.ExecuteNonQuery("sp_Content_DeleteSkill", new SqlParameter("@SkillId", skillId));
        }

        // ---------------- HOBBIES ----------------

        public DataTable GetHobbies()
        {
            DataSet ds = DbHelper.ExecuteReader("sp_Content_GetHobbies");
            return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        public void UpsertHobby(int? hobbyId, string hobbyName, int displayOrder)
        {
            DbHelper.ExecuteNonQuery("sp_Content_UpsertHobby",
                new SqlParameter("@HobbyId", (object)hobbyId ?? DBNull.Value),
                new SqlParameter("@HobbyName", hobbyName),
                new SqlParameter("@DisplayOrder", displayOrder));
        }

        public void DeleteHobby(int hobbyId)
        {
            DbHelper.ExecuteNonQuery("sp_Content_DeleteHobby", new SqlParameter("@HobbyId", hobbyId));
        }

        // ---------------- PROJECTS ("My Projects") ----------------

        public DataTable GetProjects()
        {
            DataSet ds = DbHelper.ExecuteReader("sp_Content_GetProjects");
            return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        public void UpsertProject(int? projectId, string title, string description, string tech, string projectUrl, int displayOrder)
        {
            DbHelper.ExecuteNonQuery("sp_Content_UpsertProject",
                new SqlParameter("@ProjectId", (object)projectId ?? DBNull.Value),
                new SqlParameter("@Title", title),
                new SqlParameter("@Description", description),
                new SqlParameter("@Tech", (object)tech ?? DBNull.Value),
                new SqlParameter("@ProjectUrl", (object)projectUrl ?? DBNull.Value),
                new SqlParameter("@DisplayOrder", displayOrder));
        }

        public void DeleteProject(int projectId)
        {
            DbHelper.ExecuteNonQuery("sp_Content_DeleteProject", new SqlParameter("@ProjectId", projectId));
        }
    }
}
