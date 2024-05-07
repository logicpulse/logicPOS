using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.shared.App;
using logicpos.shared.Classes.Orders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using logicpos.datalayer.Xpo;

namespace logicpos.shared
{
    public class GlobalFrameworkSession
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _jsonFile;
        private Formatting _jsonFormatting;

        public DateTime SessionStartDate { get; set; }

        public DateTime SessionUpdatedAt { get; set; }

        public Dictionary<Guid, DateTime> LoggedUsers { get; set; }

        public Guid CurrentOrderMainOid { get; set; }
       
        public Dictionary<Guid, OrderMain> OrdersMain { get; set; }

        public Dictionary<string, object> Tokens { get; set; }

        public GlobalFrameworkSession(string pFile)
        {
            //Init Parameters
            _jsonFile = pFile;
            //Default
            CurrentOrderMainOid = Guid.Empty;
            SessionStartDate = XPOHelper.CurrentDateTimeAtomic();
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
                    if (File.Exists(_jsonFile)) File.Delete(_jsonFile);
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
                    SessionUpdatedAt = XPOHelper.CurrentDateTimeAtomic();
                    var jsonSerializer = new JsonSerializer();
                    var contentsToWriteToFile = JsonConvert.SerializeObject(this, _jsonFormatting);
                    var writer = new StreamWriter(_jsonFile,false);
                    writer.Write(contentsToWriteToFile);
                    writer.Close();
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
                    sys_userdetail user = (sys_userdetail)XPOHelper.GetXPGuidObject(typeof(sys_userdetail), item);
                    SharedUtils.Audit("USER_loggerOUT", string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "audit_message_used_forced_loggerout"), user.Name));
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
            if (!SharedFramework.SessionApp.OrdersMain.ContainsKey(SharedFramework.SessionApp.CurrentOrderMainOid))
                SharedFramework.SessionApp.CurrentOrderMainOid = latestNonEmptyOrder;
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

        public static GlobalFrameworkSession InitSession(string jsonFile)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            GlobalFrameworkSession appSession;
            Formatting jsonIndented;
            bool _appSessionFileJsonIndented = Convert.ToBoolean(SharedSettings.AppSessionFileJsonIndented);
            if (_appSessionFileJsonIndented) { jsonIndented = Formatting.Indented; } else { jsonIndented = Formatting.None; };

            if (File.Exists(jsonFile))
            {
                try
                {
                    string jsonfileContents = File.ReadAllText(jsonFile);
                    appSession = JsonConvert.DeserializeObject<GlobalFrameworkSession>(jsonFile);
                    appSession._jsonFormatting = jsonIndented;
                    appSession._jsonFile = jsonFile;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("InitSession(): {0}", ex.Message), ex);
                    appSession = new GlobalFrameworkSession(jsonFile);
                    appSession._jsonFormatting = jsonIndented;
                    appSession.Write();
                };
            }
            else
            {
                appSession = new GlobalFrameworkSession(jsonFile);
                appSession._jsonFormatting = jsonIndented;
                appSession.Write();
            };

            return appSession;
        }
    }
}
