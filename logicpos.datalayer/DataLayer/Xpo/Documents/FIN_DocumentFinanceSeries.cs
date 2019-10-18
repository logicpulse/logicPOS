using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_documentfinanceseries : XPGuidObject
    {
        public fin_documentfinanceseries() : base() { }
        public fin_documentfinanceseries(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("fin_documentfinanceseries", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("fin_documentfinanceseries", "Code");
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
        fin_documentfinancetype fDocumentType;
        [Association(@"DocumentFinanceTypeReferencesDocumentFinanceSeries")]
        public fin_documentfinancetype DocumentType
        {
            get { return fDocumentType; }
            set { SetPropertyValue<fin_documentfinancetype>("DocumentType", ref fDocumentType, value); }
        }

        //DocumentFinanceType One <> Many DocumentFinanceSeries
        fin_documentfinanceyears fFiscalYear;
        [Association(@"DocumentFinanceYearsReferencesDocumentFinanceSeries")]
        public fin_documentfinanceyears FiscalYear
        {
            get { return fFiscalYear; }
            set { SetPropertyValue<fin_documentfinanceyears>("FiscalYear", ref fFiscalYear, value); }
        }

        //DocumentFinanceSeries One <> Many DocumentFinanceYearSerieTerminal
        [Association(@"DocumentFinanceSeriesReferencesDFYearSerieTerminal", typeof(fin_documentfinanceyearserieterminal))]
        public XPCollection<fin_documentfinanceyearserieterminal> YearSerieTerminal
        {
            get { return GetCollection<fin_documentfinanceyearserieterminal>("YearSerieTerminal"); }
        }

        //DocumentFinanceSeries One <> Many DocumentFinanceMaster
        [Association(@"DocumentFinanceSeriesReferencesDocumentFinanceMaster", typeof(fin_documentfinancemaster))]
        public XPCollection<fin_documentfinancemaster> DocumentMaster
        {
            get { return GetCollection<fin_documentfinancemaster>("DocumentMaster"); }
        }

        //DocumentFinanceSeries One <> Many DocumentFinancePayment
        [Association(@"DocumentFinanceSeriesReferencesDocumentFinancePayment", typeof(fin_documentfinancepayment))]
        public XPCollection<fin_documentfinancemaster> DocumentPayment
        {
            get { return GetCollection<fin_documentfinancemaster>("DocumentPayment"); }
        }
    }
}