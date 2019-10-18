using DansCSharpLibrary.JsonSerialization;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.resources.Resources.Localization;
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
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members
        private String _file;
        private Formatting _jsonIndented;

        //Public Properties
        private DateTime _sessionDateStart;
        public DateTime SessionDateStart
        {
            get { return _sessionDateStart; }
            set { _sessionDateStart = value; }
        }

        private DateTime _sessionUpdatedAt;
        public DateTime SessionUpdatedAt
        {
            get { return _sessionUpdatedAt; }
            set { _sessionUpdatedAt = value; }
        }

        private Dictionary<Guid, DateTime> _loggedUsers;
        public Dictionary<Guid, DateTime> LoggedUsers
        {
            get { return _loggedUsers; }
            set { _loggedUsers = value; }
        }

        private Guid _currentOrderMainOid;
        public Guid CurrentOrderMainOid
        {
            get { return _currentOrderMainOid; }
            set { _currentOrderMainOid = value; }
        }

        private Dictionary<Guid, OrderMain> _ordersMain;
        public Dictionary<Guid, OrderMain> OrdersMain
        {
            get { return _ordersMain; }
            set { _ordersMain = value; }
        }

        //Use Tokens ex WINDOWNAME_VALUE | POSARTICLESTOCKDIALOG_SUPPLIER with SetToken and GetToken
        private Dictionary<string, object> _tokens;
        public Dictionary<string, object> Tokens
        {
            get { return _tokens; }
            set { _tokens = value; }
        }

        public GlobalFrameworkSession() { }
        public GlobalFrameworkSession(String pFile)
        {
            //Init Parameters
            _file = pFile;
            //Default
            _currentOrderMainOid = Guid.Empty;
            _sessionDateStart = FrameworkUtils.CurrentDateTimeAtomic();
            _loggedUsers = new Dictionary<Guid, DateTime>();
            _ordersMain = new Dictionary<Guid, OrderMain>();
        }

        public bool Write()
        {
            if (GlobalFramework.SessionApp != null && (GlobalFramework.SessionApp.OrdersMain.Count == 0 && GlobalFramework.SessionApp.LoggedUsers.Count == 0))
            {
                //Empty Session May Delete
                try
                {
                    if (File.Exists(_file)) File.Delete(_file);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    return false;
                }
            }
            else
            {
                //Write Session
                try
                {
                    _sessionUpdatedAt = FrameworkUtils.CurrentDateTimeAtomic();
                    JsonSerialization.WriteToJsonFile<GlobalFrameworkSession>(_file, this, false, _jsonIndented);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
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
                foreach (Guid item in GlobalFramework.SessionApp.LoggedUsers.Keys)
                {
                    sys_userdetail user = (sys_userdetail)FrameworkUtils.GetXPGuidObject(typeof(sys_userdetail), item);
                    FrameworkUtils.Audit("USER_LOGOUT", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_used_forced_logout"), user.Name));
                }
                GlobalFramework.SessionApp.LoggedUsers.Clear();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public void DeleteEmptyTickets()
        {
            Guid latestNonEmptyOrder = Guid.Empty;

            if (_ordersMain != null && _ordersMain.Count > 0)
            {
                //Used to store List of Items to Remove after foreach, we cant remove it inside a foreach
                List<Guid> removeItems = new List<Guid>();
                foreach (Guid orderIndex in _ordersMain.Keys)
                {
                    //Remove if Not Open and if Dont have Lines
                    if (_ordersMain[orderIndex].OrderStatus != OrderStatus.Open && _ordersMain[orderIndex].OrderTickets[_ordersMain[orderIndex].CurrentTicketId].OrderDetails.Lines.Count == 0)
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
                    _ordersMain.Remove(item);
                }
            }
            if (!GlobalFramework.SessionApp.OrdersMain.ContainsKey(GlobalFramework.SessionApp._currentOrderMainOid))
                GlobalFramework.SessionApp._currentOrderMainOid = latestNonEmptyOrder;
        }

        public Boolean DeleteSession()
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            Boolean result = false;

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
                if (_tokens == null) _tokens = new Dictionary<string, object>();
                //Update Token
                if (_tokens.ContainsKey(pToken))
                {
                    _tokens[pToken] = pValue;
                }
                //Add Token
                else
                {
                    _tokens.Add(pToken, pValue);
                }

                result = true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public object GetToken(string pToken)
        {
            object result = null;

            try
            {
                if (_tokens != null && _tokens.Count > 0 && _tokens.ContainsKey(pToken)) result = _tokens[pToken];
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Helper Functions

        public static GlobalFrameworkSession InitSession(String pFile)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            GlobalFrameworkSession appSession;
            Formatting jsonIndented;
            bool _appSessionFileJsonIndented = Convert.ToBoolean(SettingsApp.AppSessionFileJsonIndented);
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
