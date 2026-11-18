using System.Data.SqlClient;

namespace PersonalWebsiteProject.App_Code
{
    public class AuditRepository
    {
        public void Log(int? userId, string actionType, string actionDetail, string ipAddress)
        {
            DbHelper.ExecuteNonQuery("sp_Audit_InsertLog",
                new SqlParameter("@UserId", (object)userId ?? System.DBNull.Value),
                new SqlParameter("@ActionType", actionType),
                new SqlParameter("@ActionDetail", (object)actionDetail ?? System.DBNull.Value),
                new SqlParameter("@IPAddress", (object)ipAddress ?? System.DBNull.Value));
        }
    }
}
