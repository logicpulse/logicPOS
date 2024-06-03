using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_articlesubfamily : Entity
    {
        public fin_articlesubfamily() : base() { }
        public fin_articlesubfamily(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(fin_articlesubfamily), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(fin_articlesubfamily), "Code");
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
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fButtonLabel;
        [Size(35)]
        public string ButtonLabel
        {
            get { return fButtonLabel; }
            set { SetPropertyValue<string>("ButtonLabel", ref fButtonLabel, value); }
        }

        private bool fButtonLabelHide;
        public bool ButtonLabelHide
        {
            get { return fButtonLabelHide; }
            set { SetPropertyValue<bool>("ButtonLabelHide", ref fButtonLabelHide, value); }
        }

        private string fButtonImage;
        [Size(255)]
        public string ButtonImage
        {
            get { return fButtonImage; }
            set { SetPropertyValue<string>("ButtonImage", ref fButtonImage, value); }
        }

        private string fButtonIcon;
        [Size(255)]
        public string ButtonIcon
        {
            get { return fButtonIcon; }
            set { SetPropertyValue<string>("ButtonIcon", ref fButtonIcon, value); }
        }

        //SubFamily One <> Many Article
        [Association(@"SubFamilyReferencesArticle", typeof(fin_article))]
        public XPCollection<fin_article> Article
        {
            get { return GetCollection<fin_article>("Article"); }
        }

        //Family One <> Many SubFamily
        private fin_articlefamily fFamily;
        [Association(@"FamilyReferencesSubFamily")]
        public fin_articlefamily Family
        {
            get { return fFamily; }
            set { SetPropertyValue("Family", ref fFamily, value); }
        }

        //UserCommissionGroup One <> Many SubFamily
        private pos_usercommissiongroup fCommissionGroup;
        [Association(@"UserCommissionGroupReferencesSubFamily")]
        public pos_usercommissiongroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue("CommissionGroup", ref fCommissionGroup, value); }
        }

        //CustomerDiscountGroup One <> Many SubFamily
        private erp_customerdiscountgroup fDiscountGroup;
        [Association(@"CustomerDiscountGroupReferencesSubFamily")]
        public erp_customerdiscountgroup DiscountGroup
        {
            get { return fDiscountGroup; }
            set { SetPropertyValue("DiscountGroup", ref fDiscountGroup, value); }
        }

        //ConfigurationVatRate One <> Many SubFamily
        private fin_configurationvatrate fVatOnTable;
        [Association(@"ConfigurationVatRateReferencesSubFamily_ForVatOnTable")]
        public fin_configurationvatrate VatOnTable
        {
            get { return fVatOnTable; }
            set { SetPropertyValue("VatOnTable", ref fVatOnTable, value); }
        }

        //ConfigurationVatRate One <> Many SubFamily
        private fin_configurationvatrate fVatDirectSelling;
        [Association(@"ConfigurationVatRateReferencesSubFamily_ForVatDirectSelling")]
        public fin_configurationvatrate VatDirectSelling
        {
            get { return fVatDirectSelling; }
            set { SetPropertyValue("VatDirectSelling", ref fVatDirectSelling, value); }
        }

        //ConfigurationPrinters One <> Many SubFamily
        private sys_configurationprinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesArticleSubFamily")]
        public sys_configurationprinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue("Printer", ref fPrinter, value); }
        }

        //One ConfigurationPrintersTemplates <> Many SubFamily
        private sys_configurationprinterstemplates fTemplate;
        [Association(@"ConfigurationPrintersTemplatesReferencesArticleSubFamily")]
        public sys_configurationprinterstemplates Template
        {
            get { return fTemplate; }
            set { SetPropertyValue("Template", ref fTemplate, value); }
        }
    }
}