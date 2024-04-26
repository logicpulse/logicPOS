using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using System.Collections;
using System.Collections.Specialized;

namespace logicpos.datalayer.App
{
    public static class DataLayerFramework
    {
        public static DatabaseType DatabaseType { get; set; }
        public static sys_userdetail LoggedUser { get; set; }
        public static pos_configurationplaceterminal LoggedTerminal { get; set; }
        public static Hashtable Path { get; set; }
        public static NameValueCollection Settings { get; set; }
    }
}
