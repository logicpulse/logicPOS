using LogicPOS.Settings.Enums;
using System;

namespace LogicPOS.Settings
{
    public  static class AppOperationModeSettings
    {
        public static AppOperationMode AppMode { get; set; } = GetAppMode();

        public static CustomAppOperationMode CustomAppOperationMode = GetCustomAppOperationMode();

        public static bool IsDefaultTheme = IsDefaultAppOperationTheme();

        public static AppOperationMode GetAppMode()
        {
            AppOperationMode mode = AppOperationMode.Default;
            string appOperationModeToken = GeneralSettings.Settings["appOperationModeToken"];


            if (!string.IsNullOrEmpty(appOperationModeToken))
            {
                CustomAppOperationMode customAppOperationMode = CustomAppOperationMode.GetAppOperationMode(appOperationModeToken);
                mode = (AppOperationMode)Enum.Parse(typeof(AppOperationMode), customAppOperationMode.AppOperationTheme);
            }

            return mode;
        }

        public static CustomAppOperationMode GetCustomAppOperationMode()
        {
            return CustomAppOperationMode.GetAppOperationMode(GeneralSettings.Settings["appOperationModeToken"]);
        }

        public static bool IsDefaultAppOperationTheme()
        {
            bool isDefaultAppOperationTheme = false;

            if (CustomAppOperationMode != null)
            {
                isDefaultAppOperationTheme = CustomAppOperationMode.DEFAULT.AppOperationTheme.Equals(
                    CustomAppOperationMode.AppOperationTheme);
            }

            return isDefaultAppOperationTheme;
        }
    }
}
