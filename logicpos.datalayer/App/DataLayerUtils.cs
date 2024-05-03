using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.datalayer.Xpo;
using System;

namespace logicpos.datalayer.App
{
    public static class DataLayerUtils
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static AppOperationMode GetAppMode()
        {
            AppOperationMode result = AppOperationMode.Default;
            string appOperationModeToken = LogicPOS.Settings.GeneralSettings.Settings["appOperationModeToken"];

            try
            {
                if (!string.IsNullOrEmpty(appOperationModeToken))
                {
                    /* IN008024 */
                    CustomAppOperationMode customAppOperationMode = CustomAppOperationMode.GetAppOperationMode(appOperationModeToken);
                    result = (AppOperationMode)Enum.Parse(typeof(AppOperationMode), customAppOperationMode.AppOperationTheme); //TO DO
                    //result = (AppOperationMode)Enum.Parse(typeof(AppOperationMode), appOperationModeToken);
                    _logger.Debug("AppOperationMode GetAppMode() :: '" + appOperationModeToken + "' with '" + result + "' AppOperationMode");
                }
            }
            catch (Exception ex)
            {
                /* IN009036 */
                _logger.Error(string.Format("AppOperationMode GetAppMode() :: [{0}]: {1}", appOperationModeToken, ex.Message));
            }

            return result;
        }

        /// <summary>
        /// Based on "appOperationModeToken" key it returns the proper CustomAppOperationMode for app operation mode.
        /// Please see #IN008024# for further details.
        /// </summary>
        /// <returns></returns>
        public static CustomAppOperationMode GetCustomAppOperationMode()
        {
            return CustomAppOperationMode.GetAppOperationMode(LogicPOS.Settings.GeneralSettings.Settings["appOperationModeToken"]);
        }

        /// <summary>
        /// Checks for default theme. This is used for POS components to be created accordingly.
        /// </summary>
        /// <returns></returns>
        public static bool IsDefaultAppOperationTheme()
        {
            bool isDefaultAppOperationTheme = false;

            if (DataLayerSettings.CustomAppOperationMode != null)
            {
                isDefaultAppOperationTheme = CustomAppOperationMode.DEFAULT.AppOperationTheme.Equals(DataLayerSettings.CustomAppOperationMode.AppOperationTheme);
            }

            return isDefaultAppOperationTheme;
        }

        /// <summary>
        /// Used to get the next Integer from a Table Field, used to get Next Ord, Code or other Int next Value
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="Field"></param>
        /// <returns></returns>
        public static uint GetNextTableFieldID(string pTable, string pField, bool pEndsWithZero = true)
        {
            uint result = 0;
            try
            {
                string sql = string.Format("SELECT MAX({0}) FROM {1};", pField, pTable);

                var resultInt = XPOSettings.Session.ExecuteScalar(sql);
                if (resultInt != null)
                {
                    result = Convert.ToUInt32(resultInt) + 1;

                    while (!("" + result).EndsWith("0"))
                    {
                        result++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            if (result <= 0)
            {
                result = 10;
            }
            return result;
        }

        public static XPGuidObject GetXPGuidObject(Type pXPGuidObjectType, Guid pOid)
        {
            return GetXPGuidObject(XPOSettings.Session, pXPGuidObjectType, pOid);
        }

        public static XPGuidObject GetXPGuidObject(Session pSession, Type pXPGuidObjectType, Guid pOid)
        {
            if (pOid == new Guid())
            {
                _logger.Error(string.Format("GetXPGuidObject[{0}]: Invalid Guid: [{1}]", pXPGuidObjectType, pOid));
            }

            try
            {
                XPClassInfo classInfo = pSession.GetClassInfo(pXPGuidObjectType);
                dynamic resultObject = pSession.GetObjectByKey(classInfo, pOid);
                XPGuidObject result = (XPGuidObject)resultObject;
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DateTime

        public static DateTime CurrentDateTimeAtomic()
        {
            string sql = string.Empty;
            var result = new DateTime();

            switch (DataLayerFramework.DatabaseType)
            {
                case DatabaseType.SQLite:
                case DatabaseType.MonoLite:
                    result = DateTime.Now;
                    break;
                case DatabaseType.MSSqlServer:
                    sql = "SELECT getdate() AS Now;";
                    break;
                case DatabaseType.MySql:
                    sql = "SELECT now() AS Now;";
                    break;
                default:
                    break;
            }

            if (sql != string.Empty)
            {
                try
                {
                    result = (DateTime)XPOSettings.Session.ExecuteScalar(sql);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            };

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helpers to Convert Guid to Unique Codes

        //The string "encoded" will be a GUID, represented as a base 64 string, using 24 characters.
        //SELECT SUBSTRING(Oid,1,30) AS CodeInternal FROM article;
        public static string GuidToStringId(string pGuidString)
        {
            //Using SubString
            return pGuidString.Substring(0, 30);//last 12 chars - 24, 12
        }
    }
}
