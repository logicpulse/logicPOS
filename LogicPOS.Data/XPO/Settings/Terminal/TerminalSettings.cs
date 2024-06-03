using LogicPOS.Domain.Entities;

namespace LogicPOS.Data.XPO.Settings
{
    public static partial class TerminalSettings
    {
        public static pos_configurationplaceterminal LoggedTerminal { get; set; }
        public static bool HasLoggedTerminal => LoggedTerminal != null;
        public static sys_configurationprinters DefaultPrinter => LoggedTerminal.Printer;
    }
}
