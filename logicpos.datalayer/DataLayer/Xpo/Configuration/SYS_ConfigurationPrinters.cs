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
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(SYS_ConfigurationPrinters), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(SYS_ConfigurationPrinters), "Code");
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

        string fThermalEncoding;
        public string ThermalEncoding
        {
            get { return fThermalEncoding; }
            set { SetPropertyValue<string>("ThermalEncoding", ref fThermalEncoding, value); }
        }

        Boolean fThermalPrintLogo;
        public Boolean ThermalPrintLogo
        {
            get { return fThermalPrintLogo; }
            set { SetPropertyValue<Boolean>("ThermalPrintLogo", ref fThermalPrintLogo, value); }
        }

        string fThermalImageCompanyLogo;
        public string ThermalImageCompanyLogo
        {
            get { return fThermalImageCompanyLogo; }
            set { SetPropertyValue<string>("ThermalImageCompanyLogo", ref fThermalImageCompanyLogo, value); }
        }

        int fThermalMaxCharsPerLineNormal;
        public int ThermalMaxCharsPerLineNormal
        {
            get { return fThermalMaxCharsPerLineNormal; }
            set { SetPropertyValue<int>("ThermalMaxCharsPerLineNormal", ref fThermalMaxCharsPerLineNormal, value); }
        }

        int fThermalMaxCharsPerLineNormalBold;
        public int ThermalMaxCharsPerLineNormalBold
        {
            get { return fThermalMaxCharsPerLineNormalBold; }
            set { SetPropertyValue<int>("ThermalMaxCharsPerLineNormalBold", ref fThermalMaxCharsPerLineNormalBold, value); }
        }

        int fThermalMaxCharsPerLineSmall;
        public int ThermalMaxCharsPerLineSmall
        {
            get { return fThermalMaxCharsPerLineSmall; }
            set { SetPropertyValue<int>("ThermalMaxCharsPerLineSmall", ref fThermalMaxCharsPerLineSmall, value); }
        }

        string fThermalCutCommand;
        public string ThermalCutCommand
        {
            get { return fThermalCutCommand; }
            set { SetPropertyValue<string>("ThermalCutCommand", ref fThermalCutCommand, value); }
        }

        int fThermalOpenDrawerValueM;
        public int ThermalOpenDrawerValueM
        {
            get { return fThermalOpenDrawerValueM; }
            set { SetPropertyValue<int>("ThermalOpenDrawerValueM", ref fThermalOpenDrawerValueM, value); }
        }

        int fThermalOpenDrawerValueT1;
        public int ThermalOpenDrawerValueT1
        {
            get { return fThermalOpenDrawerValueT1; }
            set { SetPropertyValue<int>("ThermalOpenDrawerValueT1", ref fThermalOpenDrawerValueT1, value); }
        }

        int fThermalOpenDrawerValueT2;
        public int ThermalOpenDrawerValueT2
        {
            get { return fThermalOpenDrawerValueT2; }
            set { SetPropertyValue<int>("ThermalOpenDrawerValueT2", ref fThermalOpenDrawerValueT2, value); }
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
