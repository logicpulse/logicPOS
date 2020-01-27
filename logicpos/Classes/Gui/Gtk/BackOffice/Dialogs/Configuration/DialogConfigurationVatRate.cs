using Gtk;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogConfigurationVatRate : BOBaseDialog
    {
        public DialogConfigurationVatRate(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_edit_configurationvatrate"));
            
            if (Utils.IsLinux) SetSizeRequest(500, 553);
            else SetSizeRequest(500, 533);
            InitUI();
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Get XpoObject Reference from DataSourceRow
                fin_configurationvatrate _configurationVatRate = (DataSourceRow as fin_configurationvatrate);

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
                
                //Value
                Entry entryValue = new Entry();
                BOWidgetBox boxValue = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_rate_value"), entryValue);
                vboxTab1.PackStart(boxValue, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxValue, _dataSourceRow, "Value", SettingsApp.RegexDecimalGreaterThanZero, true));

                //TaxType
                Entry entryTaxType = new Entry();
                BOWidgetBox boxTaxType = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_rate_tax_type"), entryTaxType);
                vboxTab1.PackStart(boxTaxType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTaxType, _dataSourceRow, "TaxType", SettingsApp.RegexAlfa, true));

                //TaxCode
                Entry entryTaxCode = new Entry();
                BOWidgetBox boxTaxCode = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_rate_tax_code"), entryTaxCode);
                vboxTab1.PackStart(boxTaxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTaxCode, _dataSourceRow, "TaxCode", SettingsApp.RegexAlfa, true));

                //TaxCountryRegion
                Entry entryTaxCountryRegion = new Entry();
                BOWidgetBox boxTaxCountryRegion = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_rate_tax_country_region"), entryTaxCountryRegion);
                vboxTab1.PackStart(boxTaxCountryRegion, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTaxCountryRegion, _dataSourceRow, "TaxCountryRegion", SettingsApp.RegexAlfaNumericExtended, true));

                //TaxDescription
                Entry entryTaxDescription = new Entry();
                BOWidgetBox boxTaxDescription = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_description"), entryTaxDescription);
                vboxTab1.PackStart(boxTaxDescription, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTaxDescription, _dataSourceRow, "TaxDescription", SettingsApp.RegexAlfaNumericExtended, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = SettingsApp.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));

                //Disable Components
                bool enableComponents = !(
                    _configurationVatRate.Oid == SettingsApp.XpoOidConfigurationVatRateNormalPT ||
                    _configurationVatRate.Oid == SettingsApp.XpoOidConfigurationVatRateIntermediatePT ||
                    _configurationVatRate.Oid == SettingsApp.XpoOidConfigurationVatRateReducedPT ||
                    _configurationVatRate.Oid == SettingsApp.XpoOidConfigurationVatRateNormalPTMA ||
                    _configurationVatRate.Oid == SettingsApp.XpoOidConfigurationVatRateIntermediatePTMA ||
                    _configurationVatRate.Oid == SettingsApp.XpoOidConfigurationVatRateReducedPTMA ||
                    _configurationVatRate.Oid == SettingsApp.XpoOidConfigurationVatRateNormalPTAC ||
                    _configurationVatRate.Oid == SettingsApp.XpoOidConfigurationVatRateIntermediatePTAC ||
                    _configurationVatRate.Oid == SettingsApp.XpoOidConfigurationVatRateReducedPTAC
                );
                entryDesignation.Sensitive = enableComponents;
                entryTaxType.Sensitive = enableComponents;
                entryTaxCode.Sensitive = enableComponents;
                entryTaxCountryRegion.Sensitive = enableComponents;
                checkButtonDisabled.Sensitive = enableComponents;
            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
