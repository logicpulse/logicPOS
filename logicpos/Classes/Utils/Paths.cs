using logicpos.datalayer.App;
using logicpos.shared.App;
using System;
using System.Collections;

namespace logicpos.Classes.Utils
{
    internal static class Paths
    {
        public static void InitializePaths()
        {
            DataLayerFramework.Path = new Hashtable
            {
                { "assets", SharedUtils.OSSlash(DataLayerFramework.Settings["pathAssets"]) },
                { "images", SharedUtils.OSSlash(DataLayerFramework.Settings["pathImages"]) },
                { "keyboards", SharedUtils.OSSlash(DataLayerFramework.Settings["pathKeyboards"]) },
                { "themes", SharedUtils.OSSlash(DataLayerFramework.Settings["pathThemes"]) },
                { "sounds", SharedUtils.OSSlash(DataLayerFramework.Settings["pathSounds"]) },
                { "resources", SharedUtils.OSSlash(DataLayerFramework.Settings["pathResources"]) },
                { "reports", SharedUtils.OSSlash(DataLayerFramework.Settings["pathReports"]) },
                { "temp", SharedUtils.OSSlash(DataLayerFramework.Settings["pathTemp"]) },
                { "cache", SharedUtils.OSSlash(DataLayerFramework.Settings["pathCache"]) },
                { "plugins", SharedUtils.OSSlash(DataLayerFramework.Settings["pathPlugins"]) },
                { "documents", SharedUtils.OSSlash(DataLayerFramework.Settings["pathDocuments"]) },
                { "certificates", SharedUtils.OSSlash(DataLayerFramework.Settings["pathCertificates"]) }
            };
   
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["temp"])));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["cache"])));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["documents"])));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(string.Format(@"{0}Database\Other", Convert.ToString(DataLayerFramework.Path["resources"]))));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(string.Format(@"{0}Database\{1}\Other", Convert.ToString(DataLayerFramework.Path["resources"]), DataLayerFramework.Settings["databaseType"], @"Database\MSSqlServer")));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(string.Format(@"{0}Database\{1}\Other", Convert.ToString(DataLayerFramework.Path["resources"]), DataLayerFramework.Settings["databaseType"], @"Database\SQLite")));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(string.Format(@"{0}Database\{1}\Other", Convert.ToString(DataLayerFramework.Path["resources"]), DataLayerFramework.Settings["databaseType"], @"Database\MySql")));
        }

        public static void InitializePathsPrefs()
        {
            // PreferencesValues
            // Require to add end Slash, Prefs DirChooser dont add extra Slash in the End
            DataLayerFramework.Path.Add("backups", SharedUtils.OSSlash(SharedFramework.PreferenceParameters["PATH_BACKUPS"] + '/'));
            DataLayerFramework.Path.Add("saftpt", SharedUtils.OSSlash(SharedFramework.PreferenceParameters["PATH_SAFTPT"] + '/'));
            //Create Directories
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["backups"])));
            SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["saftpt"])));
        }

    }
}
