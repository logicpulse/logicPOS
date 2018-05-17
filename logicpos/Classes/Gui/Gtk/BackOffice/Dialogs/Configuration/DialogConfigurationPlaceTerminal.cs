using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogConfigurationPlaceTerminal : BOBaseDialog
    {
        public DialogConfigurationPlaceTerminal(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(Resx.window_title_edit_configurationplaceterminal);
            SetSizeRequest(500, 422);
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

                //Place
                XPOComboBox xpoComboBoxPlace = new XPOComboBox(DataSourceRow.Session, typeof(POS_ConfigurationPlace), (DataSourceRow as POS_ConfigurationPlaceTerminal).Place, "Designation");
                BOWidgetBox boxPlace = new BOWidgetBox(Resx.global_places, xpoComboBoxPlace);
                vboxTab1.PackStart(boxPlace, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPlace, DataSourceRow, "Place", SettingsApp.RegexGuid, false));

                //HardwareId
                Entry entryHardwareId = new Entry();
                BOWidgetBox boxHardwareId = new BOWidgetBox(Resx.global_hardware_id, entryHardwareId);
                vboxTab1.PackStart(boxHardwareId, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxHardwareId, _dataSourceRow, "HardwareId", SettingsApp.RegexAlfaNumericExtended, false));

                //Tab2
                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Printer
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationPrinters), (DataSourceRow as POS_ConfigurationPlaceTerminal).Printer, "Designation");
                BOWidgetBox boxPrinter = new BOWidgetBox(Resx.global_ConfigurationPrinters, xpoComboBoxPrinter);
                vboxTab2.PackStart(boxPrinter, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinter, DataSourceRow, "Printer", SettingsApp.RegexGuid, false));

                //PoleDisplay
                XPOComboBox xpoComboBoxPoleDisplay = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationPoleDisplay), (DataSourceRow as POS_ConfigurationPlaceTerminal).PoleDisplay, "Designation");
                BOWidgetBox boxPoleDisplay = new BOWidgetBox(Resx.global_ConfigurationPoleDisplay, xpoComboBoxPoleDisplay);
                vboxTab2.PackStart(boxPoleDisplay, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPoleDisplay, DataSourceRow, "PoleDisplay", SettingsApp.RegexGuid, false));

                //WeighingMachine
                XPOComboBox xpoComboBoxWeighingMachine = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationWeighingMachine), (DataSourceRow as POS_ConfigurationPlaceTerminal).WeighingMachine, "Designation");
                BOWidgetBox boxWeighingMachine = new BOWidgetBox(Resx.global_ConfigurationWeighingMachine, xpoComboBoxWeighingMachine);
                vboxTab2.PackStart(boxWeighingMachine, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxWeighingMachine, DataSourceRow, "WeighingMachine", SettingsApp.RegexGuid, false));

                //BarcodeReader
                XPOComboBox xpoComboBoxBarcodeReader = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationInputReader), (DataSourceRow as POS_ConfigurationPlaceTerminal).BarcodeReader, "Designation");
                BOWidgetBox boxBarcodeReader = new BOWidgetBox(Resx.global_input_barcode_reader, xpoComboBoxBarcodeReader);
                vboxTab2.PackStart(boxBarcodeReader, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxBarcodeReader, DataSourceRow, "BarcodeReader", SettingsApp.RegexGuid, false));

                //CardReader
                XPOComboBox xpoComboBoxCardReader = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationInputReader), (DataSourceRow as POS_ConfigurationPlaceTerminal).CardReader, "Designation");
                BOWidgetBox boxCardReader = new BOWidgetBox(Resx.global_input_reader_card_reader, xpoComboBoxCardReader);
                vboxTab2.PackStart(boxCardReader, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCardReader, DataSourceRow, "CardReader", SettingsApp.RegexGuid, false));

                //InputReaderTimerInterval
                Entry entryInputReaderTimerInterval = new Entry();
                BOWidgetBox boxInputReaderTimerInterval = new BOWidgetBox(Resx.global_input_reader_timer_interval, entryInputReaderTimerInterval);
                vboxTab2.PackStart(boxInputReaderTimerInterval, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxInputReaderTimerInterval, _dataSourceRow, "InputReaderTimerInterval", SettingsApp.RegexInteger, true));

                ////TemplateTicket : Deprecated
                //XPOComboBox xpoComboBoxTemplateTicket = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationPrintersTemplates), (DataSourceRow as POS_ConfigurationPlaceTerminal).TemplateTicket, "Designation", "FinancialTemplate = 0");
                //BOWidgetBox boxTemplateTicket = new BOWidgetBox(Resx.global_configurationprinters_template_ticket, xpoComboBoxTemplateTicket);
                //vboxTab1.PackStart(boxTemplateTicket, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplateTicket, DataSourceRow, "TemplateTicket", SettingsApp.RegexGuid, true));

                ////TemplateTablesConsult : Deprecated
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
                _notebook.AppendPage(vboxTab2, new Label(Resx.global_devices));

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
