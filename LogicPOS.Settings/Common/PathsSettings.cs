using System;
using System.Collections;
using System.IO;

namespace LogicPOS.Settings
{
    public static class PathsSettings
    {
        public static Hashtable Paths { get; set; }

        public static string TempFolderLocation => Paths["temp"].ToString();
        public static string ImagesFolderLocation => Paths["images"].ToString();
        public static string BackupsFolderLocation => Paths["backups"].ToString();
        public static string ResourcesFolderLocation => Paths["resources"].ToString();

        public static void InitializePaths()
        {
            Paths = new Hashtable
            {
                { "assets", AppSettings.Instance.pathAssets },
                { "images", AppSettings.Instance.pathImages },
                { "keyboards", AppSettings.Instance.pathKeyboards },
                { "themes", AppSettings.Instance.pathThemes },
                { "sounds", AppSettings.Instance.pathSounds },
                { "resources", AppSettings.Instance.pathResources },
                { "reports", AppSettings.Instance.pathReports },
                { "temp", AppSettings.Instance.pathTemp },
                { "cache", AppSettings.Instance.pathCache },
                { "plugins", AppSettings.Instance.pathPlugins },
                { "documents", AppSettings.Instance.pathDocuments },
                { "certificates", AppSettings.Instance.pathCertificates }

            };

            Directory.CreateDirectory(Convert.ToString(Paths["temp"]));
            Directory.CreateDirectory(Convert.ToString(Paths["cache"]));
            Directory.CreateDirectory(Convert.ToString(Paths["documents"]));
        }

        public static void InitializePreferencesPaths()
        {
            Paths.Add("backups", GeneralSettings.PreferenceParameters["PATH_BACKUPS"] + '/');
            Paths.Add("saftpt", GeneralSettings.PreferenceParameters["PATH_SAFTPT"] + '/');

            Directory.CreateDirectory(Convert.ToString(BackupsFolderLocation));
            Directory.CreateDirectory(Convert.ToString(Paths["saftpt"]));
        }
    }
}
