using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_documentfinanceyearserieterminal : Entity
    {
        public fin_documentfinanceyearserieterminal() : base() { }
        public fin_documentfinanceyearserieterminal(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(fin_documentfinanceyearserieterminal), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(fin_documentfinanceyearserieterminal), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private uint fCode;
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        //DocumentFinanceYears One <> Many DocumentFinanceYearSerieTerminal
        private fin_documentfinanceyears fFiscalYear;
        [Association(@"DocumentFinanceYearsReferencesDocumentFinanceYearSerieTerminal")]
        public fin_documentfinanceyears FiscalYear
        {
            get { return fFiscalYear; }
            set { SetPropertyValue("FiscalYear", ref fFiscalYear, value); }
        }

        //DocumentFinanceType One <> Many DocumentFinanceYearSerieTerminal
        private fin_documentfinancetype fDocumentType;
        [Association(@"DocumentFinanceTypeReferencesDocumentFinanceYearSerieTerminal")]
        public fin_documentfinancetype DocumentType
        {
            get { return fDocumentType; }
            set { SetPropertyValue("DocumentType", ref fDocumentType, value); }
        }


        //DocumentFinanceSeries One <> Many DocumentFinanceYearSerieTerminal
        private fin_documentfinanceseries fDocumentSerie;
        [Association(@"DocumentFinanceSeriesReferencesDFYearSerieTerminal")]
        public fin_documentfinanceseries Serie
        {
            get { return fDocumentSerie; }
            set { SetPropertyValue("Serie", ref fDocumentSerie, value); }
        }


        //DocumentFinanceType One <> Many DocumentFinanceYearSerieTerminal
        private pos_configurationplaceterminal fTerminal;
        [Association(@"ConfigurationPlaceTerminalReferencesDFYearSerieTerminal")]
        public pos_configurationplaceterminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue("Terminal", ref fTerminal, value); }
        }


        //ConfigurationPrinters One <> Many DocumentFinanceYearSerieTerminal
        private sys_configurationprinters fPrinter;
        [Association(@"ConfigurationPrintersTerminalReferencesDFYearSerieTerminal")]
        public sys_configurationprinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue("Printer", ref fPrinter, value); }
        }


        //ConfigurationPrintersTemplates One <> Many DocumentFinanceYearSerieTerminal
        private sys_configurationprinterstemplates fTemplate;
        [Association(@"ConfigurationPrintersTemplatesReferencesDFYearSerieTerminal")]
        public sys_configurationprinterstemplates Template
        {
            get { return fTemplate; }
            set { SetPropertyValue("Template", ref fTemplate, value); }
        }
    }
}