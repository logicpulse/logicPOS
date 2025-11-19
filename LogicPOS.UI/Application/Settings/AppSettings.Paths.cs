using System.Collections;

namespace LogicPOS.UI.Settings
{
    public  partial class AppSettings
    {
        public static class Paths
        {
            public static string Temp => Instance.PathTemp;
            public static string Images => Instance.PathImages;
            public static string Themes => Instance.PathThemes;
            public static string Keyboards => Instance.PathKeyboards;

            public static string GetThemeFileLocation(string file)
            {
                return $@"{Themes}Default\{file}";
            }

        }
    }
}
