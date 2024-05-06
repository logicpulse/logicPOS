using LogicPOS.Settings.Enums;

namespace LogicPOS.Settings
{
    public class DatabaseSettings
    {
        public static DatabaseType DatabaseType { get; set; }
        public static string DatabaseServer { get; set; }
        public static string DatabaseName { get; set; }
        public static string DatabaseUser { get; set; }
        public static string DatabasePassword { get; set; }
    }
}
