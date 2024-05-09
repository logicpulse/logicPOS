using DevExpress.Data.Filtering;
using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.shared.App;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Settings.Extensions;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationPlaceTerminal : BOBaseDialog
    {
        public DialogConfigurationPlaceTerminal(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "window_title_edit_configurationplaceterminal"));
            
            SetSizeRequest(500, 522);

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
                BOWidgetBox boxLabel = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Place
                XPOComboBox xpoComboBoxPlace = new XPOComboBox(DataSourceRow.Session, typeof(pos_configurationplace), (DataSourceRow as pos_configurationplaceterminal).Place, "Designation", null);
                BOWidgetBox boxPlace = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_places"), xpoComboBoxPlace);
                vboxTab1.PackStart(boxPlace, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPlace, DataSourceRow, "Place", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //HardwareId
                Entry entryHardwareId = new Entry();
                BOWidgetBox boxHardwareId = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_hardware_id"), entryHardwareId);
                vboxTab1.PackStart(boxHardwareId, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxHardwareId, _dataSourceRow, "HardwareId", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false));

                //Tab2
                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Printer
                CriteriaOperator pcriteria = CriteriaOperator.Parse($"(Oid <> '{XPOSettings.XpoOidUndefinedRecord}' AND (PrinterType = '{PrintingSettings.XpoOidConfigurationPrinterTypeGenericWindows}' OR PrinterType = '{PrintingSettings.XpoOidConfigurationPrinterTypeExportPdf}'))");
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinters), (DataSourceRow as pos_configurationplaceterminal).Printer, "Designation", pcriteria);
                BOWidgetBox boxPrinter = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_ConfigurationPrinters"), xpoComboBoxPrinter);
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
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinter, DataSourceRow, "Printer", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //ThermalPrinter
                pcriteria = CriteriaOperator.Parse($"(Oid <> '{XPOSettings.XpoOidUndefinedRecord}' AND (PrinterType = '{PrintingSettings.XpoOidConfigurationPrinterTypeThermalPrinterWindows}'  OR PrinterType = '{PrintingSettings.XpoOidConfigurationPrinterTypeThermalPrinterSocket}' OR PrinterType = '{PrintingSettings.XpoOidConfigurationPrinterTypeThermalPrinterUsb}'))");

                XPOComboBox xpoComboBoxThermalPrinter = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinters), (DataSourceRow as pos_configurationplaceterminal).ThermalPrinter, "Designation", pcriteria);
                BOWidgetBox boxThermalPrinter = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_printer_thermal_printer"), xpoComboBoxThermalPrinter);
                vboxTab2.PackStart(boxThermalPrinter, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalPrinter, DataSourceRow, "ThermalPrinter", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //PoleDisplay
                XPOComboBox xpoComboBoxPoleDisplay = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationpoledisplay), (DataSourceRow as pos_configurationplaceterminal).PoleDisplay, "Designation", null);
                BOWidgetBox boxPoleDisplay = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_ConfigurationPoleDisplay"), xpoComboBoxPoleDisplay);
                vboxTab2.PackStart(boxPoleDisplay, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPoleDisplay, DataSourceRow, "PoleDisplay", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //WeighingMachine
                XPOComboBox xpoComboBoxWeighingMachine = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationweighingmachine), (DataSourceRow as pos_configurationplaceterminal).WeighingMachine, "Designation", null);
                BOWidgetBox boxWeighingMachine = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_ConfigurationWeighingMachine"), xpoComboBoxWeighingMachine);
                vboxTab2.PackStart(boxWeighingMachine, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxWeighingMachine, DataSourceRow, "WeighingMachine", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //BarcodeReader
                XPOComboBox xpoComboBoxBarcodeReader = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationinputreader), (DataSourceRow as pos_configurationplaceterminal).BarcodeReader, "Designation", null);
                BOWidgetBox boxBarcodeReader = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_input_barcode_reader"), xpoComboBoxBarcodeReader);
                vboxTab2.PackStart(boxBarcodeReader, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxBarcodeReader, DataSourceRow, "BarcodeReader", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //CardReader
                XPOComboBox xpoComboBoxCardReader = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationinputreader), (DataSourceRow as pos_configurationplaceterminal).CardReader, "Designation", null);
                BOWidgetBox boxCardReader = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_input_reader_card_reader"), xpoComboBoxCardReader);
                vboxTab2.PackStart(boxCardReader, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCardReader, DataSourceRow, "CardReader", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //InputReaderTimerInterval
                Entry entryInputReaderTimerInterval = new Entry();
                BOWidgetBox boxInputReaderTimerInterval = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_input_reader_timer_interval"), entryInputReaderTimerInterval);
                vboxTab2.PackStart(boxInputReaderTimerInterval, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxInputReaderTimerInterval, _dataSourceRow, "InputReaderTimerInterval", LogicPOS.Utility.RegexUtils.RegexInteger, true));

                ////TemplateTicket : Deprecated
                //XPOComboBox xpoComboBoxTemplateTicket = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstemplates), (DataSourceRow as pos_configurationplaceterminal).TemplateTicket, "Designation", "FinancialTemplate = 0");
                //BOWidgetBox boxTemplateTicket = new BOWidgetBox(CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_configurationprinters_template_ticket, xpoComboBoxTemplateTicket);
                //vboxTab1.PackStart(boxTemplateTicket, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplateTicket, DataSourceRow, "TemplateTicket", SettingsApp.RegexGuid, true));

                ////TemplateTablesConsult : Deprecated
                //XPOComboBox xpoComboBoxTemplateTablesConsult = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstemplates), (DataSourceRow as pos_configurationplaceterminal).TemplateTablesConsult, "Designation", "FinancialTemplate = 0");
                //BOWidgetBox boxTemplateTablesConsult = new BOWidgetBox(CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_configurationprinters_template_table_consult, xpoComboBoxTemplateTablesConsult);
                //vboxTab1.PackStart(boxTemplateTablesConsult, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplateTablesConsult, DataSourceRow, "TemplateTablesConsult", SettingsApp.RegexGuid, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_record_disabled"));
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_record_main_detail")));
                _notebook.AppendPage(vboxTab2, new Label(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_devices")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components
                entryHardwareId.Sensitive = false;
                checkButtonDisabled.Sensitive = false;
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
