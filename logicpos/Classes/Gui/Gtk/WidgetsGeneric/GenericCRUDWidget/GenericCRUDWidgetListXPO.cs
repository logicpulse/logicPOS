using DevExpress.Xpo;
using Gtk;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.App;
using logicpos.resources.Resources.Localization;
using System;
using System.Diagnostics;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    class GenericCRUDWidgetListXPO : GenericCRUDWidgetList<XPGuidObject>
    {
        private Session _session;

        //Constructor
        public GenericCRUDWidgetListXPO(Session pSession)
        {
            _session = pSession;
        }

        ////Getting values inside string
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
        public override bool Save()
        {
            bool debug = (Debugger.IsAttached) ? true : false;
            bool result = true;

            try
            {
                //BeginTransaction
                if (debug) _log.Debug("UpdateRecord(): BeginTransaction");
                _session.BeginTransaction();

                foreach (var item in _modifiedDataSourceRowObjects)
                {
                    if (debug) _log.Debug(string.Format("UpdateRecord(): Saving Modified XPGuidObjects item.Key:[{0}]", item.Key));
                    //// Required to Encrypt Properties Before Save, Required for New Records Problem, it works with Update Too
                    //item.Key.EncryptProperties();
                    // Now we can Trigger Save on XPGuidObject
                    item.Key.Save();

                    // Catch cfg_configurationpreferenceparameter : Usefull to Debug some Types
                    //if (item.Key.GetType().Equals(typeof(cfg_configurationpreferenceparameter)))
                    //{
                    //    _log.Debug("GenericCRUDWidgetListXPO: Catched#1");
                    //}
                }

                //CommitTransaction 
                if (debug) _log.Debug("UpdateRecord(): CommitTransaction");
                _session.CommitTransaction();
                result = true;
            }
            catch (Exception ex)
            {
                //IN009220 Inserir novo artigo - Código repetido - Mensagem de erro não existe
                if (ex.InnerException.HResult == -2146232060)
                {
                    string data = getBetween(ex.InnerException.Message, "(", ")");
                    String message = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_error_duplicated_key"), data);
                    Utils.ShowMessageTouch(GlobalApp.WindowBackOffice, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_exception_error"), message);
                }
                //END IN009220
                else
                {
                    _log.Error(ex.Message, ex);
                    ResponseType response = Utils.ShowMessageTouch(
                        GlobalApp.WindowBackOffice,
                        DialogFlags.DestroyWithParent | DialogFlags.Modal,
                        MessageType.Warning, ButtonsType.Close,
                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_exception_error"),
                        (ex.InnerException.Message != null) ? ex.InnerException.Message : ex.Message
                        );
                }                
                //RollbackTransaction
                //_log.Debug("UpdateRecord(): RollbackTransaction");
                try
                {
                    _session.RollbackTransaction();
                }
                catch (Exception ex2)
                {
                    _log.Error(ex.Message, ex2);
                }

                foreach (var item in _modifiedDataSourceRowObjects)
                {
                    if (debug) _log.Debug(string.Format("UpdateRecord(): Reloading Modified XPGuidObjects item.Key:[{0}]", item.Key));
                    item.Key.Reload();
                }

                result = false;
            }

            return result;
        }
    }
}
