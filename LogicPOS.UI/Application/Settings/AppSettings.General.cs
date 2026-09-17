using LogicPOS.UI.Application.Enums;

namespace LogicPOS.UI.Settings
{
    public partial class AppSettings
    {
        public static System.Drawing.Size MaxWindowSize { get; set; }
        public static string AppName { get; set; } = "LogicPos";
        public static string AppIcon { get; set; } = "application.ico";
        public AppOperationMode OperationMode => AppOperationModeExtensions.FromString(AppOperationModeToken);
        public string AppOperationModeTheme => OperationMode.GetTheme().ToLower();
        public string ThemeFilePath => $"{Paths.Themes}theme_default_{AppOperationModeTheme.ToLower()}.xml";
    }
}
