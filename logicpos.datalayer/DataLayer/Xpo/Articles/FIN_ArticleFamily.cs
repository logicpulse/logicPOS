using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_articlefamily : XPGuidObject
    {
        public fin_articlefamily() : base() { }
        public fin_articlefamily(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(fin_articlefamily), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(fin_articlefamily), "Code");
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
        pos_usercommissiongroup fCommissionGroup;
        [Association(@"UserCommissionGroupReferencesFamily")]
        public pos_usercommissiongroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue<pos_usercommissiongroup>("CommissionGroup", ref fCommissionGroup, value); }
        }

        //CustomerDiscountGroup One <> Many SubFamily
        erp_customerdiscountgroup fDiscountGroup;
        [Association(@"CustomerDiscountGroupReferencesFamily")]
        public erp_customerdiscountgroup DiscountGroup
        {
            get { return fDiscountGroup; }
            set { SetPropertyValue<erp_customerdiscountgroup>("DiscountGroup", ref fDiscountGroup, value); }
        }

        //ConfigurationPrinters One <> Many ArticleFamily
        sys_configurationprinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesArticleFamily")]
        public sys_configurationprinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<sys_configurationprinters>("Printer", ref fPrinter, value); }
        }

        //ArticleFamily One <> Many ConfigurationPrintersTemplates
        sys_configurationprinterstemplates fTemplate;
        [Association(@"ConfigurationPrintersTemplatesReferencesArticleFamily")]
        public sys_configurationprinterstemplates Template
        {
            get { return fTemplate; }
            set { SetPropertyValue<sys_configurationprinterstemplates>("Template", ref fTemplate, value); }
        }

        //Family One <> Many Article
        [Association(@"FamilyReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Article
        {
            get { return GetCollection<fin_article>("Article"); }
        }

        //Family One <> Many SubFamily
        [Association(@"FamilyReferencesSubFamily", typeof(fin_articlesubfamily))]
        public XPCollection<fin_articlesubfamily> SubFamily
        {
            get { return GetCollection<fin_articlesubfamily>("SubFamily"); }
        }
    }
}