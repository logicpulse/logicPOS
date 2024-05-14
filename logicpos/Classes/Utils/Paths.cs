using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections;

namespace logicpos.Classes.Utils
{
    internal static class Paths
    {
        public static void InitializePaths()
        {
            PathsSettings.Paths = new Hashtable
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

            GeneralUtils.CreateDirectory(Convert.ToString(PathsSettings.Paths["temp"]));
            GeneralUtils.CreateDirectory(Convert.ToString(PathsSettings.Paths["cache"]));
            GeneralUtils.CreateDirectory(Convert.ToString(PathsSettings.Paths["documents"]));
            GeneralUtils.CreateDirectory(string.Format(@"{0}Database\Other", Convert.ToString(PathsSettings.Paths["resources"])));
            GeneralUtils.CreateDirectory(string.Format(@"{0}Database\{1}\Other", Convert.ToString(PathsSettings.Paths["resources"]), GeneralSettings.Settings["databaseType"], @"Database\MSSqlServer"));
            GeneralUtils.CreateDirectory(string.Format(@"{0}Database\{1}\Other", Convert.ToString(PathsSettings.Paths["resources"]), GeneralSettings.Settings["databaseType"], @"Database\SQLite"));
            GeneralUtils.CreateDirectory(string.Format(@"{0}Database\{1}\Other", Convert.ToString(PathsSettings.Paths["resources"]), GeneralSettings.Settings["databaseType"], @"Database\MySql"));
        }

        public static void InitializePathsPrefs()
        {
            // PreferencesValues
            // Require to add end Slash, Prefs DirChooser dont add extra Slash in the End
            PathsSettings.Paths.Add("backups", GeneralSettings.PreferenceParameters["PATH_BACKUPS"] + '/');
            PathsSettings.Paths.Add("saftpt", GeneralSettings.PreferenceParameters["PATH_SAFTPT"] + '/');
            //Create Directories
            GeneralUtils.CreateDirectory(Convert.ToString(PathsSettings.Paths["backups"]));
            GeneralUtils.CreateDirectory(Convert.ToString(PathsSettings.Paths["saftpt"]));
        }

    }
}
