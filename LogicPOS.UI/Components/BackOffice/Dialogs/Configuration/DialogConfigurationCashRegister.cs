using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationCashRegister : BOBaseDialog
    {
        public DialogConfigurationCashRegister(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_configurationcashregister"));
            
            SetSizeRequest(500, 600);
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
                BOWidgetBox boxLabel = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Printer
                Entry entryPrinter = new Entry();
                BOWidgetBox boxPrinter = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_Printer"), entryPrinter);
                vboxTab1.PackStart(boxPrinter, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinter, _dataSourceRow, "Printer", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //Drawer
                Entry entryDrawer = new Entry();
                BOWidgetBox boxDrawer = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_Drawer"), entryDrawer);
                vboxTab1.PackStart(boxDrawer, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDrawer, _dataSourceRow, "Drawer", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //AutomaticDrawer
                Entry entryAutomaticDrawer = new Entry();
                BOWidgetBox boxAutomaticDrawer = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_AutomaticDrawer"), entryAutomaticDrawer);
                vboxTab1.PackStart(boxAutomaticDrawer, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxAutomaticDrawer, _dataSourceRow, "AutomaticDrawer", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //ActiveSales
                Entry entryActiveSales = new Entry();
                BOWidgetBox boxActiveSales = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_ActiveSales"), entryActiveSales);
                vboxTab1.PackStart(boxActiveSales, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxActiveSales, _dataSourceRow, "ActiveSales", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));
                
                //AllowChargeBacks
                Entry entryAllowChargeBacks = new Entry();
                BOWidgetBox boxAllowChargeBacks = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_AllowChargeBacks"), entryAllowChargeBacks);
                vboxTab1.PackStart(boxAllowChargeBacks, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxAllowChargeBacks, _dataSourceRow, "AllowChargeBacks", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_main_detail")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
