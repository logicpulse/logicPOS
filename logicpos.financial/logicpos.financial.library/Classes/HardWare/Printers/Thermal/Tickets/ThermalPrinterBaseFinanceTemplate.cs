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
        protected FIN_DocumentFinanceType _documentType;
        protected List<int> _copyNames;
        protected string[] _copyNamesArray;
        protected bool _secondCopy;
        //Used in Child Documents
        protected string _copyName = string.Empty;
        protected int _ticketTablePaddingLeftLength = 2;

        public ThermalPrinterBaseFinanceTemplate(SYS_ConfigurationPrinters pPrinter, FIN_DocumentFinanceType pDocumentType, List<int> pCopyNames)
            : this(pPrinter, pDocumentType, pCopyNames, false)
        {
        }

        public ThermalPrinterBaseFinanceTemplate(SYS_ConfigurationPrinters pPrinter, FIN_DocumentFinanceType pDocumentType, List<int> pCopyNames, bool pSecondCopy)
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
                    _copyName = Resx.ResourceManager.GetString(string.Format("global_print_copy_title{0}", copyNameIndex));
                    if (_secondCopy && i < 1) _copyName = string.Format("{0}/{1}", _copyName, Resx.global_print_second_print);
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
                throw ex;
            }

            return result;
        }

        //Finance Extended Header
        protected void PrintExtendedHeader()
        {
            //Align Center
            _thermalPrinterGeneric.SetAlignCenter();

            //Extended Header
            _thermalPrinterGeneric.WriteLine(string.Format("{0}", _customVars["Company_Address"]));
            _thermalPrinterGeneric.WriteLine(string.Format("{0} {1} - {2}", _customVars["Company_Postalcode"], _customVars["Company_City"], _customVars["Company_Country"]));
            _thermalPrinterGeneric.WriteLine(Resx.global_phone, _customVars["Company_Telephone"]);
            _thermalPrinterGeneric.WriteLine(Resx.global_mobile_phone, _customVars["Company_Mobilephone"]);
            _thermalPrinterGeneric.WriteLine(Resx.global_fax, _customVars["Company_Fax"]);
            _thermalPrinterGeneric.WriteLine(Resx.global_email, _customVars["Company_Email"]);
            _thermalPrinterGeneric.WriteLine(Resx.global_website, _customVars["Company_Website"]);
            _thermalPrinterGeneric.WriteLine(Resx.global_fiscal_number_acronym, _customVars["Company_Fiscalnumber"]);
            _thermalPrinterGeneric.LineFeed();

            //Reset to Left
            _thermalPrinterGeneric.SetAlignLeft();
        }

        //Child Shared PrintDocumentMaster
        protected void PrintDocumentMaster(string pDocumentTypeResourceString, string pDocumentID, string pDocumentDateTime)
        {
            //Call Base PrintTitle()
            base.PrintTitles(Resx.ResourceManager.GetString(pDocumentTypeResourceString), pDocumentID);

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
            _thermalPrinterGeneric.WriteLine(Resx.global_customer, pName);
            _thermalPrinterGeneric.WriteLine(Resx.global_address, pAddress);
            if (!string.IsNullOrEmpty(pZipCode) && !string.IsNullOrEmpty(pCity) && !string.IsNullOrEmpty(pCountry))
            {
                _thermalPrinterGeneric.WriteLine(string.Format("{0} {1} - {2}", pZipCode, pCity, pCountry), false);
            }
            _thermalPrinterGeneric.WriteLine(Resx.global_fiscal_number, pFiscalNumber);
            _thermalPrinterGeneric.LineFeed();
        }

        public void PrintFooterExtended()
        {
            if (_customVars["Ticket_Footer_Line1"] != string.Empty || _customVars["Ticket_Footer_Line1"] != string.Empty)
            {
                //Align Center
                _thermalPrinterGeneric.SetAlignCenter();

                if (_customVars["Ticket_Footer_Line1"] != string.Empty) _thermalPrinterGeneric.WriteLine(_customVars["Ticket_Footer_Line1"]);
                if (_customVars["Ticket_Footer_Line2"] != string.Empty) _thermalPrinterGeneric.WriteLine(_customVars["Ticket_Footer_Line2"]);

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
                dataRow[0] = Resx.global_payment_conditions;
                dataRow[1] = pPaymentCondition;
                dataTable.Rows.Add(dataRow);
            }
            //Add Row : PaymentMethod
            if (!string.IsNullOrEmpty(pPaymentMethod))
            {
                dataRow = dataTable.NewRow();
                dataRow[0] = Resx.global_payment_method;
                dataRow[1] = pPaymentMethod;
                dataTable.Rows.Add(dataRow);
            }
            //Add Row : Currency
            dataRow = dataTable.NewRow();
            dataRow[0] = Resx.global_currency;
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
            string message = Resx.ResourceManager.GetString(pDocumentTypeMessage);
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

            //All Finance Documents use Processed, else Payments that use Emmited 
            string prefix = (_documentType.SaftDocumentType == SaftDocumentType.Payments)
                ? Resx.global_report_overlay_software_certification_emitted
                : Resx.global_report_overlay_software_certification_processed
            ;
            //Processed|Emitted with certified Software Nº {0}/AT
            string certificationText = string.Format(
                Resx.global_report_overlay_software_certification,
                prefix,
                SettingsApp.SaftSoftwareCertificateNumber
            );

            //Add Hash Validation if Defined (In DocumentFinance Only)
            if (pHash4Chars != string.Empty) certificationText = string.Format("{0} - {1}", pHash4Chars, certificationText);

            string copyRightText = string.Format(
                "Copyright {0}",
                SettingsApp.SaftProductID
            );

            string licenseText = string.Format(
                Resx.global_licensed_to,
                GlobalFramework.LicenceCompany
            );

            //Write Certification,CopyRight and License Text 
            if (SettingsApp.ConfigurationSystemCountry.Oid == SettingsApp.XpoOidConfigurationCountryPortugal)
            {
                _thermalPrinterGeneric.WriteLine(certificationText, WriteLineTextMode.Small);
            }
            _thermalPrinterGeneric.WriteLine(copyRightText, WriteLineTextMode.Small);
            _thermalPrinterGeneric.WriteLine(licenseText, WriteLineTextMode.Small);
            _thermalPrinterGeneric.LineFeed();

            //Reset to Left
            _thermalPrinterGeneric.SetAlignLeft();
        }
    }
}
