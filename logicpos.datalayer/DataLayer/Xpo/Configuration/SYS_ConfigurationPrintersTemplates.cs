using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_ConfigurationPrintersTemplates : XPGuidObject
    {
        public SYS_ConfigurationPrintersTemplates() : base() { }
        public SYS_ConfigurationPrintersTemplates(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("SYS_ConfigurationPrintersTemplates", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("SYS_ConfigurationPrintersTemplates", "Code");
        }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        UInt32 fCode;
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

        string fFileTemplate;
        public string FileTemplate
        {
            get { return fFileTemplate; }
            set { SetPropertyValue<string>("FileTemplate", ref fFileTemplate, value); }
        }

        Boolean fFinancialTemplate;
        public Boolean FinancialTemplate
        {
            get { return fFinancialTemplate; }
            set { SetPropertyValue<Boolean>("FinancialTemplate", ref fFinancialTemplate, value); }
        }


        //ConfigurationPrintersTemplates One <> Many Article
        [Association(@"ConfigurationPrintersTemplatesReferencesArticle", typeof(FIN_Article))]
        public XPCollection<FIN_Article> Articles
        {
            get { return GetCollection<FIN_Article>("Articles"); }
        }

        //ConfigurationPrintersTemplates One <> Many Article
        [Association(@"ConfigurationPrintersTemplatesReferencesArticleFamily", typeof(FIN_ArticleFamily))]
        public XPCollection<FIN_Article> ArticlesFamily
        {
            get { return GetCollection<FIN_Article>("ArticlesFamily"); }
        }

        //ConfigurationPrintersTemplates One <> Many ArticleSubFamily
        [Association(@"ConfigurationPrintersTemplatesReferencesArticleSubFamily", typeof(FIN_ArticleSubFamily))]
        public XPCollection<FIN_ArticleSubFamily> ArticlesSubFamily
        {
            get { return GetCollection<FIN_ArticleSubFamily>("ArticlesSubFamily"); }
        }


        //ConfigurationPrintersTemplates One <> Many DocumentFinanceYearSerieTerminal
        [Association(@"ConfigurationPrintersTemplatesReferencesDFYearSerieTerminal", typeof(FIN_DocumentFinanceYearSerieTerminal))]
        public XPCollection<FIN_DocumentFinanceYearSerieTerminal> YearSerieTerminal
        {
            get { return GetCollection<FIN_DocumentFinanceYearSerieTerminal>("YearSerieTerminal"); }
        }

        //ConfigurationPrintersTemplates One <> Many DocumentFinanceType
        [Association(@"ConfigurationPrintersTemplatesReferencesDocumentFinanceType", typeof(FIN_DocumentFinanceType))]
        public XPCollection<FIN_DocumentFinanceType> DocumentsType
        {
            get { return GetCollection<FIN_DocumentFinanceType>("DocumentsType"); }
        }



        //....
        [Association(@"ConfigurationPrintersTemplatesTemplateTicketReferencesTerminal", typeof(POS_ConfigurationPlaceTerminal))]
        public XPCollection<POS_ConfigurationPlaceTerminal> TemplateTicket
        {
            get { return GetCollection<POS_ConfigurationPlaceTerminal>("TemplateTicket"); }
        }



        //.....
        [Association(@"ConfigurationPrintersTemplatesTemplateTablesConsultReferencesTerminal", typeof(POS_ConfigurationPlaceTerminal))]
        public XPCollection<POS_ConfigurationPlaceTerminal> TemplatesTemplateTables
        {
            get { return GetCollection<POS_ConfigurationPlaceTerminal>("TemplatesTemplateTables"); }
        }

    }
}
