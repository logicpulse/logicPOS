using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using logicpos.Classes.Enums.Dialogs;
using System;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogConfigurationPrinters : BOBaseDialog
    {
        private VBox _vboxTab2;
        private XPOComboBox _xpoComboBoxPrinterType;
        private Entry _entryThermalEncoding;
        private Entry _entryThermalImageCompanyLogo;
        private Entry _entryThermalMaxCharsPerLineNormal;
        private Entry _entryThermalMaxCharsPerLineNormalBold;
        private Entry _entryThermalMaxCharsPerLineSmall;
        private Entry _entryThermalCutCommand;
        private Entry _entryThermalOpenDrawerValueM;
        private Entry _entryThermalOpenDrawerValueT1;
        private Entry _entryThermalOpenDrawerValueT2;

        private sys_configurationprinters _configurationPrinter;

        public DialogConfigurationPrinters(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_edit_dialogconfigurationprinters"));
            SetSizeRequest(500, 448);
            InitUI();
            InitNotes();
            ShowAll();
            //Tab Visibility Require to be after ShowAll, else always is Visible
            _configurationPrinter = (_dataSourceRow as sys_configurationprinters);
            _vboxTab2.Visible = (_configurationPrinter.PrinterType != null && _configurationPrinter.PrinterType.ThermalPrinter);
        }

        private void InitUI()
        {
            try
            {
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                // HBoxs
                HBox hbox1 = new HBox(true, _boxSpacing);
                HBox hbox2 = new HBox(true, _boxSpacing);

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

                //PrinterType
                _xpoComboBoxPrinterType = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstype), (DataSourceRow as sys_configurationprinters).PrinterType, "Designation", null);
                BOWidgetBox boxPrinterType = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printer_type"), _xpoComboBoxPrinterType);
                vboxTab1.PackStart(boxPrinterType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinterType, DataSourceRow, "PrinterType", SettingsApp.RegexGuid, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", SettingsApp.RegexAlfaNumericExtended, true));

                //NetworkName
                Entry entryNetworkName = new Entry();
                BOWidgetBox boxNetworkName = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_networkname"), entryNetworkName);
                vboxTab1.PackStart(boxNetworkName, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxNetworkName, _dataSourceRow, "NetworkName", SettingsApp.RegexHardwarePrinterNetworkNameAndUsbEndpoint, false));

                //Tab2
                _vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //ThermalMaxCharsPerLineNormal
                _entryThermalMaxCharsPerLineNormal = new Entry();
                BOWidgetBox boxThermalMaxCharsPerLineNormal = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printer_thermal_max_chars_per_line_normal"), _entryThermalMaxCharsPerLineNormal);
                _vboxTab2.PackStart(boxThermalMaxCharsPerLineNormal, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalMaxCharsPerLineNormal, _dataSourceRow, "ThermalMaxCharsPerLineNormal", SettingsApp.RegexInteger, true));

                //ThermalMaxCharsPerLineNormalBold
                _entryThermalMaxCharsPerLineNormalBold = new Entry();
                BOWidgetBox boxThermalMaxCharsPerLineNormalBold = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printer_thermal_max_chars_per_line_normal_bold"), _entryThermalMaxCharsPerLineNormalBold);
                _vboxTab2.PackStart(boxThermalMaxCharsPerLineNormalBold, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalMaxCharsPerLineNormalBold, _dataSourceRow, "ThermalMaxCharsPerLineNormalBold", SettingsApp.RegexInteger, true));

                //ThermalMaxCharsPerLineSmall
                _entryThermalMaxCharsPerLineSmall = new Entry();
                BOWidgetBox boxThermalMaxCharsPerLineSmall = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printer_thermal_max_chars_per_line_small"), _entryThermalMaxCharsPerLineSmall);
                _vboxTab2.PackStart(boxThermalMaxCharsPerLineSmall, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalMaxCharsPerLineSmall, _dataSourceRow, "ThermalMaxCharsPerLineSmall", SettingsApp.RegexInteger, true));

                //ThermalEncoding
                _entryThermalEncoding = new Entry();
                BOWidgetBox boxThermalEncoding = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printer_thermal_encoding"), _entryThermalEncoding);
                //_vboxTab2.PackStart(boxThermalEncoding, false, false, 0);
                hbox1.PackStart(boxThermalEncoding, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalEncoding, _dataSourceRow, "ThermalEncoding", SettingsApp.RegexAlfaNumeric, false));

                //ThermalCutCommand
                _entryThermalCutCommand = new Entry();
                BOWidgetBox boxThermalCutCommand = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printer_thermal_cut_command"), _entryThermalCutCommand);
                //_vboxTab2.PackStart(boxThermalCutCommand, false, false, 0);
                hbox1.PackStart(boxThermalCutCommand, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalCutCommand, _dataSourceRow, "ThermalCutCommand", SettingsApp.RegexAlfaNumericExtended, false));

                // Pack hbox
                _vboxTab2.PackStart(hbox1, false, false, 0);

                //ThermalOpenDrawerValueM
                _entryThermalOpenDrawerValueM = new Entry();
                BOWidgetBox boxThermalOpenDrawerValueM = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printer_thermal_open_drawer_value_m"), _entryThermalOpenDrawerValueM);
                //_vboxTab2.PackStart(boxThermalOpenDrawerValueM, false, false, 0);
                hbox2.PackStart(boxThermalOpenDrawerValueM, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalOpenDrawerValueM, _dataSourceRow, "ThermalOpenDrawerValueM", SettingsApp.RegexAlfaNumericExtended, false));

                //ThermalOpenDrawerValueT1
                _entryThermalOpenDrawerValueT1 = new Entry();
                BOWidgetBox boxThermalOpenDrawerValueT1 = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printer_thermal_open_drawer_value_t1"), _entryThermalOpenDrawerValueT1);
                //_vboxTab2.PackStart(boxThermalOpenDrawerValueT1, false, false, 0);
                hbox2.PackStart(boxThermalOpenDrawerValueT1, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalOpenDrawerValueT1, _dataSourceRow, "ThermalOpenDrawerValueT1", SettingsApp.RegexAlfaNumericExtended, false));

                //ThermalOpenDrawerValueT2
                _entryThermalOpenDrawerValueT2 = new Entry();
                BOWidgetBox boxThermalOpenDrawerValueT2 = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printer_thermal_open_drawer_value_t2"), _entryThermalOpenDrawerValueT2);
                //_vboxTab2.PackStart(boxThermalOpenDrawerValueT2, false, false, 0);
                hbox2.PackStart(boxThermalOpenDrawerValueT2, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalOpenDrawerValueT2, _dataSourceRow, "ThermalOpenDrawerValueT2", SettingsApp.RegexAlfaNumericExtended, false));

                // Pack hbox
                _vboxTab2.PackStart(hbox2, false, false, 0);

                //ThermalPrintLogo
                _entryThermalImageCompanyLogo = new Entry();
                BOWidgetBox boxThermalImageCompanyLogo = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printer_thermal_image_company_logo"), _entryThermalImageCompanyLogo);
                _vboxTab2.PackStart(boxThermalImageCompanyLogo, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxThermalImageCompanyLogo, _dataSourceRow, "ThermalImageCompanyLogo", SettingsApp.RegexAlfaNumericFilePath, false));

                //ThermalPrintLogo
                CheckButton checkButtonThermalPrintLogo = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printer_thermal_print_logo"));
                _vboxTab2.PackStart(checkButtonThermalPrintLogo, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonThermalPrintLogo, _dataSourceRow, "ThermalPrintLogo"));

                // Events
                _xpoComboBoxPrinterType.Changed += XpoComboBoxPrinterType_Changed;

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = SettingsApp.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));
                _notebook.AppendPage(_vboxTab2, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_main_properties")));
            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void XpoComboBoxPrinterType_Changed(object sender, System.EventArgs e)
        {
            try
            {
                _log.Debug(string.Format("Message: [{0}]", _xpoComboBoxPrinterType.Value));

                //Tab Visibility
                _vboxTab2.Visible = (_xpoComboBoxPrinterType.Value != null && (_xpoComboBoxPrinterType.Value as sys_configurationprinterstype).ThermalPrinter);

                // If Working in ThermalPrinter assign Defaults From Config
                if (_vboxTab2.Visible)
                {
                    if (string.IsNullOrEmpty(_entryThermalEncoding.Text)) _entryThermalEncoding.Text = SettingsApp.PrinterThermalEncoding;
                    if (string.IsNullOrEmpty(_entryThermalImageCompanyLogo.Text)) _entryThermalImageCompanyLogo.Text = SettingsApp.PrinterThermalImageCompanyLogo;
                    if (_entryThermalMaxCharsPerLineNormal.Text.Equals("0")) _entryThermalMaxCharsPerLineNormal.Text = Convert.ToString(SettingsApp.PrinterThermalMaxCharsPerLineNormal);
                    if (_entryThermalMaxCharsPerLineNormalBold.Text.Equals("0")) _entryThermalMaxCharsPerLineNormalBold.Text = Convert.ToString(SettingsApp.PrinterThermalMaxCharsPerLineNormalBold);
                    if (_entryThermalMaxCharsPerLineSmall.Text.Equals("0")) _entryThermalMaxCharsPerLineSmall.Text = Convert.ToString(SettingsApp.PrinterThermalMaxCharsPerLineSmall);
                    if (string.IsNullOrEmpty(_entryThermalCutCommand.Text)) _entryThermalCutCommand.Text = SettingsApp.PrinterThermalCutCommand;
                    if (_entryThermalOpenDrawerValueM.Text.Equals("0")) _entryThermalOpenDrawerValueM.Text = Convert.ToString(SettingsApp.PrinterThermalOpenDrawerValueM);
                    if (_entryThermalOpenDrawerValueT1.Text.Equals("0")) _entryThermalOpenDrawerValueT1.Text = Convert.ToString(SettingsApp.PrinterThermalOpenDrawerValueT1);
                    if (_entryThermalOpenDrawerValueT2.Text.Equals("0")) _entryThermalOpenDrawerValueT2.Text = Convert.ToString(SettingsApp.PrinterThermalOpenDrawerValueT2);
                }
                // If Not Working in ThermalPrinter reset Values
                else
                {
                    _entryThermalEncoding.Text = null;
                    _entryThermalImageCompanyLogo.Text = null;
                    _entryThermalMaxCharsPerLineNormal.Text = null;
                    _entryThermalMaxCharsPerLineNormalBold.Text = null;
                    _entryThermalMaxCharsPerLineSmall.Text = null;
                    _entryThermalCutCommand.Text = null;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
