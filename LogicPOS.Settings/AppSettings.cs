using System.Collections;
using System.Collections.Specialized;

namespace LogicPOS.Settings
{
    public class AppSettings
    {
        public static string AppTheme = "Default";
        public static Hashtable Path { get; set; }
        public static NameValueCollection Settings { get; set; }
    }
}
