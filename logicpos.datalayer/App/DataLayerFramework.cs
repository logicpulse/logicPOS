using logicpos.datalayer.DataLayer.Xpo;
using System.Collections;

namespace logicpos.datalayer.App
{
    public static class DataLayerFramework
    {
        public static sys_userdetail LoggedUser { get; set; }
        public static pos_configurationplaceterminal LoggedTerminal { get; set; }
        public static Hashtable Path { get; set; }
    }
}
