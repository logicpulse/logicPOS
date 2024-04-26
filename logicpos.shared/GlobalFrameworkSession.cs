using DansCSharpLibrary.JsonSerialization;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.shared.App;
using logicpos.shared.Classes.Orders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace logicpos.shared
{
    public class GlobalFrameworkSession
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members
        private string _file;
        private Formatting _jsonIndented;

        public DateTime SessionDateStart { get; set; }

        public DateTime SessionUpdatedAt { get; set; }

        public Dictionary<Guid, DateTime> LoggedUsers { get; set; }

        private Guid _currentOrderMainOid;
        public Guid CurrentOrderMainOid
        {
            get { return _currentOrderMainOid; }
            set { _currentOrderMainOid = value; }
        }

        public Dictionary<Guid, OrderMain> OrdersMain { get; set; }

        public Dictionary<string, object> Tokens { get; set; }

        public GlobalFrameworkSession() { }
        public GlobalFrameworkSession(string pFile)
        {
            //Init Parameters
            _file = pFile;
            //Default
            _currentOrderMainOid = Guid.Empty;
            SessionDateStart = datalayer.App.DataLayerUtils.CurrentDateTimeAtomic();
            LoggedUsers = new Dictionary<Guid, DateTime>();
            OrdersMain = new Dictionary<Guid, OrderMain>();
        }

        public bool Write()
        {
            if (SharedFramework.SessionApp != null && (SharedFramework.SessionApp.OrdersMain.Count == 0 && SharedFramework.SessionApp.LoggedUsers.Count == 0))
            {
                //Empty Session May Delete
                try
                {
                    if (File.Exists(_file)) File.Delete(_file);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    return false;
                }
            }
            else
            {
                //Write Session
                try
                {
                    SessionUpdatedAt = datalayer.App.DataLayerUtils.CurrentDateTimeAtomic();
                    JsonSerialization.WriteToJsonFile<GlobalFrameworkSession>(_file, this, false, _jsonIndented);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    return false;
                }
            }
        }

        public void CleanSession()
        {
            DeleteLoggedUsers();
            DeleteEmptyTickets();
        }

        public void DeleteLoggedUsers()
        {
            try
            {
                foreach (Guid item in SharedFramework.SessionApp.LoggedUsers.Keys)
                {
                    sys_userdetail user = (sys_userdetail)datalayer.App.DataLayerUtils.GetXPGuidObject(typeof(sys_userdetail), item);
                    SharedUtils.Audit("USER_loggerOUT", string.Format(resources.CustomResources.GetCustomResource(datalayer.App.DataLayerFramework.Settings["customCultureResourceDefinition"], "audit_message_used_forced_loggerout"), user.Name));
                }
                SharedFramework.SessionApp.LoggedUsers.Clear();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public void DeleteEmptyTickets()
        {
            Guid latestNonEmptyOrder = Guid.Empty;

            if (OrdersMain != null && OrdersMain.Count > 0)
            {
                //Used to store List of Items to Remove after foreach, we cant remove it inside a foreach
                List<Guid> removeItems = new List<Guid>();
                foreach (Guid orderIndex in OrdersMain.Keys)
                {
                    //Remove if Not Open and if Dont have Lines
                    if (OrdersMain[orderIndex].OrderStatus != OrderStatus.Open && OrdersMain[orderIndex].OrderTickets[OrdersMain[orderIndex].CurrentTicketId].OrderDetails.Lines.Count == 0)
                    {
                        removeItems.Add(orderIndex);
                    }
                    else
                    {
                        latestNonEmptyOrder = orderIndex;
                    }
                };
                foreach (var item in removeItems)
                {
                    OrdersMain.Remove(item);
                    break;
                }
            }
            if (!SharedFramework.SessionApp.OrdersMain.ContainsKey(SharedFramework.SessionApp._currentOrderMainOid))
                SharedFramework.SessionApp._currentOrderMainOid = latestNonEmptyOrder;
        }

        public bool DeleteSession()
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            bool result = false;

            if (File.Exists(_file))
            {
                File.Delete(_file);
                result = true;
            }

            return result;
        }

        public bool SetToken(string pToken, object pValue)
        {
            bool result = false;

            try
            {
                //Init Dictionary
                if (Tokens == null) Tokens = new Dictionary<string, object>();
                //Update Token
                if (Tokens.ContainsKey(pToken))
                {
                    Tokens[pToken] = pValue;
                }
                //Add Token
                else
                {
                    Tokens.Add(pToken, pValue);
                }

                result = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        public object GetToken(string pToken)
        {
            object result = null;

            try
            {
                if (Tokens != null && Tokens.Count > 0 && Tokens.ContainsKey(pToken)) result = Tokens[pToken];
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Helper Functions

        public static GlobalFrameworkSession InitSession(string pFile)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            GlobalFrameworkSession appSession;
            Formatting jsonIndented;
            bool _appSessionFileJsonIndented = Convert.ToBoolean(SharedSettings.AppSessionFileJsonIndented);
            if (_appSessionFileJsonIndented) { jsonIndented = Formatting.Indented; } else { jsonIndented = Formatting.None; };

            if (File.Exists(pFile))
            {
                try
                {
                    appSession = JsonSerialization.ReadFromJsonFile<GlobalFrameworkSession>(pFile);
                    appSession._jsonIndented = jsonIndented;
                    appSession._file = pFile;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("InitSession(): {0}", ex.Message), ex);
                    appSession = new GlobalFrameworkSession(pFile);
                    appSession._jsonIndented = jsonIndented;
                    appSession.Write();
                };
            }
            else
            {
                appSession = new GlobalFrameworkSession(pFile);
                appSession._jsonIndented = jsonIndented;
                appSession.Write();
            };

            return appSession;
        }
    }
}
