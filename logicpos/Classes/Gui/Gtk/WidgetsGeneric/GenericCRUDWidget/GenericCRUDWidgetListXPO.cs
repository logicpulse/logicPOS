using DevExpress.Xpo;
using Gtk;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.App;
using logicpos.resources.Resources.Localization;
using System;

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

        public override bool Save()
        {
            bool debug = false;
            bool result = true;

            try
            {
                //BeginTransaction
                if (debug) _log.Debug("UpdateRecord(): BeginTransaction");
                _session.BeginTransaction();
                foreach (var item in _modifiedDataSourceRowObjects)
                {
                    if (debug) _log.Debug(string.Format("UpdateRecord(): Saving Modified XPGuidObjects item.Key:[{0}]", item.Key));
                    item.Key.Save();
                }
                //CommitTransaction 
                if (debug) _log.Debug("UpdateRecord(): CommitTransaction");
                _session.CommitTransaction();
                result = true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);

                ResponseType response = Utils.ShowMessageTouch(GlobalApp.WindowBackOffice, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Warning, ButtonsType.Close, Resx.window_title_dialog_exception_error, ex.InnerException.Message);

                //RollbackTransaction
                //_log.Debug("UpdateRecord(): RollbackTransaction");
                _session.RollbackTransaction();
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
