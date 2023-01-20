using System.Data;
using System.Text;
using static Ecommerce.SqlHandler;

namespace Ecommerce.DataModels
{
    public class PasswordRecovery : Base
    {
        public string Key { get; set; }
        public string UserID { get; set; }
        public DateTime Date { get; set; }
        public DateTime Expiry { get; set; }
        public PasswordRecovery(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    UserID = "";
                    Date = DateTime.UtcNow;
                    Expiry = DateTime.UtcNow.AddHours(1);
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > PasswordRecovery > Constructor " + Ex.Message);
            }
        }
        public void Delete()
        {
            try
            {
                if (!string.IsNullOrEmpty(Key))
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" delete from [PasswordRecovery] ");
                    Sql.Append(" where PasswordRecovery_ID = '" + Key.SanitizeInput() + "'");
                    ExecuteNonQuery(Sql.ToString());
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > PasswordRecovery > Delete " + Ex.Message);
            }
        }
        public bool Exists()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                throw new NotImplementedException("Models > PasswordRecovery > Exists ");
                return Convert.ToInt32(ExecuteScalar(Sql.ToString())) > 0;
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > PasswordRecovery > Exists " + Ex.Message);
            }
        }
        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where PasswordRecovery_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > PasswordRecovery > Read " + Ex.Message);
            }
        }
        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["PasswordRecovery_ID"].ToString();
                UserID = Data["User_ID"].ToString();
                Date = Convert.ToDateTime(Data["Date"].ToString());
                Expiry = Convert.ToDateTime(Data["Expiry"].ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > PasswordRecovery > DataRowToClass " + Ex.Message);
            }
        }
        public bool Save()
        {
            try
            {
                if (string.IsNullOrEmpty(Key))
                {
                    StringBuilder Sql = new StringBuilder();
                    Key = Guid.NewGuid().ToString();
                    Sql.Append("insert into [PasswordRecovery] (");
                    Sql.Append(" PasswordRecovery_ID, ");
                    Sql.Append(" User_ID, ");
                    Sql.Append(" Date, ");
                    Sql.Append(" Expiry ");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "', ");
                    Sql.Append(" '" + UserID.SanitizeInput() + "', ");
                    Sql.Append(" '" + Date.ToDBDate() + "', ");
                    Sql.Append(" '" + Expiry.ToDBDate() + "' ");
                    Sql.Append(" )");
                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append("update [PasswordRecovery] set");
                    Sql.Append(" PasswordRecovery_ID = '" + Key.SanitizeInput() + "', ");
                    Sql.Append(" User_ID = '" + UserID.SanitizeInput() + "', ");
                    Sql.Append(" Date = '" + Date.ToDBDate() + "', ");
                    Sql.Append(" Expiry = '" + Expiry.ToDBDate() + "' ");
                    Sql.Append(" where PasswordRecovery_ID = '" + Key.SanitizeInput() + "'");
                    return ExecuteNonQuery(Sql.ToString());
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > PasswordRecovery > Save " + Ex.Message);
            }
        }
        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" PasswordRecovery_ID, ");
                Sql.Append(" User_ID, ");
                Sql.Append(" Date, ");
                Sql.Append(" Expiry ");
                Sql.Append(" from PasswordRecovery");
                return Sql.ToString();
            }

            public static List<PasswordRecovery> GetUserPasswordRecoveries(string UserID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where User_ID = '" + UserID.SanitizeInput() + "' ");

                    List<PasswordRecovery> PasswordRecoveries = new List<PasswordRecovery>();
                    DataTable Results = ReadDataTable(Sql.ToString());

                    foreach (DataRow PasswordRecoveryDR in Results.Rows)
                    {
                        PasswordRecovery PasswordRecovery = new PasswordRecovery();
                        PasswordRecovery.DataRowToClass(PasswordRecoveryDR);
                        PasswordRecoveries.Add(PasswordRecovery);
                    }

                    return PasswordRecoveries;
                }
                catch (Exception Ex)
                {
                    throw new Exception("PasswordRecoveries > Methods > GetPasswordRecoveries " + Ex.Message);
                }
            }
        }
    }
}
