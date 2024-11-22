using LogicPOS.Settings;

namespace LogicPOS.Data.XPO.Settings
{
    public static partial class TerminalSettings
    {
        static int result;
        public static class ThermalPrinter
        {
             
            public static int MaxCharsPerLineNormal
            {
                get
                {
                    if (HasLoggedTerminal != false)
                    {
                        return PrintingSettings.ThermalPrinter.MaxCharsPerLineNormal;
                    }

                     //result=LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineNormal; LUCIANO
                    
                    return (result > 0) ? result : PrintingSettings.ThermalPrinter.MaxCharsPerLineNormal;
                }
            }

            public static int MaxCharsPerLineNormalBold
            {
                get
                {
                    if (HasLoggedTerminal != false)
                    {
                        return PrintingSettings.ThermalPrinter.MaxCharsPerLineNormalBold;
                    }

                    // result = LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineNormalBold;
                    return result > 0 ? result : PrintingSettings.ThermalPrinter.MaxCharsPerLineNormalBold;
                }
            }

            public static int MaxCharsPerLineSmall
            {
                get
                {
                    if (HasLoggedTerminal != false)
                    {
                        return PrintingSettings.ThermalPrinter.MaxCharsPerLineSmall;
                    }

                    //result = LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineSmall;
                    return result > 0 ? result : PrintingSettings.ThermalPrinter.MaxCharsPerLineSmall;
                }
            }
        }
    }
}
