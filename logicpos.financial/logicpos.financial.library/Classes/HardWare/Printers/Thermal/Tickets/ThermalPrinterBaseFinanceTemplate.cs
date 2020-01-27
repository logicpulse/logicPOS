using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using logicpos.financial.library.Classes.Reports;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets
{
    public abstract class ThermalPrinterBaseFinanceTemplate : ThermalPrinterBaseTemplate
    {
        protected fin_documentfinancetype _documentType;
        protected List<int> _copyNames;
        protected string[] _copyNamesArray;
        protected bool _secondCopy;
        //Used in Child Documents
        protected string _copyName = string.Empty;
        protected int _ticketTablePaddingLeftLength = 2;

        public ThermalPrinterBaseFinanceTemplate(sys_configurationprinters pPrinter, fin_documentfinancetype pDocumentType, List<int> pCopyNames)
            : this(pPrinter, pDocumentType, pCopyNames, false)
        {
        }

        public ThermalPrinterBaseFinanceTemplate(sys_configurationprinters pPrinter, fin_documentfinancetype pDocumentType, List<int> pCopyNames, bool pSecondCopy)
            : base(pPrinter, SettingsApp.PrinterThermalImageCompanyLogo)
        {
            //Assign Parameter Properties
            _documentType = pDocumentType;
            _copyNames = pCopyNames;
            _secondCopy = pSecondCopy;

            //Generate CopyNamesArray (Original, Duplicate,...)
            if (_copyNames != null)
            {
                _copyNamesArray = CustomReport.CopyNames(pCopyNames);
            }
        }

        //Override Parent Template
        public override bool Print()
        {
            bool result = false;
            int copyNameIndex = 0;

            try
            {
                for (int i = 0; i < _copyNames.Count; i++)
                {
                    //Call Base Template PrintHeader
                    base.PrintHeader();
                    //PrintExtendedHeader
                    PrintExtendedHeader();

                    //Get CopyName Position, ex 0[Original], 4[Quadriplicate], we cant use I, else 0[Original], 1[Duplicate]
                    copyNameIndex = _copyNames[i] + 1;
                    //Overrided by Child Classes
                    _copyName = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], string.Format("global_print_copy_title{0}", copyNameIndex));
                    if (_secondCopy && i < 1) _copyName = string.Format("{0}/{1}", _copyName, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_print_second_print"));
                    //_log.Debug(String.Format("copyName: [{0}], copyNameIndex: [{1}]", _copyName, copyNameIndex));

                    //Call Child Content (Overrided)
                    PrintContent();

                    //PrintFooterExtended
                    PrintFooterExtended();

                    //Call Base Template PrintFooter
                    base.PrintFooter();
                }

                //End Job
                PrintBuffer();

                result = true;
            }
            catch (Exception ex)
            {
                _log.Debug("override bool Print() :: Thermal Printer: " + ex.Message, ex);
                throw ex;
            }

            return result;
        }

        //Finance Extended Header
        protected void PrintExtendedHeader()
        {
            //Align Center
            _thermalPrinterGeneric.SetAlignLeft();/* IN009055 */

            //Extended Header
            _thermalPrinterGeneric.WriteLine(string.Format("{0}", _customVars["COMPANY_ADDRESS"]));
            _thermalPrinterGeneric.WriteLine(string.Format("{0} {1} - {2}", _customVars["COMPANY_POSTALCODE"], _customVars["COMPANY_CITY"], _customVars["COMPANY_COUNTRY"]));
			/* IN009055 block */
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "prefparam_company_telephone"), _customVars["COMPANY_TELEPHONE"]);
            //_thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_mobile_phone, _customVars["COMPANY_MOBILEPHONE"]);
            //_thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_fax, _customVars["COMPANY_FAX"]);
            //_thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_email"), _customVars["COMPANY_EMAIL"]);
            _thermalPrinterGeneric.WriteLine(_customVars["COMPANY_WEBSITE"], false); /* IN009211 */
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "prefparam_company_fiscalnumber"), _customVars["COMPANY_FISCALNUMBER"]);
            _thermalPrinterGeneric.LineFeed();

            //Reset to Left
            //_thermalPrinterGeneric.SetAlignLeft(); /* IN009055 */
        }

        //Child Shared PrintDocumentMaster
        protected void PrintDocumentMaster(string pDocumentTypeResourceString, string pDocumentID, string pDocumentDateTime)
        {
            //Call Base PrintTitle()
            base.PrintTitles(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], pDocumentTypeResourceString), pDocumentID);

            //Set Align Center
            _thermalPrinterGeneric.SetAlignCenter();

            //Copy Names + Document Date
            _thermalPrinterGeneric.WriteLine(_copyName);
            _thermalPrinterGeneric.WriteLine(pDocumentDateTime);
            _thermalPrinterGeneric.LineFeed();

            //Reset Align 
            _thermalPrinterGeneric.SetAlignLeft();
        }

        //Child Shared PrintCustomer
        protected void PrintCustomer(string pName, string pAddress, string pZipCode, string pCity, string pCountry, string pFiscalNumber)
        {
			/* IN009055 block */
            string fiscalNumber = pFiscalNumber;
            string name = pName;

            /* IN009055: pFiscalNumber real value is overwritten by GetFRBOFinancePayment(Guid pDocumentFinancePaymentOid) method */
            if (SettingsApp.FinanceFinalConsumerFiscalNumberDisplay.Equals(pFiscalNumber) || SettingsApp.FinanceFinalConsumerFiscalNumber.Equals(pFiscalNumber))
            {
                //fiscalNumber = SettingsApp.FinanceFinalConsumerFiscalNumberDisplay;
                fiscalNumber = string.Empty; /* show the Fical Number display value is not necessary */
                name = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_final_consumer");
            }

			/* IN009055 block - begin */
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_customer"), name, false);
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_address"), pAddress, false);

            string addressDetails = pCountry;

            if (!string.IsNullOrEmpty(pZipCode) && !string.IsNullOrEmpty(pCity))
            {
                addressDetails = string.Format("{0} {1} - {2}", pZipCode, pCity, pCountry);
            }
            else if (!string.IsNullOrEmpty(pZipCode))
            {
                addressDetails = string.Format("{0} - {1}", pZipCode, pCountry);
            }
            else if (!string.IsNullOrEmpty(pCity))
            {
                addressDetails = string.Format("{0} - {1}", pCity, pCountry);
            }
            _thermalPrinterGeneric.WriteLine(addressDetails, false); /* When FS, no details */
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_fiscal_number"), fiscalNumber, false); /* Do not print Fiscal Number when empty */
            /* IN009055  block - end */
            _thermalPrinterGeneric.LineFeed();
        }

        public void PrintFooterExtended()
        {
            if (_customVars["TICKET_FOOTER_LINE1"] != string.Empty || _customVars["TICKET_FOOTER_LINE1"] != string.Empty)
            {
                //Align Center
                _thermalPrinterGeneric.SetAlignCenter();

                if (_customVars["TICKET_FOOTER_LINE1"] != string.Empty) _thermalPrinterGeneric.WriteLine(_customVars["TICKET_FOOTER_LINE1"]);
                if (_customVars["TICKET_FOOTER_LINE2"] != string.Empty) _thermalPrinterGeneric.WriteLine(_customVars["TICKET_FOOTER_LINE2"]);

                //Line Feed
                _thermalPrinterGeneric.LineFeed();

                //Reset to Left
                _thermalPrinterGeneric.SetAlignLeft();
            }
        }

        //Chield Shared PrintCustomer
        protected void PrintDocumentPaymentDetails(string pPaymentMethod, string pCurrency)
        {
            PrintDocumentPaymentDetails(string.Empty, pPaymentMethod, pCurrency);
        }

        protected void PrintDocumentPaymentDetails(string pPaymentCondition, string pPaymentMethod, string pCurrency)
        {
            //Init DataTable
            DataRow dataRow = null;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Label", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Value", typeof(string)));

            //Add Row : PaymentConditions
            if (!string.IsNullOrEmpty(pPaymentCondition))
            {
                dataRow = dataTable.NewRow();
                dataRow[0] = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_payment_conditions");
                dataRow[1] = pPaymentCondition;
                dataTable.Rows.Add(dataRow);
            }
            //Add Row : PaymentMethod
            if (!string.IsNullOrEmpty(pPaymentMethod))
            {
                dataRow = dataTable.NewRow();
                dataRow[0] = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_payment_method_field"); /* IN009055 */
                dataRow[1] = pPaymentMethod;
                dataTable.Rows.Add(dataRow);
            }
            //Add Row : Currency
            dataRow = dataTable.NewRow();
            dataRow[0] = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_currency_field"); /* IN009055 */
            dataRow[1] = pCurrency;
            dataTable.Rows.Add(dataRow);

            //Configure Ticket Column Properties
            List<TicketColumn> columns = new List<TicketColumn>();
            columns.Add(new TicketColumn("Label", "", _thermalPrinterGeneric.MaxCharsPerLineNormal / 2, TicketColumnsAlign.Right));
            columns.Add(new TicketColumn("Value", "", _thermalPrinterGeneric.MaxCharsPerLineNormal / 2, TicketColumnsAlign.Left));

            //TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
            TicketTable ticketTable = new TicketTable(dataTable, columns, _thermalPrinterGeneric.MaxCharsPerLineNormal);

            //Create Table Buffer, With Bigger TextMode
            ticketTable.Print(_thermalPrinterGeneric, true);

            //Line Feed
            _thermalPrinterGeneric.LineFeed();
        }

        //Used On Child Finance Documents - ResourceStringReport
        protected void PrintDocumentTypeFooterString(string pDocumentTypeMessage)
        {
            //Align Center
            _thermalPrinterGeneric.SetAlignCenter();

            //Differ from Payments and Other Document Types
            string message = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], pDocumentTypeMessage);
            if (pDocumentTypeMessage != string.Empty && message != null) _thermalPrinterGeneric.WriteLine(message);

            //Line Feed
            _thermalPrinterGeneric.LineFeed();

            //Reset to Left
            _thermalPrinterGeneric.SetAlignLeft();
        }

        protected void PrintCertificationText()
        {
            PrintCertificationText(string.Empty);
        }

        //Used On Child Finance Documents
        protected void PrintCertificationText(string pHash4Chars)
        {
            //Align Center
            _thermalPrinterGeneric.SetAlignCenter();

            /* IN009211 */
            string copyRightText = string.Format(
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_copyright") + " {0}",
                SettingsApp.SaftProductID
            );

            string certificationText = string.Empty;

            //Write Certification,CopyRight and License Text 
            if (SettingsApp.XpoOidConfigurationCountryPortugal.Equals(SettingsApp.ConfigurationSystemCountry.Oid))
            {
                //All Finance Documents use Processed, else Payments that use Emmited 
                string prefix = (_documentType.SaftDocumentType == SaftDocumentType.Payments)
                    ? resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_emitted")
                    : resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_processed")
                ;
                //Processed|Emitted with certified Software Nº {0}/AT
                certificationText = string.Format(
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification"),
                    prefix,
                    SettingsApp.SaftSoftwareCertificateNumber
                );

                //Add Hash Validation if Defined (In DocumentFinance Only)
                if (pHash4Chars != string.Empty)
                {
                    certificationText = string.Format("{0} - {1}", pHash4Chars, certificationText);
                }
            }
            /* IN005975 and IN005979 for Mozambique deployment */
            else if (SettingsApp.XpoOidConfigurationCountryMozambique.Equals(SettingsApp.ConfigurationSystemCountry.Oid))
            {/* IN009055 - related to IN006047 */
				/* {Processado por computador} || Autorização da Autoridade Tributária: {DAFM1 - 0198 / 2018} */
                certificationText = string.Format(
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_short"),
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_moz_tax_authority_cert_number") + "\n" + 
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_processed")
                 );
            }
			//TK016268 Angola - Certificação 
            else if (SettingsApp.XpoOidConfigurationCountryAngola.Equals(SettingsApp.ConfigurationSystemCountry.Oid))
            {
                //All Finance Documents use Processed, else Payments that use Emmited 
                string prefix = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_processed"); ;
                //Current Year
                string localDate = DateTime.Now.Year.ToString();
                //Processed|Emitted with certified Software Nº {0}/AT
                certificationText = string.Format(
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_ao"),
                    prefix,
                    SettingsApp.SaftSoftwareCertificateNumberAO,
                    SettingsApp.SaftProductIDAO,
                    localDate);  
            }

            else {
				/* All other countries: "Processado por computador" */
                certificationText = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_processed");
            }

            _thermalPrinterGeneric.WriteLine(certificationText, WriteLineTextMode.Small);

            _thermalPrinterGeneric.WriteLine(copyRightText, WriteLineTextMode.Small);

            /* IN009211 - it was printing empty label */
            if (!String.IsNullOrEmpty(GlobalFramework.LicenceCompany))
            {
                string licenseText = string.Format(
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_licensed_to"),
                    GlobalFramework.LicenceCompany
            );
                _thermalPrinterGeneric.WriteLine(licenseText, WriteLineTextMode.Small);
            }

            _thermalPrinterGeneric.LineFeed();

            //Reset to Left
            _thermalPrinterGeneric.SetAlignLeft();
        }
    }
}
