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
                { "assets", DataLayerFramework.Settings["pathAssets"] },
                { "images", DataLayerFramework.Settings["pathImages"] },
                { "keyboards", DataLayerFramework.Settings["pathKeyboards"] },
                { "themes", DataLayerFramework.Settings["pathThemes"] },
                { "sounds", DataLayerFramework.Settings["pathSounds"] },
                { "resources", DataLayerFramework.Settings["pathResources"] },
                { "reports", DataLayerFramework.Settings["pathReports"] },
                { "temp", DataLayerFramework.Settings["pathTemp"] },
                { "cache", DataLayerFramework.Settings["pathCache"] },
                { "plugins", DataLayerFramework.Settings["pathPlugins"] },
                { "documents", DataLayerFramework.Settings["pathDocuments"] },
                { "certificates", DataLayerFramework.Settings["pathCertificates"] }
            };
   
            SharedUtils.CreateDirectory(Convert.ToString(DataLayerFramework.Path["temp"]));
            SharedUtils.CreateDirectory(Convert.ToString(DataLayerFramework.Path["cache"]));
            SharedUtils.CreateDirectory(Convert.ToString(DataLayerFramework.Path["documents"]));
            SharedUtils.CreateDirectory(string.Format(@"{0}Database\Other", Convert.ToString(DataLayerFramework.Path["resources"])));
            SharedUtils.CreateDirectory(string.Format(@"{0}Database\{1}\Other", Convert.ToString(DataLayerFramework.Path["resources"]), DataLayerFramework.Settings["databaseType"], @"Database\MSSqlServer"));
            SharedUtils.CreateDirectory(string.Format(@"{0}Database\{1}\Other", Convert.ToString(DataLayerFramework.Path["resources"]), DataLayerFramework.Settings["databaseType"], @"Database\SQLite"));
            SharedUtils.CreateDirectory(string.Format(@"{0}Database\{1}\Other", Convert.ToString(DataLayerFramework.Path["resources"]), DataLayerFramework.Settings["databaseType"], @"Database\MySql"));
        }

        public static void InitializePathsPrefs()
        {
            // PreferencesValues
            // Require to add end Slash, Prefs DirChooser dont add extra Slash in the End
            DataLayerFramework.Path.Add("backups", SharedFramework.PreferenceParameters["PATH_BACKUPS"] + '/');
            DataLayerFramework.Path.Add("saftpt", SharedFramework.PreferenceParameters["PATH_SAFTPT"] + '/');
            //Create Directories
            SharedUtils.CreateDirectory(Convert.ToString(DataLayerFramework.Path["backups"]));
            SharedUtils.CreateDirectory(Convert.ToString(DataLayerFramework.Path["saftpt"]));
        }

    }
}
