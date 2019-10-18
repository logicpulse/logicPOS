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
    public class rpt_report : XPGuidObject
    {
        public rpt_report() : base() { }
        public rpt_report(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(rpt_report), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(rpt_report), "Code");
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
        rpt_reporttype fReportType;
        [Association(@"ReportTypeReferencesReport")]
        public rpt_reporttype ReportType
        {
            get { return fReportType; }
            set { SetPropertyValue<rpt_reporttype>("ReportType", ref fReportType, value); }
        }
    }
}