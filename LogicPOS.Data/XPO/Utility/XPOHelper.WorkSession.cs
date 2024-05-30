using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.Settings;

namespace LogicPOS.Data.XPO.Utility
{
    public static partial class XPOHelper
    {
        public static class WorkSession
        {
            public static string GetWorkSessionMovementPrintingFileTemplate()
            {
                sys_configurationprinterstemplates template = GetEntityById<sys_configurationprinterstemplates>(PrintingSettings.WorkSessionMovementPrintingTemplateId);
                return template.FileTemplate;

            }
        }
    }
}
