using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Globalization;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components;
using LogicPOS.UI.Application;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationPaymentMethod : EditDialog
    {
        public DialogConfigurationPaymentMethod(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_configurationpaymentmethod"));
            
            SetSizeRequest(500, 398);
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

                //Token
                Entry entryToken = new Entry();
                BOWidgetBox boxToken = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationPaymentMethod_Token"), entryToken);
                vboxTab1.PackStart(boxToken, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxToken, Entity, "Token", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, true));

                //Acronym
                Entry entryAcronym = new Entry();
                BOWidgetBox boxAcronym = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationPaymentMethod_Acronym"), entryAcronym);
                vboxTab1.PackStart(boxAcronym, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxAcronym, Entity, "Acronym", LogicPOS.Utility.RegexUtils.RegexAcronym2Chars, true));

                //Symbol
                //Entry entrySymbol = new Entry();
                //BOWidgetBox boxSymbol = new BOWidgetBox(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationPaymentMethod_Symbol, entrySymbol);
                //vboxTab1.PackStart(boxSymbol, false, false, 0);
                //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxSymbol, _dataSourceRow, "Symbol", SettingsApp.RegexAlfaNumeric, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = LogicPOSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, Entity, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_main_detail")));

                //Disable Components
                entryOrd.Sensitive = false;
                entryCode.Sensitive = false;
                entryToken.Sensitive = false;
                entryAcronym.Sensitive = false;
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
