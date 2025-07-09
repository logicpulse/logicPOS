using System.Diagnostics;
using System.Reflection;

namespace LogicPOS.UI.Settings
{
    public partial class AppSettings
    {

        public static string AppTheme => "Default";
        public static string ServerVersion { get; set; }
        public static string ProductVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return $"v{fileVersionInfo.ProductVersion}";
            }
        }

        public static System.Drawing.Size MaxWindowSize { get; set; }
        public static string AppName { get; set; } = "LogicPos";
        public static string AppIcon { get; set; } = "application.ico";
        public static string AppUrl { get; set; } = "www.logicpulse.com";
        private static readonly string FileFormatThemeFile = "theme_{0}_{1}.xml";
        public static string ThemeFile => $"{AppSettings.Paths.Themes}{string.Format(FileFormatThemeFile, "default", "default")}";
    }
}
