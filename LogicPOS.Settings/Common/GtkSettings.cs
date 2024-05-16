using System;

namespace LogicPOS.Settings
{
    public static class GtkSettings
    {
        public static bool ShowMinimizeButton
        {
            get
            {
                if (string.IsNullOrEmpty(GeneralSettings.Settings["appShowMinimize"]))
                {
                    return false;
                }

                return Convert.ToBoolean(GeneralSettings.Settings["appShowMinimize"]);
            }
        }

        public static string DefaultMinimizeWindowIconLocation => PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_window_minimize.png";
    }
}
