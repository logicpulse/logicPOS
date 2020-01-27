using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using logicpos.Classes.Enums.Dialogs;
using DevExpress.Data.Filtering;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogConfigurationPlaceTerminal : BOBaseDialog
    {
        public DialogConfigurationPlaceTerminal(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_edit_configurationplaceterminal"));
            
            if (Utils.IsLinux) SetSizeRequest(500, 542);
            else SetSizeRequest(500, 522);
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

                //Place
                XPOComboBox xpoComboBoxPlace = new XPOComboBox(DataSourceRow.Session, typeof(pos_configurationplace), (DataSourceRow as pos_configurationplaceterminal).Place, "Designation", null);
                BOWidgetBox boxPlace = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_places"), xpoComboBoxPlace);
                vboxTab1.PackStart(boxPlace, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPlace, DataSourceRow, "Place", SettingsApp.RegexGuid, false));

                //HardwareId
                Entry entryHardwareId = new Entry();
                BOWidgetBox boxHardwareId = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_hardware_id"), entryHardwareId);
                vboxTab1.PackStart(boxHardwareId, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxHardwareId, _dataSourceRow, "HardwareId", SettingsApp.RegexAlfaNumericExtended, false));

                //Tab2
                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Printer
                CriteriaOperator pcriteria = CriteriaOperator.Parse(string.Format("(Oid <> '{0}' AND (PrinterType = '{1}' OR PrinterType = '{2}' OR PrinterType = '{3}'))", SettingsApp.XpoOidUndefinedRecord, SettingsApp.XpoOidConfigurationPrinterTypeGenericWindows, SettingsApp.XpoOidConfigurationPrinterTypeGenericLinux, SettingsApp.XpoOidConfigurationPrinterTypeExportPdf));
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinters), (DataSourceRow as pos_configurationplaceterminal).Printer, "Designation", pcriteria);
                BOWidgetBox boxPrinter = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ConfigurationPrinters"), xpoComboBoxPrinter);
                TreeIter iter;
                xpoComboBoxPrinter.Model.GetIterFirst(out iter);
                do
                {
                    GLib.Value thisRow = new GLib.Value();
                    xpoComboBoxPrinter.Model.GetValue(iter, 0, ref thisRow);
                    //if ((thisRow.Val as string).Equals("Exportação para PDF"))
                    //{
                    //    xpoComboBoxPrinter.SetActiveIter(iter);
                    //    break;
                    //}

                } while (xpoComboBoxPrinter.Model.IterNext(ref iter));
                vboxTab2.PackStart(boxPrinter, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinter, DataSourceRow, "Printer", SettingsApp.RegexGuid, false));

                //ThermalPrinter
                pcriteria = CriteriaOperator.Parse(string.Format("(Oid <> '{0}' AND (PrinterType = '{1}' OR PrinterType = '{2}' OR PrinterType = '{3}' OR PrinterType = '{4}'))", SettingsApp.XpoOidUndefinedRecord, SettingsApp.XpoOidConfigurationPrinterTypeThermalPrinterWindows, SettingsApp.XpoOidConfigurationPrinterTypeThermalPrinterLinux, SettingsApp.XpoOidConfigurationPrinterTypeThermalPrinterSocket, SettingsApp.XpoOidConfigurationPrinterTypeThermalPrinterUsb));
                
                XPOComboBox xpoComboBoxThermalPrinter = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinters), (DataSourceRow as pos_configurationplaceterminal).ThermalPrinter, "Designation", pcriteria);
                BOWidgetBox boxThermalPrinter = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printer_thermal_printer"), xpoComboBoxThermalPrinter);
                vboxTab2.PackStart(boxThermalPrinter, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalPrinter, DataSourceRow, "ThermalPrinter", SettingsApp.RegexGuid, false));

                //PoleDisplay
                XPOComboBox xpoComboBoxPoleDisplay = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationpoledisplay), (DataSourceRow as pos_configurationplaceterminal).PoleDisplay, "Designation", null);
                BOWidgetBox boxPoleDisplay = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ConfigurationPoleDisplay"), xpoComboBoxPoleDisplay);
                vboxTab2.PackStart(boxPoleDisplay, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPoleDisplay, DataSourceRow, "PoleDisplay", SettingsApp.RegexGuid, false));

                //WeighingMachine
                XPOComboBox xpoComboBoxWeighingMachine = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationweighingmachine), (DataSourceRow as pos_configurationplaceterminal).WeighingMachine, "Designation", null);
                BOWidgetBox boxWeighingMachine = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ConfigurationWeighingMachine"), xpoComboBoxWeighingMachine);
                vboxTab2.PackStart(boxWeighingMachine, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxWeighingMachine, DataSourceRow, "WeighingMachine", SettingsApp.RegexGuid, false));

                //BarcodeReader
                XPOComboBox xpoComboBoxBarcodeReader = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationinputreader), (DataSourceRow as pos_configurationplaceterminal).BarcodeReader, "Designation", null);
                BOWidgetBox boxBarcodeReader = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_input_barcode_reader"), xpoComboBoxBarcodeReader);
                vboxTab2.PackStart(boxBarcodeReader, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxBarcodeReader, DataSourceRow, "BarcodeReader", SettingsApp.RegexGuid, false));

                //CardReader
                XPOComboBox xpoComboBoxCardReader = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationinputreader), (DataSourceRow as pos_configurationplaceterminal).CardReader, "Designation", null);
                BOWidgetBox boxCardReader = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_input_reader_card_reader"), xpoComboBoxCardReader);
                vboxTab2.PackStart(boxCardReader, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCardReader, DataSourceRow, "CardReader", SettingsApp.RegexGuid, false));

                //InputReaderTimerInterval
                Entry entryInputReaderTimerInterval = new Entry();
                BOWidgetBox boxInputReaderTimerInterval = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_input_reader_timer_interval"), entryInputReaderTimerInterval);
                vboxTab2.PackStart(boxInputReaderTimerInterval, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxInputReaderTimerInterval, _dataSourceRow, "InputReaderTimerInterval", SettingsApp.RegexInteger, true));

                ////TemplateTicket : Deprecated
                //XPOComboBox xpoComboBoxTemplateTicket = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstemplates), (DataSourceRow as pos_configurationplaceterminal).TemplateTicket, "Designation", "FinancialTemplate = 0");
                //BOWidgetBox boxTemplateTicket = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_configurationprinters_template_ticket, xpoComboBoxTemplateTicket);
                //vboxTab1.PackStart(boxTemplateTicket, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplateTicket, DataSourceRow, "TemplateTicket", SettingsApp.RegexGuid, true));

                ////TemplateTablesConsult : Deprecated
                //XPOComboBox xpoComboBoxTemplateTablesConsult = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstemplates), (DataSourceRow as pos_configurationplaceterminal).TemplateTablesConsult, "Designation", "FinancialTemplate = 0");
                //BOWidgetBox boxTemplateTablesConsult = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_configurationprinters_template_table_consult, xpoComboBoxTemplateTablesConsult);
                //vboxTab1.PackStart(boxTemplateTablesConsult, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplateTablesConsult, DataSourceRow, "TemplateTablesConsult", SettingsApp.RegexGuid, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_disabled"));
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));
                _notebook.AppendPage(vboxTab2, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_devices")));

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
