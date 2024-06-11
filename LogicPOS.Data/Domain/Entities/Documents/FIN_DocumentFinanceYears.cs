using DevExpress.Xpo;
using LogicPOS.Settings;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Utility;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_documentfinanceyears : Entity
    {
        public fin_documentfinanceyears() : base() { }
        public fin_documentfinanceyears(Session pSession) : base(pSession) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(fin_documentfinanceyears), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(fin_documentfinanceyears), "Code");
            int currentYear = XPOUtility.CurrentDateTimeAtomic().Year;
            FiscalYear = currentYear;
            Acronym = string.Format("{0}{1}{2}", FiscalYear, "A", Code / 10);
            Designation = string.Format("{0} {1} {2}{3}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_fiscal_year"), FiscalYear, "A", Code / 10);
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
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

        private int fFiscalYear;
        //False to Recreate new FiscalYears
        [Indexed(Unique = false)]
        public int FiscalYear
        {
            get { return fFiscalYear; }
            set { SetPropertyValue<int>("FiscalYear", ref fFiscalYear, value); }
        }

        private string fAcronym;
        [Indexed(Unique = true)]
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        private bool fSeriesForEachTerminal;
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
