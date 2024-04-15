using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_configurationprinterstemplates : XPGuidObject
    {
        public sys_configurationprinterstemplates() : base() { }
        public sys_configurationprinterstemplates(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(sys_configurationprinterstemplates), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(sys_configurationprinterstemplates), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private uint fCode;
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

        private string fFileTemplate;
        public string FileTemplate
        {
            get { return fFileTemplate; }
            set { SetPropertyValue<string>("FileTemplate", ref fFileTemplate, value); }
        }

        private bool fFinancialTemplate;
        public bool FinancialTemplate
        {
            get { return fFinancialTemplate; }
            set { SetPropertyValue<bool>("FinancialTemplate", ref fFinancialTemplate, value); }
        }

        private bool fIsBarCode;
        public bool IsBarCode
        {
            get { return fIsBarCode; }
            set { SetPropertyValue<bool>("IsBarCode", ref fIsBarCode, value); }
        }


        //ConfigurationPrintersTemplates One <> Many Article
        [Association(@"ConfigurationPrintersTemplatesReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Articles
        {
            get { return GetCollection<fin_article>("Articles"); }
        }

        [Association(@"ConfigurationPrintersBarCodeTemplatesReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> ArticlesBarCode
        {
            get { return GetCollection<fin_article>("ArticlesBarCode"); }
        }

        //ConfigurationPrintersTemplates One <> Many Article
        [Association(@"ConfigurationPrintersTemplatesReferencesArticleFamily", typeof(fin_articlefamily))]
        public XPCollection<fin_article> ArticlesFamily
        {
            get { return GetCollection<fin_article>("ArticlesFamily"); }
        }

        //ConfigurationPrintersTemplates One <> Many ArticleSubFamily
        [Association(@"ConfigurationPrintersTemplatesReferencesArticleSubFamily", typeof(fin_articlesubfamily))]
        public XPCollection<fin_articlesubfamily> ArticlesSubFamily
        {
            get { return GetCollection<fin_articlesubfamily>("ArticlesSubFamily"); }
        }


        //ConfigurationPrintersTemplates One <> Many DocumentFinanceYearSerieTerminal
        [Association(@"ConfigurationPrintersTemplatesReferencesDFYearSerieTerminal", typeof(fin_documentfinanceyearserieterminal))]
        public XPCollection<fin_documentfinanceyearserieterminal> YearSerieTerminal
        {
            get { return GetCollection<fin_documentfinanceyearserieterminal>("YearSerieTerminal"); }
        }

        //ConfigurationPrintersTemplates One <> Many DocumentFinanceType
        [Association(@"ConfigurationPrintersTemplatesReferencesDocumentFinanceType", typeof(fin_documentfinancetype))]
        public XPCollection<fin_documentfinancetype> DocumentsType
        {
            get { return GetCollection<fin_documentfinancetype>("DocumentsType"); }
        }


        //....
        [Association(@"ConfigurationPrintersTemplatesTemplateTicketReferencesTerminal", typeof(pos_configurationplaceterminal))]
        public XPCollection<pos_configurationplaceterminal> TemplateTicket
        {
            get { return GetCollection<pos_configurationplaceterminal>("TemplateTicket"); }
        }



        //.....
        [Association(@"ConfigurationPrintersTemplatesTemplateTablesConsultReferencesTerminal", typeof(pos_configurationplaceterminal))]
        public XPCollection<pos_configurationplaceterminal> TemplatesTemplateTables
        {
            get { return GetCollection<pos_configurationplaceterminal>("TemplatesTemplateTables"); }
        }

    }
}
