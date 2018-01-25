using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_ConfigurationPrinters : XPGuidObject
    {
        public SYS_ConfigurationPrinters() : base() { }
        public SYS_ConfigurationPrinters(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("SYS_ConfigurationPrinters", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("SYS_ConfigurationPrinters", "Code");
            ShowInDialog = true;
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
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        string fNetworkName;
        public string NetworkName
        {
            get { return fNetworkName; }
            set { SetPropertyValue<string>("NetworkName", ref fNetworkName, value); }
        }

        Boolean fShowInDialog;
        public Boolean ShowInDialog
        {
            get { return fShowInDialog; }
            set { SetPropertyValue<Boolean>("ShowInDialog", ref fShowInDialog, value); }
        }

        //ConfigurationPrintersType One <> Many ConfigurationPlace
        SYS_ConfigurationPrintersType fPrinterType;
        [Association(@"ConfigurationPrintersTypeConfigurationPrinters")]
        public SYS_ConfigurationPrintersType PrinterType
        {
            get { return fPrinterType; }
            set { SetPropertyValue<SYS_ConfigurationPrintersType>("PrinterType", ref fPrinterType, value); }
        }

        //ConfigurationPrinters One <> Many CConfigurationPlaceTerminal
        [Association(@"ConfigurationPrintersReferencesConfigurationPlaceTerminal", typeof(POS_ConfigurationPlaceTerminal))]
        public XPCollection<POS_ConfigurationPlaceTerminal> Terminals
        {
            get { return GetCollection<POS_ConfigurationPlaceTerminal>("Terminals"); }
        }

        //ConfigurationPrinters One <> Many DocumentFinanceYearSerieTerminal
        [Association(@"ConfigurationPrintersTerminalReferencesDFYearSerieTerminal", typeof(FIN_DocumentFinanceYearSerieTerminal))]
        public XPCollection<FIN_DocumentFinanceYearSerieTerminal> YearSerieTerminal
        {
            get { return GetCollection<FIN_DocumentFinanceYearSerieTerminal>("YearSerieTerminal"); }
        }

        //ConfigurationPrinters One <> Many Article
        [Association(@"ConfigurationPrintersReferencesArticle", typeof(FIN_Article))]
        public XPCollection<FIN_Article> Article
        {
            get { return GetCollection<FIN_Article>("Article"); }
        }

        //ConfigurationPrinters One <> Many ArticleFamily
        [Association(@"ConfigurationPrintersReferencesArticleFamily", typeof(FIN_ArticleFamily))]
        public XPCollection<FIN_ArticleFamily> ArticleFamily
        {
            get { return GetCollection<FIN_ArticleFamily>("ArticleFamily"); }
        }

        //ConfigurationPrinters One <> Many ArticleSubFamily
        [Association(@"ConfigurationPrintersReferencesArticleSubFamily", typeof(FIN_ArticleSubFamily))]
        public XPCollection<FIN_ArticleSubFamily> ArticleSubFamily
        {
            get { return GetCollection<FIN_ArticleSubFamily>("ArticleSubFamily"); }
        }

        //ConfigurationPrinters One <> Many CConfigurationPlaceTerminal
        [Association(@"ConfigurationPrintersReferencesDocumentFinanceType", typeof(FIN_DocumentFinanceType))]
        public XPCollection<FIN_DocumentFinanceType> DocumentType
        {
            get { return GetCollection<FIN_DocumentFinanceType>("DocumentType"); }
        }
    }
}
