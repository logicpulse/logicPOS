using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Domain.Entities
{

    [DeferredDeletion(false)]
    public class rpt_report : XPGuidObject
    {
        public rpt_report() : base() { }
        public rpt_report(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(rpt_report), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(rpt_report), "Code");
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

        private string fResourceString;
        [Indexed(Unique = true)]
        public string ResourceString
        {
            get { return fResourceString; }
            set { SetPropertyValue<string>("ResourceString", ref fResourceString, value); }
        }

        private string fToken;
        [Indexed(Unique = true)]
        public string Token
        {
            get { return fToken; }
            set { SetPropertyValue<string>("Token", ref fToken, value); }
        }

        private string fFileName;
        public string FileName
        {
            get { return fFileName; }
            set { SetPropertyValue<string>("FileName", ref fFileName, value); }
        }

        private string fParameterFields;
        public string ParameterFields
        {
            get { return fParameterFields; }
            set { SetPropertyValue<string>("ParameterFields", ref fParameterFields, value); }
        }

        private ReportAuthorType fAuthorType;
        public ReportAuthorType AuthorType
        {
            get { return fAuthorType; }
            set { SetPropertyValue("AuthorType", ref fAuthorType, value); }
        }

        //ReportType One <> Many Report
        private rpt_reporttype fReportType;
        [Association(@"ReportTypeReferencesReport")]
        public rpt_reporttype ReportType
        {
            get { return fReportType; }
            set { SetPropertyValue("ReportType", ref fReportType, value); }
        }
    }
}