using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using System.Collections;
using System.Collections.Specialized;

namespace logicpos.datalayer.App
{
    public class GlobalFramework
    {
        // Xpo
        public static Session SessionXpo;
        // Database
        public static DatabaseType DatabaseType;
        // User/Terminal/Permissions
        public static sys_userdetail LoggedUser;
        public static pos_configurationplaceterminal LoggedTerminal;
        // System Paths
        public static Hashtable Path;
        // Settings
        public static NameValueCollection Settings;
    }
}
