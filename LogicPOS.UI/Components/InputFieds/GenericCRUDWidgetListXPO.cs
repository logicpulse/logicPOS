using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using System;
using System.Diagnostics;
using LogicPOS.Globalization;
using LogicPOS.Domain.Entities;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    internal class GenericCRUDWidgetListXPO : GenericCRUDWidgetList<Entity>
    {
        private readonly Session _session;

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
            bool debug = (Debugger.IsAttached);
            bool result;
            try
            {
                _session.BeginTransaction();

                foreach (var item in _modifiedDataSourceRowObjects)
                { 
                    item.Key.Save();
                }

                _session.CommitTransaction();

                result = true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.HResult == -2146232060)
                {
                    string data = getBetween(ex.InnerException.Message, "(", ")");
                    string message = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_error_duplicated_key"), data);
                    logicpos.Utils.ShowMessageTouch(GlobalApp.BackOfficeMainWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_exception_error"), message);
                }
                else
                {
                    _logger.Error(ex.Message, ex);
                    ResponseType response = logicpos.Utils.ShowMessageTouch(
                        GlobalApp.BackOfficeMainWindow,
                        DialogFlags.DestroyWithParent | DialogFlags.Modal,
                        MessageType.Warning, ButtonsType.Close,
                        CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_exception_error"),
                        (ex.InnerException.Message != null) ? ex.InnerException.Message : ex.Message
                        );
                }
                try
                {
                    _session.RollbackTransaction();
                }
                catch (Exception ex2)
                {
                    _logger.Error(ex.Message, ex2);
                }

                foreach (var item in _modifiedDataSourceRowObjects)
                {
                    if (debug) _logger.Debug(string.Format("UpdateRecord(): Reloading Modified XPGuidObjects item.Key:[{0}]", item.Key));
                    item.Key.Reload();
                }

                result = false;
            }

            return result;
        }
    }
}
