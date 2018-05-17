using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    public enum ReportAuthorType
    {
        Undefined, System, User
    }

    [DeferredDeletion(false)]
    public class RPT_Report : XPGuidObject
    {
        public RPT_Report() : base() { }
        public RPT_Report(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(RPT_Report), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(RPT_Report), "Code");
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

        string fResourceString;
        [Indexed(Unique = true)]
        public string ResourceString
        {
            get { return fResourceString; }
            set { SetPropertyValue<string>("ResourceString", ref fResourceString, value); }
        }

        string fToken;
        [Indexed(Unique = true)]
        public string Token
        {
            get { return fToken; }
            set { SetPropertyValue<string>("Token", ref fToken, value); }
        }

        string fFileName;
        public string FileName
        {
            get { return fFileName; }
            set { SetPropertyValue<string>("FileName", ref fFileName, value); }
        }

        string fParameterFields;
        public string ParameterFields
        {
            get { return fParameterFields; }
            set { SetPropertyValue<string>("ParameterFields", ref fParameterFields, value); }
        }

        ReportAuthorType fAuthorType;
        public ReportAuthorType AuthorType
        {
            get { return fAuthorType; }
            set { SetPropertyValue<ReportAuthorType>("AuthorType", ref fAuthorType, value); }
        }

        //ReportType One <> Many Report
        RPT_ReportType fReportType;
        [Association(@"ReportTypeReferencesReport")]
        public RPT_ReportType ReportType
        {
            get { return fReportType; }
            set { SetPropertyValue<RPT_ReportType>("ReportType", ref fReportType, value); }
        }
    }
}