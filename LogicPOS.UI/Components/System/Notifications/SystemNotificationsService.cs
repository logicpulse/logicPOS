using System.Collections.Generic;

namespace LogicPOS.UI.Services
{
    public class SystemNotificationsService
    {
        public static Dictionary<string, bool> Notifications { get; set; } = new Dictionary<string, bool>
        {
            ["SHOW_PRINTER_UNDEFINED"] = true
        };
    }
}
