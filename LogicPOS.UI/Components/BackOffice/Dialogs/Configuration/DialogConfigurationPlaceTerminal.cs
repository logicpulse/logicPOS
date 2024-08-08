using DevExpress.Data.Filtering;
using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Components;
using LogicPOS.Utility;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationPlaceTerminal : EditDialog
    {
        public DialogConfigurationPlaceTerminal(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(GeneralUtils.GetResourceByName("window_title_edit_configurationplaceterminal"));
            
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
                BOWidgetBox boxLabel = new BOWidgetBox(GeneralUtils.GetResourceByName("global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxLabel, Entity, "Ord", RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(GeneralUtils.GetResourceByName("global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCode, Entity, "Code", RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(GeneralUtils.GetResourceByName("global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDesignation, Entity, "Designation", RegexUtils.RegexAlfaNumericExtended, true));

                //Place
                XPOComboBox xpoComboBoxPlace = new XPOComboBox(Entity.Session, typeof(pos_configurationplace), (Entity as pos_configurationplaceterminal).Place, "Designation", null);
                BOWidgetBox boxPlace = new BOWidgetBox(GeneralUtils.GetResourceByName("global_places"), xpoComboBoxPlace);
                vboxTab1.PackStart(boxPlace, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxPlace, Entity, "Place", RegexUtils.RegexGuid, false));

                //HardwareId
                Entry entryHardwareId = new Entry();
                BOWidgetBox boxHardwareId = new BOWidgetBox(GeneralUtils.GetResourceByName("global_hardware_id"), entryHardwareId);
                vboxTab1.PackStart(boxHardwareId, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxHardwareId, Entity, "HardwareId", RegexUtils.RegexAlfaNumericExtended, false));

                //Tab2
                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Printer
                CriteriaOperator pcriteria = CriteriaOperator.Parse($"(Oid <> '{XPOSettings.XpoOidUndefinedRecord}' AND (PrinterType = '{PrintingSettings.WindowsGenericPrinterId}' OR PrinterType = '{PrintingSettings.ExportToPdfPrinterId}'))");
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(Entity.Session, typeof(sys_configurationprinters), (Entity as pos_configurationplaceterminal).Printer, "Designation", pcriteria);
                BOWidgetBox boxPrinter = new BOWidgetBox(GeneralUtils.GetResourceByName("global_ConfigurationPrinters"), xpoComboBoxPrinter);
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
                InputFields.Add(new GenericCRUDWidgetXPO(boxPrinter, Entity, "Printer", RegexUtils.RegexGuid, false));

                //ThermalPrinter
                pcriteria = CriteriaOperator.Parse($"(Oid <> '{XPOSettings.XpoOidUndefinedRecord}' AND (PrinterType = '{PrintingSettings.WindowsThermalPrinterId}'  OR PrinterType = '{PrintingSettings.ThermalSocketPrinterId}' OR PrinterType = '{PrintingSettings.UsbThermalPrinterId}'))");

                XPOComboBox xpoComboBoxThermalPrinter = new XPOComboBox(Entity.Session, typeof(sys_configurationprinters), (Entity as pos_configurationplaceterminal).ThermalPrinter, "Designation", pcriteria);
                BOWidgetBox boxThermalPrinter = new BOWidgetBox(GeneralUtils.GetResourceByName("global_printer_thermal_printer"), xpoComboBoxThermalPrinter);
                vboxTab2.PackStart(boxThermalPrinter, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxThermalPrinter, Entity, "ThermalPrinter", RegexUtils.RegexGuid, false));

                //PoleDisplay
                XPOComboBox xpoComboBoxPoleDisplay = new XPOComboBox(Entity.Session, typeof(sys_configurationpoledisplay), (Entity as pos_configurationplaceterminal).PoleDisplay, "Designation", null);
                BOWidgetBox boxPoleDisplay = new BOWidgetBox(GeneralUtils.GetResourceByName("global_ConfigurationPoleDisplay"), xpoComboBoxPoleDisplay);
                vboxTab2.PackStart(boxPoleDisplay, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxPoleDisplay, Entity, "PoleDisplay", RegexUtils.RegexGuid, false));

                //WeighingMachine
                XPOComboBox xpoComboBoxWeighingMachine = new XPOComboBox(Entity.Session, typeof(sys_configurationweighingmachine), (Entity as pos_configurationplaceterminal).WeighingMachine, "Designation", null);
                BOWidgetBox boxWeighingMachine = new BOWidgetBox(GeneralUtils.GetResourceByName("global_ConfigurationWeighingMachine"), xpoComboBoxWeighingMachine);
                vboxTab2.PackStart(boxWeighingMachine, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxWeighingMachine, Entity, "WeighingMachine", RegexUtils.RegexGuid, false));

                //BarcodeReader
                XPOComboBox xpoComboBoxBarcodeReader = new XPOComboBox(Entity.Session, typeof(sys_configurationinputreader), (Entity as pos_configurationplaceterminal).BarcodeReader, "Designation", null);
                BOWidgetBox boxBarcodeReader = new BOWidgetBox(GeneralUtils.GetResourceByName("global_input_barcode_reader"), xpoComboBoxBarcodeReader);
                vboxTab2.PackStart(boxBarcodeReader, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxBarcodeReader, Entity, "BarcodeReader", RegexUtils.RegexGuid, false));

                //CardReader
                XPOComboBox xpoComboBoxCardReader = new XPOComboBox(Entity.Session, typeof(sys_configurationinputreader), (Entity as pos_configurationplaceterminal).CardReader, "Designation", null);
                BOWidgetBox boxCardReader = new BOWidgetBox(GeneralUtils.GetResourceByName("global_input_reader_card_reader"), xpoComboBoxCardReader);
                vboxTab2.PackStart(boxCardReader, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCardReader, Entity, "CardReader", RegexUtils.RegexGuid, false));

                //InputReaderTimerInterval
                Entry entryInputReaderTimerInterval = new Entry();
                BOWidgetBox boxInputReaderTimerInterval = new BOWidgetBox(GeneralUtils.GetResourceByName("global_input_reader_timer_interval"), entryInputReaderTimerInterval);
                vboxTab2.PackStart(boxInputReaderTimerInterval, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxInputReaderTimerInterval, Entity, "InputReaderTimerInterval", RegexUtils.RegexInteger, true));

                ////TemplateTicket : Deprecated
                //XPOComboBox xpoComboBoxTemplateTicket = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstemplates), (DataSourceRow as pos_configurationplaceterminal).TemplateTicket, "Designation", "FinancialTemplate = 0");
                //BOWidgetBox boxTemplateTicket = new BOWidgetBox(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_configurationprinters_template_ticket, xpoComboBoxTemplateTicket);
                //vboxTab1.PackStart(boxTemplateTicket, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplateTicket, DataSourceRow, "TemplateTicket", SettingsApp.RegexGuid, true));

                ////TemplateTablesConsult : Deprecated
                //XPOComboBox xpoComboBoxTemplateTablesConsult = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstemplates), (DataSourceRow as pos_configurationplaceterminal).TemplateTablesConsult, "Designation", "FinancialTemplate = 0");
                //BOWidgetBox boxTemplateTablesConsult = new BOWidgetBox(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_configurationprinters_template_table_consult, xpoComboBoxTemplateTablesConsult);
                //vboxTab1.PackStart(boxTemplateTablesConsult, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplateTablesConsult, DataSourceRow, "TemplateTablesConsult", SettingsApp.RegexGuid, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, Entity, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(GeneralUtils.GetResourceByName("global_record_main_detail")));
                _notebook.AppendPage(vboxTab2, new Label(GeneralUtils.GetResourceByName("global_devices")));

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
