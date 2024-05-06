using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.Settings.Extensions;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationDevice : BOBaseDialog
    {
        public DialogConfigurationDevice(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_edit_configurationdevice"));

            SetSizeRequest(500, 500);
            InitUI();
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //PlaceTerminal
                Entry entryPlaceTerminal = new Entry();
                BOWidgetBox boxPlaceTerminal = new BOWidgetBox(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_ConfigurationDevice_PlaceTerminal"), entryPlaceTerminal);
                vboxTab1.PackStart(boxPlaceTerminal, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPlaceTerminal, _dataSourceRow, "PlaceTerminal", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_record_main_detail")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
