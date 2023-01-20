using System.Data;
using System.Text;
using static Ecommerce.SqlHandler;

namespace Ecommerce.DataModels
{
    public class LinkUserRole : Base
    {
        public string Key { get; set; }
        public string UserID { get; set; }
        public Enums.UserRole RoleID { get; set; }
        public Enums.AccessType AccessType { get; set; }
        public LinkUserRole(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    UserID = "";
                    RoleID = Enums.UserRole.Unknown;
                    AccessType = Enums.AccessType.Unknown;
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > LinkUserRole > Constructor " + Ex.Message);
            }
        }
        public void Delete()
        {
            try
            {
                if (!string.IsNullOrEmpty(Key))
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" delete from [LinkUserRole] ");
                    Sql.Append(" where LinkUserRole_ID = '" + Key.SanitizeInput() + "'");
                    ExecuteNonQuery(Sql.ToString());
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > LinkUserRole > Delete " + Ex.Message);
            }
        }
        public bool Exists()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select count(*) from LinkUserRole");
                Sql.Append(" where User_ID = '" + UserID.SanitizeInput() + "' and Role_ID = " + (int)RoleID + "");
                return Convert.ToInt32(ExecuteScalar(Sql.ToString())) > 0;
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > LinkUserRole > Exists " + Ex.Message);
            }
        }
        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where LinkUserRole_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > LinkUserRole > Read " + Ex.Message);
            }
        }
        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["LinkUserRole_ID"].ToString();
                UserID = Data["User_ID"].ToString();
                RoleID = (Enums.UserRole)Convert.ToInt32(Data["Role_ID"].ToString());
                AccessType = (Enums.AccessType)Convert.ToInt32(Data["AccessType_ID"].ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > LinkUserRole > DataRowToClass " + Ex.Message);
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
                    Sql.Append("insert into [LinkUserRole] (");
                    Sql.Append(" LinkUserRole_ID, ");
                    Sql.Append(" User_ID, ");
                    Sql.Append(" Role_ID, ");
                    Sql.Append(" AccessType_ID ");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "', ");
                    Sql.Append(" '" + UserID.SanitizeInput() + "', ");
                    Sql.Append(" " + (int)RoleID + " ");
                    Sql.Append(" )");
                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append("update [LinkUserRole] set");
                    Sql.Append(" LinkUserRole_ID = '" + Key.SanitizeInput() + "', ");
                    Sql.Append(" User_ID = '" + UserID.SanitizeInput() + "', ");
                    Sql.Append(" Role_ID = " + (int)RoleID + ", ");
                    Sql.Append(" AccessType_ID = " + (int)AccessType + " ");
                    Sql.Append(" where LinkUserRole_ID = '" + Key.SanitizeInput() + "'");
                    return ExecuteNonQuery(Sql.ToString());
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > LinkUserRole > Save " + Ex.Message);
            }
        }
        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" LinkUserRole_ID, ");
                Sql.Append(" User_ID, ");
                Sql.Append(" Role_ID, ");
                Sql.Append(" AccessType_ID ");
                Sql.Append(" from LinkUserRole");
                return Sql.ToString();
            }

            public static List<LinkUserRole> GetUserRoles(string UserID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where User_ID = '" + UserID.SanitizeInput() + "'");

                    List<LinkUserRole> Roles = new List<LinkUserRole>();
                    DataTable Results = ReadDataTable(Sql.ToString());

                    foreach (DataRow RoleDR in Results.Rows)
                    {
                        LinkUserRole UserRole = new LinkUserRole();
                        UserRole.DataRowToClass(RoleDR);
                        Roles.Add(UserRole);
                    }

                    return Roles;
                }
                catch (Exception Ex)
                {
                    throw new Exception("LinkUserRole > Methods > GetUserRoles " + Ex.Message);
                }
            }
        }
    }
}
