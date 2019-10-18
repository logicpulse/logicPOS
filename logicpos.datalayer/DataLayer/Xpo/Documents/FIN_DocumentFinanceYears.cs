using DevExpress.Xpo;
using logicpos.resources.Resources.Localization;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_documentfinanceyears : XPGuidObject
    {
        public fin_documentfinanceyears() : base() { }
        public fin_documentfinanceyears(Session pSession) : base(pSession) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(fin_documentfinanceyears), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(fin_documentfinanceyears), "Code");
            int currentYear = FrameworkUtils.CurrentDateTimeAtomic().Year;
            FiscalYear = currentYear;
            Acronym = string.Format("{0}{1}{2}", FiscalYear, "A", Code/10);
            Designation = string.Format("{0} {1} {2}{3}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_fiscal_year"), FiscalYear, "A", Code/10);
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
        //False to Recreate new FiscalYears
        [Indexed(Unique = false)]
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
        [Association(@"DocumentFinanceYearsReferencesDocumentFinanceYearSerieTerminal", typeof(fin_documentfinanceyearserieterminal))]
        public XPCollection<fin_documentfinanceyearserieterminal> YearSerieTerminal
        {
            get { return GetCollection<fin_documentfinanceyearserieterminal>("YearSerieTerminal"); }
        }

        //DocumentFinanceYears  One <> Many DocumentFinanceSeries
        [Association(@"DocumentFinanceYearsReferencesDocumentFinanceSeries", typeof(fin_documentfinanceseries))]
        public XPCollection<fin_documentfinanceseries> Series
        {
            get { return GetCollection<fin_documentfinanceseries>("Series"); }
        }
    }
}
