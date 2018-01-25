using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogConfigurationPlaceTerminal : BOBaseDialog
    {
        public DialogConfigurationPlaceTerminal(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(Resx.window_title_edit_configurationplaceterminal);
            SetSizeRequest(500, 443);
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
                BOWidgetBox boxLabel = new BOWidgetBox(Resx.global_record_order, entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(Resx.global_record_code, entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(Resx.global_designation, entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", SettingsApp.RegexAlfaNumericExtended, true));

                //HardwareId
                Entry entryHardwareId = new Entry();
                BOWidgetBox boxHardwareId = new BOWidgetBox(Resx.global_hardware_id, entryHardwareId);
                vboxTab1.PackStart(boxHardwareId, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxHardwareId, _dataSourceRow, "HardwareId", SettingsApp.RegexAlfaNumericExtended, false));

                //Printer
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationPrinters), (DataSourceRow as POS_ConfigurationPlaceTerminal).Printer, "Designation");
                BOWidgetBox boxPrinter = new BOWidgetBox(Resx.global_ConfigurationPrinters, xpoComboBoxPrinter);
                vboxTab1.PackStart(boxPrinter, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinter, DataSourceRow, "Printer", SettingsApp.RegexGuid, false));

                //Place
                XPOComboBox xpoComboBoxPlace = new XPOComboBox(DataSourceRow.Session, typeof(POS_ConfigurationPlace), (DataSourceRow as POS_ConfigurationPlaceTerminal).Place, "Designation");
                BOWidgetBox boxPlace = new BOWidgetBox(Resx.global_places, xpoComboBoxPlace);
                vboxTab1.PackStart(boxPlace, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPlace, DataSourceRow, "Place", SettingsApp.RegexGuid, false));

                ////TemplateTicket
                //XPOComboBox xpoComboBoxTemplateTicket = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationPrintersTemplates), (DataSourceRow as POS_ConfigurationPlaceTerminal).TemplateTicket, "Designation", "FinancialTemplate = 0");
                //BOWidgetBox boxTemplateTicket = new BOWidgetBox(Resx.global_configurationprinters_template_ticket, xpoComboBoxTemplateTicket);
                //vboxTab1.PackStart(boxTemplateTicket, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplateTicket, DataSourceRow, "TemplateTicket", SettingsApp.RegexGuid, true));

                ////TemplateTablesConsult
                //XPOComboBox xpoComboBoxTemplateTablesConsult = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationPrintersTemplates), (DataSourceRow as POS_ConfigurationPlaceTerminal).TemplateTablesConsult, "Designation", "FinancialTemplate = 0");
                //BOWidgetBox boxTemplateTablesConsult = new BOWidgetBox(Resx.global_configurationprinters_template_table_consult, xpoComboBoxTemplateTablesConsult);
                //vboxTab1.PackStart(boxTemplateTablesConsult, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplateTablesConsult, DataSourceRow, "TemplateTablesConsult", SettingsApp.RegexGuid, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(Resx.global_record_disabled);
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(Resx.global_record_main_detail));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components
                entryHardwareId.Sensitive = false;
                checkButtonDisabled.Sensitive = false;
            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
