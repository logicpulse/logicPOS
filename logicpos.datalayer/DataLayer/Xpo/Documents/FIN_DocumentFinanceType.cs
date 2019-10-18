using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.Enums;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_documentfinancetype : XPGuidObject
    {
        public fin_documentfinancetype() : base() { }
        public fin_documentfinancetype(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(fin_documentfinancetype), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(fin_documentfinancetype), "Code");
        }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        UInt32 fCode;
        [Indexed(Unique = true)]
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        string fAcronym;
        [Size(4)]
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        Int32 fAcronymLastSerie;
        public Int32 AcronymLastSerie
        {
            get { return fAcronymLastSerie; }
            set { SetPropertyValue<Int32>("AcronymLastSerie", ref fAcronymLastSerie, value); }
        }

        string fResourceString;
        public string ResourceString
        {
            get { return fResourceString; }
            set { SetPropertyValue<string>("ResourceString", ref fResourceString, value); }
        }

        string fResourceStringReport;
        public string ResourceStringReport
        {
            get { return fResourceStringReport; }
            set { SetPropertyValue<string>("ResourceStringReport", ref fResourceStringReport, value); }
        }

        //Default Value when create new DocumentMaster
        Boolean fPayed;
        public Boolean Payed
        {
            get { return fPayed; }
            set { SetPropertyValue<Boolean>("Payed", ref fPayed, value); }
        }

        Boolean fCredit;
        public Boolean Credit
        {
            get { return fCredit; }
            set { SetPropertyValue<Boolean>("Credit", ref fCredit, value); }
        }

        Int32 fCreditDebit;
        public Int32 CreditDebit
        {
            get { return fCreditDebit; }
            set { SetPropertyValue<Int32>("CreditDebit", ref fCreditDebit, value); }
        }

        //Number of Copies to Print, ex 2, Generate Original, Duplicate, Note: this is not the number of copies to print in document
        Int32 fPrintCopies;
        public Int32 PrintCopies
        {
            get { return fPrintCopies; }
            set { SetPropertyValue<Int32>("PrintCopies", ref fPrintCopies, value); }
        }

        Boolean fPrintRequestMotive;
        public Boolean PrintRequestMotive
        {
            get { return fPrintRequestMotive; }
            set { SetPropertyValue<Boolean>("PrintRequestMotive", ref fPrintRequestMotive, value); }
        }

        Boolean fPrintRequestConfirmation;
        public Boolean PrintRequestConfirmation
        {
            get { return fPrintRequestConfirmation; }
            set { SetPropertyValue<Boolean>("PrintRequestConfirmation", ref fPrintRequestConfirmation, value); }
        }

        Boolean fPrintOpenDrawer;
        public Boolean PrintOpenDrawer
        {
            get { return fPrintOpenDrawer; }
            set { SetPropertyValue<Boolean>("PrintOpenDrawer", ref fPrintOpenDrawer, value); }
        }

        Boolean fWayBill;
        public Boolean WayBill
        {
            get { return fWayBill; }
            set { SetPropertyValue<Boolean>("WayBill", ref fWayBill, value); }
        }

        //Valid AT WebService Document
        Boolean fWsAtDocument;
        public Boolean WsAtDocument
        {
            get { return fWsAtDocument; }
            set { SetPropertyValue<Boolean>("WsAtDocument", ref fWsAtDocument, value); }
        }

        //SAF-T PT:Is a SAF-T PT Audit Financial Document
        Boolean fSaftAuditFile;
        public Boolean SaftAuditFile
        {
            get { return fSaftAuditFile; }
            set { SetPropertyValue<Boolean>("SaftAuditFile", ref fSaftAuditFile, value); }
        }

        //SAF-T PT: SaftDocumentType: SalesInvoices, WorkingDocuments or MovementOfGoods
        SaftDocumentType fSaftDocumentType;
        public SaftDocumentType SaftDocumentType
        {
            get { return fSaftDocumentType; }
            set { SetPropertyValue<SaftDocumentType>("SaftDocumentType", ref fSaftDocumentType, value); }
        }

        ProcessArticleStockMode fStockMode;
        public ProcessArticleStockMode StockMode
        {
            get { return fStockMode; }
            set { SetPropertyValue<ProcessArticleStockMode>("StockMode", ref fStockMode, value); }
        }

        //DocumentFinanceType One <> Many ConfigurationPlaceTerminal
        sys_configurationprinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesDocumentFinanceType")]
        public sys_configurationprinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<sys_configurationprinters>("Printer", ref fPrinter, value); }
        }

        //DocumentFinanceType One <> Many ConfigurationPrintersTemplates
        sys_configurationprinterstemplates fTemplate;
        [Association(@"ConfigurationPrintersTemplatesReferencesDocumentFinanceType")]
        public sys_configurationprinterstemplates Template
        {
            get { return fTemplate; }
            set { SetPropertyValue<sys_configurationprinterstemplates>("Template", ref fTemplate, value); }
        }

        //DocumentFinanceType One <> Many DocumentFinanceMaster
        [Association(@"DocumentFinanceTypeReferencesDocumentFinanceMaster", typeof(fin_documentfinancemaster))]
        public XPCollection<fin_documentfinancemaster> DocumentMaster
        {
            get { return GetCollection<fin_documentfinancemaster>("DocumentMaster"); }
        }

        //DocumentFinanceType One <> Many DocumentFinancePayment
        [Association(@"DocumentFinanceTypeReferencesDocumentFinancePayment", typeof(fin_documentfinancepayment))]
        public XPCollection<fin_documentfinancemaster> DocumentPayment
        {
            get { return GetCollection<fin_documentfinancemaster>("DocumentPayment"); }
        }

        //DocumentFinanceType One <> Many DocumentFinanceSeries
        [Association(@"DocumentFinanceTypeReferencesDocumentFinanceSeries", typeof(fin_documentfinanceseries))]
        public XPCollection<fin_documentfinanceseries> Series
        {
            get { return GetCollection<fin_documentfinanceseries>("Series"); }
        }

        //DocumentFinanceType One <> Many DocumentFinanceYearSerieTerminal
        [Association(@"DocumentFinanceTypeReferencesDocumentFinanceYearSerieTerminal", typeof(fin_documentfinanceyearserieterminal))]
        public XPCollection<fin_documentfinanceyearserieterminal> YearSerieTerminal
        {
            get { return GetCollection<fin_documentfinanceyearserieterminal>("YearSerieTerminal"); }
        }
    }
}