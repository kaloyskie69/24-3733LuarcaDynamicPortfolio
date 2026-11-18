using System;
using System.Data;
using System.Data.SqlClient;

namespace PersonalWebsiteProject.App_Code
{
    /// <summary>
    /// Wraps every user/auth/admin-related stored procedure call in strongly-typed methods.
    /// </summary>
    public class UserRepository
    {
        /// <summary>
        /// Looks up a user by username or email. Returns null if not found.
        /// Password verification is done by the caller using PasswordHelper.
        /// </summary>
        public UserAccount GetByUsernameOrEmail(string usernameOrEmail)
        {
            DataSet ds = DbHelper.ExecuteReader("sp_User_Login",
                new SqlParameter("@UsernameOrEmail", usernameOrEmail));

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;

            return MapRowToUser(ds.Tables[0].Rows[0]);
        }

        public UserAccount GetById(int userId)
        {
            DataSet ds = DbHelper.ExecuteReader("sp_User_GetById",
                new SqlParameter("@UserId", userId));

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;

            return MapRowToUser(ds.Tables[0].Rows[0]);
        }

        public bool UsernameOrEmailExists(string username, string email)
        {
            DataSet ds = DbHelper.ExecuteReader("sp_User_CheckExists",
                new SqlParameter("@Username", username),
                new SqlParameter("@Email", email));

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return false;

            return Convert.ToInt32(ds.Tables[0].Rows[0]["MatchCount"]) > 0;
        }

        /// <summary>
        /// Creates a new user account. Hashes the password before sending it to SQL.
        /// Returns the newly created UserId.
        /// </summary>
        public int Signup(string username, string email, string plainPassword, string fullName)
        {
            string hash;
            string salt;
            PasswordHelper.CreatePasswordHash(plainPassword, out hash, out salt);

            SqlParameter outputParam = new SqlParameter("@NewUserId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Email", email),
                new SqlParameter("@PasswordHash", hash),
                new SqlParameter("@PasswordSalt", salt),
                new SqlParameter("@FullName", fullName),
                outputParam
            };

            DbHelper.ExecuteNonQueryWithOutput("sp_User_Signup", parameters);

            return Convert.ToInt32(outputParam.Value);
        }

        public void UpdateLastLogin(int userId)
        {
            DbHelper.ExecuteNonQuery("sp_User_UpdateLastLogin", new SqlParameter("@UserId", userId));
        }

        public void UpdateProfile(int userId, string fullName, string email)
        {
            DbHelper.ExecuteNonQuery("sp_User_UpdateProfile",
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FullName", fullName),
                new SqlParameter("@Email", email));
        }

        public void ChangePassword(int userId, string newPlainPassword)
        {
            string hash;
            string salt;
            PasswordHelper.CreatePasswordHash(newPlainPassword, out hash, out salt);

            DbHelper.ExecuteNonQuery("sp_User_ChangePassword",
                new SqlParameter("@UserId", userId),
                new SqlParameter("@PasswordHash", hash),
                new SqlParameter("@PasswordSalt", salt));
        }

        // ---------------- ADMIN ----------------

        public DataTable GetAllUsers()
        {
            DataSet ds = DbHelper.ExecuteReader("sp_Admin_GetAllUsers");
            return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        public void SetUserActiveStatus(int userId, bool isActive)
        {
            DbHelper.ExecuteNonQuery("sp_Admin_SetUserActiveStatus",
                new SqlParameter("@UserId", userId),
                new SqlParameter("@IsActive", isActive));
        }

        public void DeleteUser(int userId)
        {
            DbHelper.ExecuteNonQuery("sp_Admin_DeleteUser", new SqlParameter("@UserId", userId));
        }

        public DataSet GetUserStats()
        {
            // Returns 3 tables: [0] summary counts, [1] daily signups (30 days), [2] recent activity
            return DbHelper.ExecuteReader("sp_Admin_GetUserStats");
        }

        // ---------------- MAPPING ----------------

        private UserAccount MapRowToUser(DataRow row)
        {
            return new UserAccount
            {
                UserId = Convert.ToInt32(row["UserId"]),
                Username = row["Username"].ToString(),
                Email = row["Email"].ToString(),
                PasswordHash = row.Table.Columns.Contains("PasswordHash") ? row["PasswordHash"].ToString() : null,
                PasswordSalt = row.Table.Columns.Contains("PasswordSalt") ? row["PasswordSalt"].ToString() : null,
                RoleId = Convert.ToInt32(row["RoleId"]),
                RoleName = row["RoleName"].ToString(),
                FullName = row["FullName"] == DBNull.Value ? "" : row["FullName"].ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                DateCreated = row.Table.Columns.Contains("DateCreated") ? Convert.ToDateTime(row["DateCreated"]) : DateTime.MinValue,
                LastLogin = row.Table.Columns.Contains("LastLogin") && row["LastLogin"] != DBNull.Value
                    ? Convert.ToDateTime(row["LastLogin"])
                    : (DateTime?)null
            };
        }
    }
}
