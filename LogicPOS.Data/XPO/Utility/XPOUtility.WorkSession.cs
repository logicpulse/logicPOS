using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.DTOs.Printing;
using LogicPOS.Settings;

namespace LogicPOS.Data.XPO.Utility
{
    public static partial class XPOUtility
    {
        public static class WorkSession
        {
            public static string GetWorkSessionMovementPrintingFileTemplate()
            {
                sys_configurationprinterstemplates template = GetEntityById<sys_configurationprinterstemplates>(PrintingSettings.WorkSessionMovementPrintingTemplateId);
                return template.FileTemplate;

            }

            public static void SaveCurrentWorkSessionPeriodDayDto()
            {
                XPOSettings.LastWorkSessionPeriodDto = MappingUtils.GetPrintWorkSessionDto(XPOSettings.WorkSessionPeriodDay);
            }

            public static PrintWorkSessionDto GetCurrentWorkSessionPeriodDayDto()
            {
                if (XPOSettings.WorkSessionPeriodDay != null)
                {
                    return MappingUtils.GetPrintWorkSessionDto(XPOSettings.WorkSessionPeriodDay);
                }

                return XPOSettings.LastWorkSessionPeriodDto;
            }
        }
    }
}
