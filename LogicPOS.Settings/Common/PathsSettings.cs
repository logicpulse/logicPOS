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
                { "assets", GeneralSettings.Settings["pathAssets"] },
                { "images", GeneralSettings.Settings["pathImages"] },
                { "keyboards", GeneralSettings.Settings["pathKeyboards"] },
                { "themes", GeneralSettings.Settings["pathThemes"] },
                { "sounds", GeneralSettings.Settings["pathSounds"] },
                { "resources", GeneralSettings.Settings["pathResources"] },
                { "reports", GeneralSettings.Settings["pathReports"] },
                { "temp", GeneralSettings.Settings["pathTemp"] },
                { "cache", GeneralSettings.Settings["pathCache"] },
                { "plugins", GeneralSettings.Settings["pathPlugins"] },
                { "documents", GeneralSettings.Settings["pathDocuments"] },
                { "certificates", GeneralSettings.Settings["pathCertificates"] }
            };

            Directory.CreateDirectory(Convert.ToString(Paths["temp"]));
            Directory.CreateDirectory(Convert.ToString(Paths["cache"]));
            Directory.CreateDirectory(Convert.ToString(Paths["documents"]));
            Directory.CreateDirectory($@"{ResourcesFolderLocation}Database\Other");
            Directory.CreateDirectory(string.Format(@"{0}Database\{1}\Other", ResourcesFolderLocation, GeneralSettings.Settings["databaseType"], @"Database\MSSqlServer"));
            Directory.CreateDirectory(string.Format(@"{0}Database\{1}\Other", ResourcesFolderLocation, GeneralSettings.Settings["databaseType"], @"Database\SQLite"));
            Directory.CreateDirectory(string.Format(@"{0}Database\{1}\Other", ResourcesFolderLocation, GeneralSettings.Settings["databaseType"], @"Database\MySql"));
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
