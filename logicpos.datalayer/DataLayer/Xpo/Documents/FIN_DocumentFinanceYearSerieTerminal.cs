using System;
using DevExpress.Xpo;
using logicpos.datalayer.App;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_DocumentFinanceYearSerieTerminal : XPGuidObject
    {
        public FIN_DocumentFinanceYearSerieTerminal() : base() { }
        public FIN_DocumentFinanceYearSerieTerminal(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(FIN_DocumentFinanceYearSerieTerminal), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(FIN_DocumentFinanceYearSerieTerminal), "Code");
        }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        UInt32 fCode;
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

        //DocumentFinanceYears One <> Many DocumentFinanceYearSerieTerminal
        FIN_DocumentFinanceYears fFiscalYear;
        [Association(@"DocumentFinanceYearsReferencesDocumentFinanceYearSerieTerminal")]
        public FIN_DocumentFinanceYears FiscalYear
        {
            get { return fFiscalYear; }
            set { SetPropertyValue<FIN_DocumentFinanceYears>("FiscalYear", ref fFiscalYear, value); }
        }

        //DocumentFinanceType One <> Many DocumentFinanceYearSerieTerminal
        FIN_DocumentFinanceType fDocumentType;
        [Association(@"DocumentFinanceTypeReferencesDocumentFinanceYearSerieTerminal")]
        public FIN_DocumentFinanceType DocumentType
        {
            get { return fDocumentType; }
            set { SetPropertyValue<FIN_DocumentFinanceType>("DocumentType", ref fDocumentType, value); }
        }


        //DocumentFinanceSeries One <> Many DocumentFinanceYearSerieTerminal
        FIN_DocumentFinanceSeries fDocumentSerie;
        [Association(@"DocumentFinanceSeriesReferencesDFYearSerieTerminal")]
        public FIN_DocumentFinanceSeries Serie
        {
            get { return fDocumentSerie; }
            set { SetPropertyValue<FIN_DocumentFinanceSeries>("Serie", ref fDocumentSerie, value); }
        }


        //DocumentFinanceType One <> Many DocumentFinanceYearSerieTerminal
        POS_ConfigurationPlaceTerminal fTerminal;
        [Association(@"ConfigurationPlaceTerminalReferencesDFYearSerieTerminal")]
        public POS_ConfigurationPlaceTerminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue<POS_ConfigurationPlaceTerminal>("Terminal", ref fTerminal, value); }
        }


        //ConfigurationPrinters One <> Many DocumentFinanceYearSerieTerminal
        SYS_ConfigurationPrinters fPrinter;
        [Association(@"ConfigurationPrintersTerminalReferencesDFYearSerieTerminal")]
        public SYS_ConfigurationPrinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<SYS_ConfigurationPrinters>("Printer", ref fPrinter, value); }
        }


        //ConfigurationPrintersTemplates One <> Many DocumentFinanceYearSerieTerminal
        SYS_ConfigurationPrintersTemplates fTemplate;
        [Association(@"ConfigurationPrintersTemplatesReferencesDFYearSerieTerminal")]
        public SYS_ConfigurationPrintersTemplates Template
        {
            get { return fTemplate; }
            set { SetPropertyValue<SYS_ConfigurationPrintersTemplates>("Template", ref fTemplate, value); }
        }
    }
}