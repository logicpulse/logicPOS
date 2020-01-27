using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogConfigurationCountry : BOBaseDialog
    {
        public DialogConfigurationCountry(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_edit_dialog_configuration_country"));
            
            if (Utils.IsLinux) SetSizeRequest(400, 418);
            else SetSizeRequest(400, 398);
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

                //Capital
                Entry entryCapital = new Entry();
                BOWidgetBox boxCapital = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_country_capital"), entryCapital);
                vboxTab1.PackStart(boxCapital, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCapital, _dataSourceRow, "Capital", SettingsApp.RegexAlfaNumericExtended, false));

                //Currency
                Entry entryCurrency = new Entry();
                BOWidgetBox boxCurrency = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_currency"), entryCurrency);
                vboxTab1.PackStart(boxCurrency, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCurrency, _dataSourceRow, "Currency", SettingsApp.RegexAlfa, false));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = SettingsApp.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab2
                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Code2
                Entry entryCode2 = new Entry();
                BOWidgetBox boxCode2 = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_country_code2"), entryCode2);
                vboxTab2.PackStart(boxCode2, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode2, _dataSourceRow, "Code2", SettingsApp.RegexAlfaCountryCode2, true));

                //Code3
                Entry entryCode3 = new Entry();
                BOWidgetBox boxCode3 = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_country_code3"), entryCode3);
                vboxTab2.PackStart(boxCode3, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode3, _dataSourceRow, "Code3", SettingsApp.RegexAlfaCountryCode3, true));

                //CurrencyCode
                Entry entryCurrencyCode = new Entry();
                BOWidgetBox boxCurrencyCode = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_currency_code"), entryCurrencyCode);
                vboxTab2.PackStart(boxCurrencyCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCurrencyCode, _dataSourceRow, "CurrencyCode", SettingsApp.RegexAcronym3Chars, false));

                //RegExFiscalNumber
                Entry entryRegExFiscalNumber = new Entry() { Sensitive = false };
                BOWidgetBox boxRegExFiscalNumber = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_regex_fiscal_number"), entryRegExFiscalNumber);
                vboxTab2.PackStart(boxRegExFiscalNumber, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxRegExFiscalNumber, _dataSourceRow, "RegExFiscalNumber"));

                //RegExZipCode
                Entry entryRegExZipCode = new Entry() { Sensitive = false };
                BOWidgetBox boxRegExZipCode = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_regex_postal_code"), entryRegExZipCode);
                vboxTab2.PackStart(boxRegExZipCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxRegExZipCode, _dataSourceRow, "RegExZipCode"));

                //Append Tab
                _notebook.AppendPage(vboxTab2, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_others")));
            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}

