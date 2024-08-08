using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Globalization;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationCashRegister : EditDialog
    {
        public DialogConfigurationCashRegister(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
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
                InputFields.Add(new GenericCRUDWidgetXPO(boxLabel, Entity, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCode, Entity, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDesignation, Entity, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, Entity, "Disabled"));

                //Printer
                Entry entryPrinter = new Entry();
                BOWidgetBox boxPrinter = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_Printer"), entryPrinter);
                vboxTab1.PackStart(boxPrinter, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxPrinter, Entity, "Printer", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //Drawer
                Entry entryDrawer = new Entry();
                BOWidgetBox boxDrawer = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_Drawer"), entryDrawer);
                vboxTab1.PackStart(boxDrawer, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDrawer, Entity, "Drawer", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //AutomaticDrawer
                Entry entryAutomaticDrawer = new Entry();
                BOWidgetBox boxAutomaticDrawer = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_AutomaticDrawer"), entryAutomaticDrawer);
                vboxTab1.PackStart(boxAutomaticDrawer, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxAutomaticDrawer, Entity, "AutomaticDrawer", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //ActiveSales
                Entry entryActiveSales = new Entry();
                BOWidgetBox boxActiveSales = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_ActiveSales"), entryActiveSales);
                vboxTab1.PackStart(boxActiveSales, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxActiveSales, Entity, "ActiveSales", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));
                
                //AllowChargeBacks
                Entry entryAllowChargeBacks = new Entry();
                BOWidgetBox boxAllowChargeBacks = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_AllowChargeBacks"), entryAllowChargeBacks);
                vboxTab1.PackStart(boxAllowChargeBacks, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxAllowChargeBacks, Entity, "AllowChargeBacks", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

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
