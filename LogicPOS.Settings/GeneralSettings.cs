using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace LogicPOS.Settings
{
    public static class GeneralSettings
    {
        public static string AppTheme = "Default";
        public static Hashtable Path { get; set; }
        public static NameValueCollection Settings { get; set; }

        public static Dictionary<string, string> PreferenceParameters { get; set; }
    
    }
}
