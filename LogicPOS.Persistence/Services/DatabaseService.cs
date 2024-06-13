using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using LogicPOS.Settings;
using System.IO;

namespace LogicPOS.Persistence.Services
{
    public class DatabaseService
    {
        public static bool DatabaseExists()
        {
            try
            {

                Session databaseSession = CreateDatabaseSession();

                string databaseType = GeneralSettings.Settings["databaseType"];

                switch (databaseType)
                {
                    case "SQLite":
                        return SqliteExists();
                    case "MySql":
                        return MysqlExists(databaseSession);
                    case "MSSqlServer":
                    default:
                        return MsqlServerExists(databaseSession);
                }
            }
            catch
            {
                return false;
            }
        }

        private static bool MsqlServerExists(Session xpoSession)
        {
            string sql = string.Format("SELECT name FROM sys.databases WHERE name = '{0}' AND name NOT IN ('master', 'tempdb', 'model', 'msdb');", DatabaseSettings.DatabaseName);
            object resultCmd = xpoSession.ExecuteScalar(sql);

            if (resultCmd != null)
            {
                return (resultCmd.ToString() == DatabaseSettings.DatabaseName);
            }

            return false;
        }

        private static bool MysqlExists(Session xpoSession)
        {
            string sql = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{DatabaseSettings.DatabaseName}';";
            object resultCmd = xpoSession.ExecuteScalar(sql);

            if (resultCmd != null)
            {
                return resultCmd.ToString() == DatabaseSettings.DatabaseName;
            }

            return false;
        }

        private static bool SqliteExists()
        {
            string filename = $"{DatabaseSettings.DatabaseName}.db";
            return File.Exists(filename) && (new FileInfo(filename)).Length > 0;
        }

        public static Session CreateDatabaseSession()
        {
            string databaseName = GeneralSettings.Settings["databaseName"];

            DatabaseSettings.DatabaseName = databaseName;

            if (string.IsNullOrEmpty(databaseName))
            {
                DatabaseSettings.DatabaseName = "logicposdbdevelopment";
            }

            string connectionString = string.Format(GeneralSettings.Settings["xpoConnectionString"], DatabaseSettings.DatabaseName.ToLower());

            AutoCreateOption xpoAutoCreateOption = AutoCreateOption.None;

            XpoDefault.DataLayer = XpoDefault.GetDataLayer(connectionString, xpoAutoCreateOption);

            return  new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };
        }
    }
}
