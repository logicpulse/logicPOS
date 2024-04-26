using logicpos.datalayer.App;
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
            string appOperationModeToken = DataLayerFramework.Settings["appOperationModeToken"];


            if (!string.IsNullOrEmpty(appOperationModeToken))
            {
                CustomAppOperationMode customAppOperationMode = CustomAppOperationMode.GetAppOperationMode(appOperationModeToken);
                mode = (AppOperationMode)Enum.Parse(typeof(AppOperationMode), customAppOperationMode.AppOperationTheme);
            }

            return mode;
        }

        public static CustomAppOperationMode GetCustomAppOperationMode()
        {
            return CustomAppOperationMode.GetAppOperationMode(DataLayerFramework.Settings["appOperationModeToken"]);
        }

        public static bool IsDefaultAppOperationTheme()
        {
            bool isDefaultAppOperationTheme = false;

            if (AppOperationModeSettings.CustomAppOperationMode != null)
            {
                isDefaultAppOperationTheme = CustomAppOperationMode.DEFAULT.AppOperationTheme.Equals(AppOperationModeSettings.CustomAppOperationMode.AppOperationTheme);
            }

            return isDefaultAppOperationTheme;
        }
    }
}
