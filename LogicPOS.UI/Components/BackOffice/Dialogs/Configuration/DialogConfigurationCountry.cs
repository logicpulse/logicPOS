using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Globalization;
using LogicPOS.Domain.Entities;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationCountry : BOBaseDialog
    {
        public DialogConfigurationCountry(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_dialog_configuration_country"));
            
            SetSizeRequest(400, 398);
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

                //Capital
                Entry entryCapital = new Entry();
                BOWidgetBox boxCapital = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_country_capital"), entryCapital);
                vboxTab1.PackStart(boxCapital, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCapital, _dataSourceRow, "Capital", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false));

                //Currency
                Entry entryCurrency = new Entry();
                BOWidgetBox boxCurrency = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_currency"), entryCurrency);
                vboxTab1.PackStart(boxCurrency, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCurrency, _dataSourceRow, "Currency", LogicPOS.Utility.RegexUtils.RegexAlfa, false));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab2
                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Code2
                Entry entryCode2 = new Entry();
                BOWidgetBox boxCode2 = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_country_code2"), entryCode2);
                vboxTab2.PackStart(boxCode2, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode2, _dataSourceRow, "Code2", LogicPOS.Utility.RegexUtils.RegexAlfaCountryCode2, true));

                //Code3
                Entry entryCode3 = new Entry();
                BOWidgetBox boxCode3 = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_country_code3"), entryCode3);
                vboxTab2.PackStart(boxCode3, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode3, _dataSourceRow, "Code3", LogicPOS.Utility.RegexUtils.RegexAlfaCountryCode3, true));

                //CurrencyCode
                Entry entryCurrencyCode = new Entry();
                BOWidgetBox boxCurrencyCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_currency_code"), entryCurrencyCode);
                vboxTab2.PackStart(boxCurrencyCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCurrencyCode, _dataSourceRow, "CurrencyCode", LogicPOS.Utility.RegexUtils.RegexAcronym3Chars, false));

                //RegExFiscalNumber
                Entry entryRegExFiscalNumber = new Entry() { Sensitive = false };
                BOWidgetBox boxRegExFiscalNumber = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_regex_fiscal_number"), entryRegExFiscalNumber);
                vboxTab2.PackStart(boxRegExFiscalNumber, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxRegExFiscalNumber, _dataSourceRow, "RegExFiscalNumber"));

                //RegExZipCode
                Entry entryRegExZipCode = new Entry() { Sensitive = false };
                BOWidgetBox boxRegExZipCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_regex_postal_code"), entryRegExZipCode);
                vboxTab2.PackStart(boxRegExZipCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxRegExZipCode, _dataSourceRow, "RegExZipCode"));

                //Append Tab
                _notebook.AppendPage(vboxTab2, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_others")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}

