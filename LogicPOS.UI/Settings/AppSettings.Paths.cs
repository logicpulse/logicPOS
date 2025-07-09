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
            public static string Sounds => Instance.PathSounds;
            public static string Resources => Instance.PathResources;
            public static string Cache => Instance.PathCache;
            public static string Plugins => Instance.PathPlugins;
            public static string Certificates => Instance.PathCertificates;
            public static string Assets => Instance.PathAssets;
        }
    }
}
