using System.Data;
using System.Text;
using static Ecommerce.SqlHandler;

namespace Ecommerce.DataModels
{
    public class User : Base
    {
        public string Key { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastActive { get; set; }

        public List<LinkUserRole> Roles
        {
            get
            {
                return LinkUserRole.Methods.GetUserRoles(Key);
            }
        }

        List<PasswordRecovery> PasswordRecoveries
        {
            get
            {
                return PasswordRecovery.Methods.GetUserPasswordRecoveries(Key);
            }
        }

        public User(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    Email = "";
                    Password = new byte[0];
                    Salt = new byte[0];
                    CreateDate = DateTime.UtcNow;
                    LastActive = DateTime.UtcNow;
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > User > Constructor " + Ex.Message);
            }
        }
        public void Delete()
        {
            try
            {
                if (!string.IsNullOrEmpty(Key))
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" delete from [User] ");
                    Sql.Append(" where User_ID = '" + Key.SanitizeInput() + "'");
                    ExecuteNonQuery(Sql.ToString());
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > User > Delete " + Ex.Message);
            }
        }
        public bool Exists()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append("Select Count(Email) from [User] where Email = '" + Email.SanitizeInput() + "' ");
                return Convert.ToInt32(ExecuteScalar(Sql.ToString())) > 0;
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > User > Exists " + Ex.Message);
            }
        }
        public static DataRow Read(string Key, bool IncludePassword = false)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql(IncludePassword));
                Sql.Append(" where User_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > User > Read " + Ex.Message);
            }
        }
        public void DataRowToClass(DataRow Data, bool IncludePassword = false)
        {
            try
            {
                Key = Data["User_ID"].ToString();
                Email = Data["Email"].ToString();
                if (IncludePassword)
                {
                    Password = Convert.FromBase64String(Data["Password"].ToString());
                    Salt = Convert.FromBase64String(Data["Salt"].ToString());
                }
                CreateDate = Convert.ToDateTime(Data["CreateDate"].ToString());
                LastActive = Convert.ToDateTime(Data["LastActive"].ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > User > DataRowToClass " + Ex.Message);
            }
        }
        public bool Save(bool UpdatePassword = false)
        {
            try
            {
                if (string.IsNullOrEmpty(Key))
                {
                    StringBuilder Sql = new StringBuilder();
                    Key = Guid.NewGuid().ToString();
                    Sql.Append("insert into [User] (");
                    Sql.Append(" User_ID, ");
                    Sql.Append(" Email, ");
                    Sql.Append(" Password, ");
                    Sql.Append(" Salt, ");
                    Sql.Append(" CreateDate, ");
                    Sql.Append(" LastActive ");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "', ");
                    Sql.Append(" '" + Email.SanitizeInput() + "', ");
                    Sql.Append(" '" + Convert.ToBase64String(Password) + "', ");
                    Sql.Append(" '" + Convert.ToBase64String(Salt) + "', ");
                    Sql.Append(" '" + CreateDate.ToDBDate() + "', ");
                    Sql.Append(" '" + LastActive.ToDBDate() + "' ");
                    Sql.Append(" )");
                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append("update [User] set");
                    Sql.Append(" User_ID = '" + Key.SanitizeInput() + "', ");
                    Sql.Append(" Email = '" + Email.SanitizeInput() + "', ");
                    if (UpdatePassword)
                    {
                        Sql.Append(" Password = '" + Convert.ToBase64String(Password) + "', ");
                        Sql.Append(" Salt = '" + Convert.ToBase64String(Salt) + "', ");
                    }
                    Sql.Append(" CreateDate = '" + CreateDate.ToDBDate() + "', ");
                    Sql.Append(" LastActive = '" + LastActive.ToDBDate() + "' ");
                    Sql.Append(" where User_ID = '" + Key.SanitizeInput() + "'");
                    return ExecuteNonQuery(Sql.ToString());
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > User > Save " + Ex.Message);
            }
        }
        public class Methods
        {
            public static string GetReadSql(bool IncludePassword)
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" User_ID, ");
                Sql.Append(" Email, ");
                if (IncludePassword)
                {
                    Sql.Append(" Password, ");
                    Sql.Append(" Salt, ");
                }
                Sql.Append(" CreateDate, ");
                Sql.Append(" LastActive ");
                Sql.Append(" from [User]");
                return Sql.ToString();
            }

            public static Tuple<bool, string> Authenticate(string Email, string Password)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql(true));
                    Sql.Append(" where Email = '" + Email.SanitizeInput() + "'");

                    DataRow UserDR = ReadDataRow(Sql.ToString());

                    if (UserDR != null)
                    {
                        User ExistingUser = new User();
                        ExistingUser.DataRowToClass(UserDR, true);

                        if (Security.VerifyHash(Password, ExistingUser.Salt, ExistingUser.Password))
                        {
                            return new Tuple<bool, string>(true, ExistingUser.Key);
                        }
                        else
                        {
                            return new Tuple<bool, string>(false, "Incorrect Password");
                        }
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "Email is not registered");
                    }
                }
                catch (Exception Ex)
                {
                    throw new Exception("User > Methods > Authenticate " + Ex.Message);
                }
            }

            public static string GetUserIDFromEmail(string Email)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append("select User_ID from [User] where Email = '" + Email.SanitizeInput() + "'");

                    return ExecuteScalar(Sql.ToString());
                }
                catch (Exception Ex)
                {
                    throw new Exception("User > Methods > GetUserIDFromEmail " + Ex.Message);
                }

            }
        }
    }
}
