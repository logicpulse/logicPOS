using LogicPOS.UI.Components.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Windows
{
    public partial class BackOfficeWindow
    {
        public void UpdatePrivileges()
        {
            #region Documents
            BtnNewDocument.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTSNEW_MENU");
            BtnDocuments.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTSLISTALL_MENU");
            BtnReceiptsEmission.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTSPAY_MENU");
            BtnReceipts.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTSPAYMENTS_MENU");
            BtnCurrentAccount.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTSCURRENTACCOUNT_MENU");
            #endregion

            #region Reports
            BtnReports.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTSREPORTS_MENU");
            #endregion

            #region Articles
            BtnArticleFamilies.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLEFAMILY_MENU");
            BtnArticleSubfamilies.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLESUBFAMILY_MENU");
            BtnArticles.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLE_MENU");
            BtnArticleTypes.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLETYPE_MENU");
            BtnArticleClasses.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLECLASS_MENU");
            BtnPriceTypes.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPRICETYPE_MENU");
            BtnSotck.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLESTOCK_MENU");
            #endregion

            #region FiscalYear
            BtnFiscalYears.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_MENU");
            BtnDocumentTypes.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_MENU");
            BtnDocumentSeries.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCESERIES_MANAGE_SERIES");
            BtnVatRates.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONVATRATE_CREATE");
            BtnVatExemptionReasons.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONVATEXEMPTIONREASON_MENU");
            BtnPaymentConditions.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPAYMENTCONDITION_MENU");
            BtnPaymentMethods.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPAYMENTMETHOD_MENU");
            #endregion

            #region Customers
            BtnCustomers.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMER_MENU");
            BtnCustomerTypes.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMERTYPE_MENU");
            BtnCustomerDiscountGroups.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMERDISCOUNTGROUP_MENU");
            #endregion

            #region Users
            BtnUsers.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERDETAIL_MENU");
            BtnPermissions.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERPERMISSIONPROFILE_MENU");
            BtnCommissionGroups.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERCOMMISSIONGROUP_MENU");
            #endregion

            #region Devices
            BtnPrinterTypes.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPRINTERSTYPE_MENU");
            BtnPrinters.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPRINTERS_MENU");
            BtnCommissionGroups.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERCOMMISSIONGROUP_MENU");
            BtnInputReaders.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONINPUTREADER_MENU");
            BtnPoleDisplays.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPOLEDISPLAY_MENU");
            BtnWeighingMachine.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONWEIGHINGMACHINE_MENU");
            #endregion

            #region Others
            BtnCountries.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONCOUNTRY_MENU");
            BtnCurrencies.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONCURRENCY_MENU");
            BtnPlaces.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACE_MENU");
            BtnTables.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_MENU");
            BtnMovementTypes.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACEMOVEMENTTYPE_MENU");
            BtnMeasurementUnits.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONUNITMEASURE_MENU");
            BtnSizeUnits.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONUNITSIZE_MENU");
            BtnHolidays.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONHOLIDAYS_MENU");
            BtnWarehouses.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_WAREHOUSE_MENU");
            #endregion

            #region Configurations
            BtnCompanyPreferenceParameters.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETERCOMPANY_MENU");
            BtnSystemPreferenceParameters.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETERSYSTEM_MENU");
            BtnTerminals.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_MENU");
            #endregion

            #region Import
            BtnImportArticles.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_SYSTEM_IMPORT_ARTICLES_MENU");
            BtnImportCustomers.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_SYSTEM_IMPORT_CUSTOMERS_MENU");
            #endregion

            #region Export
            BtnExportArticles.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_SYSTEM_EXPORT_ARTICLES_MENU");
            BtnExportCustomers.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_SYSTEM_EXPORT_COSTUMERS_MENU");
            BtnExportCustomSaft.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_SYSTEM_EXPORTSAFTPT_CUSTOM_MENU");
            BtnExportLastMonthSaft.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_SYSTEM_EXPORTSAFTPT_SAFTPT_MENU");
            BtnExportYearlySaft.Button.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_SYSTEM_EXPORTSAFTPT_E-FATURA_MENU");
            #endregion

        }
    }
}
