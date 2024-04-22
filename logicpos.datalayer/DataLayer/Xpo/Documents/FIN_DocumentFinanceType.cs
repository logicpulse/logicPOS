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
            Ord = DataLayerUtils.GetNextTableFieldID(nameof(fin_documentfinancetype), "Ord");
            Code = DataLayerUtils.GetNextTableFieldID(nameof(fin_documentfinancetype), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fAcronym;
        [Size(4)]
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        private int fAcronymLastSerie;
        public int AcronymLastSerie
        {
            get { return fAcronymLastSerie; }
            set { SetPropertyValue<int>("AcronymLastSerie", ref fAcronymLastSerie, value); }
        }

        private string fResourceString;
        public string ResourceString
        {
            get { return fResourceString; }
            set { SetPropertyValue<string>("ResourceString", ref fResourceString, value); }
        }

        private string fResourceStringReport;
        public string ResourceStringReport
        {
            get { return fResourceStringReport; }
            set { SetPropertyValue<string>("ResourceStringReport", ref fResourceStringReport, value); }
        }

        //Default Value when create new DocumentMaster
        private bool fPayed;
        public bool Payed
        {
            get { return fPayed; }
            set { SetPropertyValue<bool>("Payed", ref fPayed, value); }
        }

        private bool fCredit;
        public bool Credit
        {
            get { return fCredit; }
            set { SetPropertyValue<bool>("Credit", ref fCredit, value); }
        }

        private int fCreditDebit;
        public int CreditDebit
        {
            get { return fCreditDebit; }
            set { SetPropertyValue<int>("CreditDebit", ref fCreditDebit, value); }
        }

        //Number of Copies to Print, ex 2, Generate Original, Duplicate, Note: this is not the number of copies to print in document
        private int fPrintCopies;
        public int PrintCopies
        {
            get { return fPrintCopies; }
            set { SetPropertyValue<int>("PrintCopies", ref fPrintCopies, value); }
        }

        private bool fPrintRequestMotive;
        public bool PrintRequestMotive
        {
            get { return fPrintRequestMotive; }
            set { SetPropertyValue<bool>("PrintRequestMotive", ref fPrintRequestMotive, value); }
        }

        private bool fPrintRequestConfirmation;
        public bool PrintRequestConfirmation
        {
            get { return fPrintRequestConfirmation; }
            set { SetPropertyValue<bool>("PrintRequestConfirmation", ref fPrintRequestConfirmation, value); }
        }

        private bool fPrintOpenDrawer;
        public bool PrintOpenDrawer
        {
            get { return fPrintOpenDrawer; }
            set { SetPropertyValue<bool>("PrintOpenDrawer", ref fPrintOpenDrawer, value); }
        }

        private bool fWayBill;
        public bool WayBill
        {
            get { return fWayBill; }
            set { SetPropertyValue<bool>("WayBill", ref fWayBill, value); }
        }

        //Valid AT WebService Document
        private bool fWsAtDocument;
        public bool WsAtDocument
        {
            get { return fWsAtDocument; }
            set { SetPropertyValue<bool>("WsAtDocument", ref fWsAtDocument, value); }
        }

        //SAF-T PT:Is a SAF-T PT Audit Financial Document
        private bool fSaftAuditFile;
        public bool SaftAuditFile
        {
            get { return fSaftAuditFile; }
            set { SetPropertyValue<bool>("SaftAuditFile", ref fSaftAuditFile, value); }
        }

        //SAF-T PT: SaftDocumentType: SalesInvoices, WorkingDocuments or MovementOfGoods
        private SaftDocumentType fSaftDocumentType;
        public SaftDocumentType SaftDocumentType
        {
            get { return fSaftDocumentType; }
            set { SetPropertyValue<SaftDocumentType>("SaftDocumentType", ref fSaftDocumentType, value); }
        }

        private ProcessArticleStockMode fStockMode;
        public ProcessArticleStockMode StockMode
        {
            get { return fStockMode; }
            set { SetPropertyValue<ProcessArticleStockMode>("StockMode", ref fStockMode, value); }
        }

        //DocumentFinanceType One <> Many ConfigurationPlaceTerminal
        private sys_configurationprinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesDocumentFinanceType")]
        public sys_configurationprinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<sys_configurationprinters>("Printer", ref fPrinter, value); }
        }

        //DocumentFinanceType One <> Many ConfigurationPrintersTemplates
        private sys_configurationprinterstemplates fTemplate;
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