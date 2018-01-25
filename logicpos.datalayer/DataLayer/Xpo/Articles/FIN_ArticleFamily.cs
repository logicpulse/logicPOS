using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_ArticleFamily : XPGuidObject
    {
        public FIN_ArticleFamily() : base() { }
        public FIN_ArticleFamily(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("FIN_ArticleFamily", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("FIN_ArticleFamily", "Code");
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

        string fButtonLabel;
        [Size(35)]
        public string ButtonLabel
        {
            get { return fButtonLabel; }
            set { SetPropertyValue<string>("ButtonLabel", ref fButtonLabel, value); }
        }

        bool fButtonLabelHide;
        public bool ButtonLabelHide
        {
            get { return fButtonLabelHide; }
            set { SetPropertyValue<bool>("ButtonLabelHide", ref fButtonLabelHide, value); }
        }

        string fButtonImage;
        [Size(255)]
        public string ButtonImage
        {
            get { return fButtonImage; }
            set { SetPropertyValue<string>("ButtonImage", ref fButtonImage, value); }
        }

        string fButtonIcon;
        [Size(255)]
        public string ButtonIcon
        {
            get { return fButtonIcon; }
            set { SetPropertyValue<string>("ButtonIcon", ref fButtonIcon, value); }
        }

        //UserCommissionGroup One <> Many Family
        POS_UserCommissionGroup fCommissionGroup;
        [Association(@"UserCommissionGroupReferencesFamily")]
        public POS_UserCommissionGroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue<POS_UserCommissionGroup>("CommissionGroup", ref fCommissionGroup, value); }
        }

        //CustomerDiscountGroup One <> Many SubFamily
        ERP_CustomerDiscountGroup fDiscountGroup;
        [Association(@"CustomerDiscountGroupReferencesFamily")]
        public ERP_CustomerDiscountGroup DiscountGroup
        {
            get { return fDiscountGroup; }
            set { SetPropertyValue<ERP_CustomerDiscountGroup>("DiscountGroup", ref fDiscountGroup, value); }
        }

        //ConfigurationPrinters One <> Many ArticleFamily
        SYS_ConfigurationPrinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesArticleFamily")]
        public SYS_ConfigurationPrinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<SYS_ConfigurationPrinters>("Printer", ref fPrinter, value); }
        }

        //ArticleFamily One <> Many ConfigurationPrintersTemplates
        SYS_ConfigurationPrintersTemplates fTemplate;
        [Association(@"ConfigurationPrintersTemplatesReferencesArticleFamily")]
        public SYS_ConfigurationPrintersTemplates Template
        {
            get { return fTemplate; }
            set { SetPropertyValue<SYS_ConfigurationPrintersTemplates>("Template", ref fTemplate, value); }
        }

        //Family One <> Many Article
        [Association(@"FamilyReferencesArticle", typeof(FIN_Article))]
        public XPCollection<FIN_Article> Article
        {
            get { return GetCollection<FIN_Article>("Article"); }
        }

        //Family One <> Many SubFamily
        [Association(@"FamilyReferencesSubFamily", typeof(FIN_ArticleSubFamily))]
        public XPCollection<FIN_ArticleSubFamily> SubFamily
        {
            get { return GetCollection<FIN_ArticleSubFamily>("SubFamily"); }
        }
    }
}