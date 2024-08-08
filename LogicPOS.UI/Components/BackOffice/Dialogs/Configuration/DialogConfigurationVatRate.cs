﻿using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationVatRate : EditDialog
    {
        public DialogConfigurationVatRate(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_edit_configurationvatrate"));

            SetSizeRequest(500, 533);
           
            InitUI();
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Get XpoObject Reference from DataSourceRow
                fin_configurationvatrate _configurationVatRate = (Entity as fin_configurationvatrate);

                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxLabel, Entity, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCode, Entity, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDesignation, Entity, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Value
                Entry entryValue = new Entry();
                BOWidgetBox boxValue = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_vat_rate_value"), entryValue);
                vboxTab1.PackStart(boxValue, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxValue, Entity, "Value", LogicPOS.Utility.RegexUtils.RegexDecimalGreaterThanZero, true));

                //TaxType
                Entry entryTaxType = new Entry();
                BOWidgetBox boxTaxType = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_vat_rate_tax_type"), entryTaxType);
                vboxTab1.PackStart(boxTaxType, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxTaxType, Entity, "TaxType", LogicPOS.Utility.RegexUtils.RegexAlfa, true));

                //TaxCode
                Entry entryTaxCode = new Entry();
                BOWidgetBox boxTaxCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_vat_rate_tax_code"), entryTaxCode);
                vboxTab1.PackStart(boxTaxCode, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxTaxCode, Entity, "TaxCode", LogicPOS.Utility.RegexUtils.RegexAlfa, true));

                //TaxCountryRegion
                Entry entryTaxCountryRegion = new Entry();
                BOWidgetBox boxTaxCountryRegion = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_vat_rate_tax_country_region"), entryTaxCountryRegion);
                vboxTab1.PackStart(boxTaxCountryRegion, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxTaxCountryRegion, Entity, "TaxCountryRegion", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //TaxDescription
                Entry entryTaxDescription = new Entry();
                BOWidgetBox boxTaxDescription = new BOWidgetBox(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_description"), entryTaxDescription);
                vboxTab1.PackStart(boxTaxDescription, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxTaxDescription, Entity, "TaxDescription", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, Entity, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_main_detail")));

                //Disable Components
                bool enableComponents = !(
                    _configurationVatRate.Oid == InvoiceSettings.XpoOidConfigurationVatRateNormalPT ||
                    _configurationVatRate.Oid == InvoiceSettings.XpoOidConfigurationVatRateIntermediatePT ||
                    _configurationVatRate.Oid == InvoiceSettings.XpoOidConfigurationVatRateReducedPT ||
                    _configurationVatRate.Oid == InvoiceSettings.XpoOidConfigurationVatRateNormalPTMA ||
                    _configurationVatRate.Oid == InvoiceSettings.XpoOidConfigurationVatRateIntermediatePTMA ||
                    _configurationVatRate.Oid == InvoiceSettings.XpoOidConfigurationVatRateReducedPTMA ||
                    _configurationVatRate.Oid == InvoiceSettings.XpoOidConfigurationVatRateNormalPTAC ||
                    _configurationVatRate.Oid == InvoiceSettings.XpoOidConfigurationVatRateIntermediatePTAC ||
                    _configurationVatRate.Oid == InvoiceSettings.XpoOidConfigurationVatRateReducedPTAC
                );
                entryDesignation.Sensitive = enableComponents;
                entryTaxType.Sensitive = enableComponents;
                entryTaxCode.Sensitive = enableComponents;
                entryTaxCountryRegion.Sensitive = enableComponents;
                checkButtonDisabled.Sensitive = enableComponents;
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
