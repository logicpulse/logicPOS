using LogicPOS.Settings.Enums;

namespace LogicPOS.Settings.Extensions
{
    public static class DatabaseTypeExtensions
    {
        public static bool IsSQLite(this DatabaseType databaseType)
        {
            return databaseType == DatabaseType.SQLite || databaseType == DatabaseType.MonoLite;
        }

        public static bool IsSqlServer(this DatabaseType databaseType)
        {
            return databaseType == DatabaseType.MSSqlServer;
        }

        public static bool IsMySql(this DatabaseType databaseType)
        {
            return databaseType == DatabaseType.MySql;
        }
    }
}
