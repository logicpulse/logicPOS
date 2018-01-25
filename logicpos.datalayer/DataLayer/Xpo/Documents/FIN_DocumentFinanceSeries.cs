using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_DocumentFinanceSeries : XPGuidObject
    {
        public FIN_DocumentFinanceSeries() : base() { }
        public FIN_DocumentFinanceSeries(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("FIN_DocumentFinanceSeries", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("FIN_DocumentFinanceSeries", "Code");
        }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        UInt32 fCode;
        //[Indexed(Unique = true)] Series have Duplicated Codes (ex when hidden)
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

        Int32 fNextDocumentNumber;
        public Int32 NextDocumentNumber
        {
            get { return fNextDocumentNumber; }
            set { SetPropertyValue<Int32>("NextDocumentNumber", ref fNextDocumentNumber, value); }
        }

        Int32 fDocumentNumberRangeBegin;
        public Int32 DocumentNumberRangeBegin
        {
            get { return fDocumentNumberRangeBegin; }
            set { SetPropertyValue<Int32>("DocumentNumberRangeBegin", ref fDocumentNumberRangeBegin, value); }
        }
        Int32 fDocumentNumberRangeEnd;
        public Int32 DocumentNumberRangeEnd
        {
            get { return fDocumentNumberRangeEnd; }
            set { SetPropertyValue<Int32>("DocumentNumberRangeEnd", ref fDocumentNumberRangeEnd, value); }
        }

        string fAcronym;
        [Indexed(Unique = true)]
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        //DocumentFinanceType One <> Many DocumentFinanceSeries
        FIN_DocumentFinanceType fDocumentType;
        [Association(@"DocumentFinanceTypeReferencesDocumentFinanceSeries")]
        public FIN_DocumentFinanceType DocumentType
        {
            get { return fDocumentType; }
            set { SetPropertyValue<FIN_DocumentFinanceType>("DocumentType", ref fDocumentType, value); }
        }

        //DocumentFinanceType One <> Many DocumentFinanceSeries
        FIN_DocumentFinanceYears fFiscalYear;
        [Association(@"DocumentFinanceYearsReferencesDocumentFinanceSeries")]
        public FIN_DocumentFinanceYears FiscalYear
        {
            get { return fFiscalYear; }
            set { SetPropertyValue<FIN_DocumentFinanceYears>("FiscalYear", ref fFiscalYear, value); }
        }

        //DocumentFinanceSeries One <> Many DocumentFinanceYearSerieTerminal
        [Association(@"DocumentFinanceSeriesReferencesDFYearSerieTerminal", typeof(FIN_DocumentFinanceYearSerieTerminal))]
        public XPCollection<FIN_DocumentFinanceYearSerieTerminal> YearSerieTerminal
        {
            get { return GetCollection<FIN_DocumentFinanceYearSerieTerminal>("YearSerieTerminal"); }
        }

        //DocumentFinanceSeries One <> Many DocumentFinanceMaster
        [Association(@"DocumentFinanceSeriesReferencesDocumentFinanceMaster", typeof(FIN_DocumentFinanceMaster))]
        public XPCollection<FIN_DocumentFinanceMaster> DocumentMaster
        {
            get { return GetCollection<FIN_DocumentFinanceMaster>("DocumentMaster"); }
        }

        //DocumentFinanceSeries One <> Many DocumentFinancePayment
        [Association(@"DocumentFinanceSeriesReferencesDocumentFinancePayment", typeof(FIN_DocumentFinancePayment))]
        public XPCollection<FIN_DocumentFinanceMaster> DocumentPayment
        {
            get { return GetCollection<FIN_DocumentFinanceMaster>("DocumentPayment"); }
        }
    }
}