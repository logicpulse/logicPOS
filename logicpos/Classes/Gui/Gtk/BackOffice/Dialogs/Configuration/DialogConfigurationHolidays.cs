using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogConfigurationHolidays : BOBaseDialog
    {
        public DialogConfigurationHolidays(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_edit_dialog_configuration_holydays"));
            
            if(Utils.IsLinux) SetSizeRequest(400, 508);
            else SetSizeRequest(400, 488);
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
                BOWidgetBox boxLabel = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", SettingsApp.RegexAlfaNumericExtended, true));

                //Description
                Entry entryDescription = new Entry();
                BOWidgetBox boxDescription = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_description"), entryDescription);
                vboxTab1.PackStart(boxDescription, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDescription, _dataSourceRow, "Description", SettingsApp.RegexAlfaNumericExtended, false));

                //Year
                Entry entryYear = new Entry();
                BOWidgetBox boxYear = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_year"), entryYear);
                vboxTab1.PackStart(boxYear, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxYear, _dataSourceRow, "Year", SettingsApp.RegexDateYearHolidays, true));

                //Month
                Entry entryMonth = new Entry();
                BOWidgetBox boxMonth = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_month"), entryMonth);
                vboxTab1.PackStart(boxMonth, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxMonth, _dataSourceRow, "Month", SettingsApp.RegexDateMonth, true));

                //Day
                Entry entryDay = new Entry();
                BOWidgetBox boxDay = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_day"), entryDay);
                vboxTab1.PackStart(boxDay, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDay, _dataSourceRow, "Day", SettingsApp.RegexDateDay, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = SettingsApp.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));
            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
