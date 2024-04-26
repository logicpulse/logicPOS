using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.console.App;
using logicpos.shared.App;
using System;

namespace logicpos.financial.console.Objects
{
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    public class Utils
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Terminals

        //Duplicate method from logicpos.Utils

        public static pos_configurationplaceterminal GetTerminal()
        {
            pos_configurationplaceterminal configurationPlaceTerminal = null;

            //Debug Directive disabled by Mario, if enabled we cant force Hardware id in Release, if we want to ignore appHardwareId from config we just delete it
            //If assigned in Config use it, else does nothing and use default ####-####-####-####-####-####
            if (ConsoleSettings.AppHardwareId != null && ConsoleSettings.AppHardwareId != string.Empty)
            {
                SharedFramework.LicenseHardwareId = ConsoleSettings.AppHardwareId;
            }

            try
            {
                //Try TerminalID from Database
                configurationPlaceTerminal = (pos_configurationplaceterminal)SharedUtils.GetXPGuidObjectFromField(typeof(pos_configurationplaceterminal), "HardwareId", SharedFramework.LicenseHardwareId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            //Create a new db terminal
            if (configurationPlaceTerminal == null)
            {
                try
                {
                    //Persist Terminal in DB
                    configurationPlaceTerminal = new pos_configurationplaceterminal(XPOSettings.Session)
                    {
                        Ord = DataLayerUtils.GetNextTableFieldID("pos_configurationplaceterminal", "Ord"),
                        Code = DataLayerUtils.GetNextTableFieldID("pos_configurationplaceterminal", "Code"),
                        Designation = "Terminal #" + DataLayerUtils.GetNextTableFieldID("pos_configurationplaceterminal", "Code"),
                        HardwareId = SharedFramework.LicenseHardwareId
                        //Fqdn = GetFQDN()
                    };
                    configurationPlaceTerminal.Save();
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("Error! Cant Register a new TerminalId [{0}] with HardwareId: [{1}], Error: [2]", configurationPlaceTerminal.Oid, configurationPlaceTerminal.HardwareId, ex.Message), ex);
                    Environment.Exit(0);
                }
            }
            return configurationPlaceTerminal;
        }
    }
}
