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
            Ord = DataLayerUtils.GetNextTableFieldID("fin_documentfinanceseries", "Ord");
            Code = DataLayerUtils.GetNextTableFieldID("fin_documentfinanceseries", "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private uint fCode;
        //[Indexed(Unique = true)] Series have Duplicated Codes (ex when hidden)
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

        private int fNextDocumentNumber;
        public int NextDocumentNumber
        {
            get { return fNextDocumentNumber; }
            set { SetPropertyValue<int>("NextDocumentNumber", ref fNextDocumentNumber, value); }
        }

        private int fDocumentNumberRangeBegin;
        public int DocumentNumberRangeBegin
        {
            get { return fDocumentNumberRangeBegin; }
            set { SetPropertyValue<int>("DocumentNumberRangeBegin", ref fDocumentNumberRangeBegin, value); }
        }

        private int fDocumentNumberRangeEnd;
        public int DocumentNumberRangeEnd
        {
            get { return fDocumentNumberRangeEnd; }
            set { SetPropertyValue<int>("DocumentNumberRangeEnd", ref fDocumentNumberRangeEnd, value); }
        }

        private string fAcronym;
        [Indexed(Unique = true)]
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        //DocumentFinanceType One <> Many DocumentFinanceSeries
        private fin_documentfinancetype fDocumentType;
        [Association(@"DocumentFinanceTypeReferencesDocumentFinanceSeries")]
        public fin_documentfinancetype DocumentType
        {
            get { return fDocumentType; }
            set { SetPropertyValue<fin_documentfinancetype>("DocumentType", ref fDocumentType, value); }
        }

        //DocumentFinanceType One <> Many DocumentFinanceSeries
        private fin_documentfinanceyears fFiscalYear;
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