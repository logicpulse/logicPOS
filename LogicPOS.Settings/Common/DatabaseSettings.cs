using LogicPOS.Settings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.Settings
{
    public class DatabaseSettings
    {
        public static DatabaseType DatabaseType { get; set; }
        public static string DatabaseServer { get; set; }
        public static string DatabaseName { get; set; }
        public static string DatabaseUser { get; set; }
        public static string DatabasePassword { get; set; }

        public static void AssignConnectionStringToSettings(string connectionString)
        {
            Dictionary<string, string> connectionStringToDictionary = GetDictionaryFromConnectionString(connectionString);

            switch (DatabaseType)
            {
                case DatabaseType.SQLite:
                case DatabaseType.MonoLite:
                    break;
                case DatabaseType.MSSqlServer:
                    DatabaseServer = connectionStringToDictionary["Data Source"];
                    DatabaseUser = connectionStringToDictionary["User ID"];
                    DatabasePassword = connectionStringToDictionary["Password"];
                    break;
                case DatabaseType.MySql:
                    DatabaseServer = connectionStringToDictionary["server"];
                    DatabaseUser = connectionStringToDictionary["user id"];
                    DatabasePassword = connectionStringToDictionary["password"];
                    break;
            }
        }

        private static Dictionary<string, string> GetDictionaryFromConnectionString(string connectionString)
        {
            Dictionary<string, string> connStringParts = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
              .Select(t => t.Split(new char[] { '=' }, 2))
              .ToDictionary(t => t[0].Trim(), t => t[1].Trim(), StringComparer.InvariantCultureIgnoreCase);

            return connStringParts;
        }
    }
}
