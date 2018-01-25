using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_ArticleSubFamily : XPGuidObject
    {
        public FIN_ArticleSubFamily() : base() { }
        public FIN_ArticleSubFamily(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("FIN_ArticleSubFamily", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("FIN_ArticleSubFamily", "Code");
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

        //SubFamily One <> Many Article
        [Association(@"SubFamilyReferencesArticle", typeof(FIN_Article))]
        public XPCollection<FIN_Article> Article
        {
            get { return GetCollection<FIN_Article>("Article"); }
        }

        //Family One <> Many SubFamily
        FIN_ArticleFamily fFamily;
        [Association(@"FamilyReferencesSubFamily")]
        public FIN_ArticleFamily Family
        {
            get { return fFamily; }
            set { SetPropertyValue<FIN_ArticleFamily>("Family", ref fFamily, value); }
        }

        //UserCommissionGroup One <> Many SubFamily
        POS_UserCommissionGroup fCommissionGroup;
        [Association(@"UserCommissionGroupReferencesSubFamily")]
        public POS_UserCommissionGroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue<POS_UserCommissionGroup>("CommissionGroup", ref fCommissionGroup, value); }
        }

        //CustomerDiscountGroup One <> Many SubFamily
        ERP_CustomerDiscountGroup fDiscountGroup;
        [Association(@"CustomerDiscountGroupReferencesSubFamily")]
        public ERP_CustomerDiscountGroup DiscountGroup
        {
            get { return fDiscountGroup; }
            set { SetPropertyValue<ERP_CustomerDiscountGroup>("DiscountGroup", ref fDiscountGroup, value); }
        }

        //ConfigurationVatRate One <> Many SubFamily
        FIN_ConfigurationVatRate fVatOnTable;
        [Association(@"ConfigurationVatRateReferencesSubFamily_ForVatOnTable")]
        public FIN_ConfigurationVatRate VatOnTable
        {
            get { return fVatOnTable; }
            set { SetPropertyValue<FIN_ConfigurationVatRate>("VatOnTable", ref fVatOnTable, value); }
        }

        //ConfigurationVatRate One <> Many SubFamily
        FIN_ConfigurationVatRate fVatDirectSelling;
        [Association(@"ConfigurationVatRateReferencesSubFamily_ForVatDirectSelling")]
        public FIN_ConfigurationVatRate VatDirectSelling
        {
            get { return fVatDirectSelling; }
            set { SetPropertyValue<FIN_ConfigurationVatRate>("VatDirectSelling", ref fVatDirectSelling, value); }
        }

        //ConfigurationPrinters One <> Many SubFamily
        SYS_ConfigurationPrinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesArticleSubFamily")]
        public SYS_ConfigurationPrinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<SYS_ConfigurationPrinters>("Printer", ref fPrinter, value); }
        }

        //One ConfigurationPrintersTemplates <> Many SubFamily
        SYS_ConfigurationPrintersTemplates fTemplate;
        [Association(@"ConfigurationPrintersTemplatesReferencesArticleSubFamily")]
        public SYS_ConfigurationPrintersTemplates Template
        {
            get { return fTemplate; }
            set { SetPropertyValue<SYS_ConfigurationPrintersTemplates>("Template", ref fTemplate, value); }
        }
    }
}