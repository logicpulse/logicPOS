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
                { "assets", LogicPOS.Settings.GeneralSettings.Settings["pathAssets"] },
                { "images", LogicPOS.Settings.GeneralSettings.Settings["pathImages"] },
                { "keyboards", LogicPOS.Settings.GeneralSettings.Settings["pathKeyboards"] },
                { "themes", LogicPOS.Settings.GeneralSettings.Settings["pathThemes"] },
                { "sounds", LogicPOS.Settings.GeneralSettings.Settings["pathSounds"] },
                { "resources", LogicPOS.Settings.GeneralSettings.Settings["pathResources"] },
                { "reports", LogicPOS.Settings.GeneralSettings.Settings["pathReports"] },
                { "temp", LogicPOS.Settings.GeneralSettings.Settings["pathTemp"] },
                { "cache", LogicPOS.Settings.GeneralSettings.Settings["pathCache"] },
                { "plugins", LogicPOS.Settings.GeneralSettings.Settings["pathPlugins"] },
                { "documents", LogicPOS.Settings.GeneralSettings.Settings["pathDocuments"] },
                { "certificates", LogicPOS.Settings.GeneralSettings.Settings["pathCertificates"] }
            };
   
            SharedUtils.CreateDirectory(Convert.ToString(DataLayerFramework.Path["temp"]));
            SharedUtils.CreateDirectory(Convert.ToString(DataLayerFramework.Path["cache"]));
            SharedUtils.CreateDirectory(Convert.ToString(DataLayerFramework.Path["documents"]));
            SharedUtils.CreateDirectory(string.Format(@"{0}Database\Other", Convert.ToString(DataLayerFramework.Path["resources"])));
            SharedUtils.CreateDirectory(string.Format(@"{0}Database\{1}\Other", Convert.ToString(DataLayerFramework.Path["resources"]), LogicPOS.Settings.GeneralSettings.Settings["databaseType"], @"Database\MSSqlServer"));
            SharedUtils.CreateDirectory(string.Format(@"{0}Database\{1}\Other", Convert.ToString(DataLayerFramework.Path["resources"]), LogicPOS.Settings.GeneralSettings.Settings["databaseType"], @"Database\SQLite"));
            SharedUtils.CreateDirectory(string.Format(@"{0}Database\{1}\Other", Convert.ToString(DataLayerFramework.Path["resources"]), LogicPOS.Settings.GeneralSettings.Settings["databaseType"], @"Database\MySql"));
        }

        public static void InitializePathsPrefs()
        {
            // PreferencesValues
            // Require to add end Slash, Prefs DirChooser dont add extra Slash in the End
            DataLayerFramework.Path.Add("backups", LogicPOS.Settings.GeneralSettings.PreferenceParameters["PATH_BACKUPS"] + '/');
            DataLayerFramework.Path.Add("saftpt", LogicPOS.Settings.GeneralSettings.PreferenceParameters["PATH_SAFTPT"] + '/');
            //Create Directories
            SharedUtils.CreateDirectory(Convert.ToString(DataLayerFramework.Path["backups"]));
            SharedUtils.CreateDirectory(Convert.ToString(DataLayerFramework.Path["saftpt"]));
        }

    }
}
