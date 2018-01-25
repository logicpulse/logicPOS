using DevExpress.Xpo;
using logicpos.resources.Resources.Localization;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_DocumentFinanceYears : XPGuidObject
    {
        public FIN_DocumentFinanceYears() : base() { }
        public FIN_DocumentFinanceYears(Session pSession) : base(pSession) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("FIN_DocumentFinanceYears", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("FIN_DocumentFinanceYears", "Code");
            int currentYear = FrameworkUtils.CurrentDateTimeAtomic().Year;
            FiscalYear = currentYear;
            Acronym = string.Format("{0}{1}", FiscalYear, "S");
            Designation = string.Format("{0} {1}", Resx.global_fiscal_year, FiscalYear);
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

        int fFiscalYear;
        [Indexed(Unique = true)]
        public int FiscalYear
        {
            get { return fFiscalYear; }
            set { SetPropertyValue<int>("FiscalYear", ref fFiscalYear, value); }
        }

        string fAcronym;
        [Indexed(Unique = true)]
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        bool fSeriesForEachTerminal;
        public bool SeriesForEachTerminal
        {
            get { return fSeriesForEachTerminal; }
            set { SetPropertyValue<bool>("SeriesForEachTerminal", ref fSeriesForEachTerminal, value); }
        }

        //ConfigurationPlaceTerminal  One <> Many DocumentFinanceYearSerieTerminal
        [Association(@"DocumentFinanceYearsReferencesDocumentFinanceYearSerieTerminal", typeof(FIN_DocumentFinanceYearSerieTerminal))]
        public XPCollection<FIN_DocumentFinanceYearSerieTerminal> YearSerieTerminal
        {
            get { return GetCollection<FIN_DocumentFinanceYearSerieTerminal>("YearSerieTerminal"); }
        }

        //DocumentFinanceYears  One <> Many DocumentFinanceSeries
        [Association(@"DocumentFinanceYearsReferencesDocumentFinanceSeries", typeof(FIN_DocumentFinanceSeries))]
        public XPCollection<FIN_DocumentFinanceSeries> Series
        {
            get { return GetCollection<FIN_DocumentFinanceSeries>("Series"); }
        }
    }
}
