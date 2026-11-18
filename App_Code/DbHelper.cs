using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PersonalWebsiteProject.App_Code
{
    /// <summary>
    /// Central Data Access Layer. Every method here calls a stored procedure —
    /// there is no inline SQL anywhere in this project.
    /// </summary>
    public class DbHelper
    {
        private static readonly string ConnStr =
            ConfigurationManager.ConnectionStrings["PersonalWebsiteDB"].ConnectionString;

        /// <summary>
        /// Generic executor for stored procedures that return rows (SELECT).
        /// Returns a DataSet so multi-result-set procedures (e.g. stats) work too.
        /// </summary>
        public static DataSet ExecuteReader(string procName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);

                DataSet ds = new DataSet();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(ds);
                }
                return ds;
            }
        }

        /// <summary>
        /// Generic executor for stored procedures that perform INSERT/UPDATE/DELETE.
        /// Returns the number of affected rows.
        /// </summary>
        public static int ExecuteNonQuery(string procName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a stored procedure and returns a single scalar value.
        /// </summary>
        public static object ExecuteScalar(string procName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Executes a stored procedure that has an OUTPUT parameter (e.g. Signup returning new UserId).
        /// The command object is passed back out so the caller can read the output parameter value.
        /// </summary>
        public static void ExecuteNonQueryWithOutput(string procName, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
