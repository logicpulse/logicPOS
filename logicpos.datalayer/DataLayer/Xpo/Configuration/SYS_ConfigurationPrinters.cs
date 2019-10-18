using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_configurationprinters : XPGuidObject
    {
        public sys_configurationprinters() : base() { }
        public sys_configurationprinters(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(sys_configurationprinters), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(sys_configurationprinters), "Code");
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
        sys_configurationprinterstype fPrinterType;
        [Association(@"ConfigurationPrintersTypeConfigurationPrinters")]
        public sys_configurationprinterstype PrinterType
        {
            get { return fPrinterType; }
            set { SetPropertyValue<sys_configurationprinterstype>("PrinterType", ref fPrinterType, value); }
        }

        // Impressoras - Diferenciação entre Tipos TK016249
        //ConfigurationPrinters One <> Many CConfigurationPlaceTerminal
        [Association(@"ConfigurationPrintersReferencesConfigurationPlaceTerminal", typeof(pos_configurationplaceterminal))]
        public XPCollection<pos_configurationplaceterminal> Terminals
        {
            get { return GetCollection<pos_configurationplaceterminal>("Terminals"); }
        }
		
        //ConfigurationPrinters One <> Many CConfigurationPlaceTerminal
        [Association(@"ConfigurationThermalPrintersReferencesConfigurationPlaceTerminal", typeof(pos_configurationplaceterminal))]
        public XPCollection<pos_configurationplaceterminal> ThermalPrinter
        {
            get { return GetCollection<pos_configurationplaceterminal>("ThermalPrinter"); }
        }

        //ConfigurationPrinters One <> Many DocumentFinanceYearSerieTerminal
        [Association(@"ConfigurationPrintersTerminalReferencesDFYearSerieTerminal", typeof(fin_documentfinanceyearserieterminal))]
        public XPCollection<fin_documentfinanceyearserieterminal> YearSerieTerminal
        {
            get { return GetCollection<fin_documentfinanceyearserieterminal>("YearSerieTerminal"); }
        }

        //ConfigurationPrinters One <> Many Article
        [Association(@"ConfigurationPrintersReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Article
        {
            get { return GetCollection<fin_article>("Article"); }
        }

        //ConfigurationPrinters One <> Many ArticleFamily
        [Association(@"ConfigurationPrintersReferencesArticleFamily", typeof(fin_articlefamily))]
        public XPCollection<fin_articlefamily> ArticleFamily
        {
            get { return GetCollection<fin_articlefamily>("ArticleFamily"); }
        }

        //ConfigurationPrinters One <> Many ArticleSubFamily
        [Association(@"ConfigurationPrintersReferencesArticleSubFamily", typeof(fin_articlesubfamily))]
        public XPCollection<fin_articlesubfamily> ArticleSubFamily
        {
            get { return GetCollection<fin_articlesubfamily>("ArticleSubFamily"); }
        }

        //ConfigurationPrinters One <> Many CConfigurationPlaceTerminal
        [Association(@"ConfigurationPrintersReferencesDocumentFinanceType", typeof(fin_documentfinancetype))]
        public XPCollection<fin_documentfinancetype> DocumentType
        {
            get { return GetCollection<fin_documentfinancetype>("DocumentType"); }
        }
    }
}
