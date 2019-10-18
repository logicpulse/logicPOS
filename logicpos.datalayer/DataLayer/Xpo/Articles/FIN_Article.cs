using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.Enums;
using System;
namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_article : XPGuidObject
    {
        public fin_article() : base() { }
        public fin_article(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(fin_article), "Ord");
            Type = this.Session.GetObjectByKey<fin_articletype>(SettingsApp.XpoOidArticleDefaultType);
            Class = this.Session.GetObjectByKey<fin_articleclass>(SettingsApp.XpoOidArticleDefaultClass);

            if (SettingsApp.AppMode == AppOperationMode.Default)
            {
                //Force users to choose Tax for both modes Normal and TakeAway
                VatOnTable = null;
                VatDirectSelling = null;
            }
            else
            {
                VatOnTable = null;
                VatDirectSelling = this.Session.GetObjectByKey<fin_configurationvatrate>(SettingsApp.XpoOidArticleDefaultVatDirectSelling);
            }

            UnitMeasure = this.Session.GetObjectByKey<cfg_configurationunitmeasure>(SettingsApp.XpoOidArticleDefaultUnitMeasure);
            UnitSize = this.Session.GetObjectByKey<cfg_configurationunitsize>(SettingsApp.XpoOidArticleDefaultUnitSize);
            Template = this.Session.GetObjectByKey<sys_configurationprinterstemplates>(SettingsApp.XpoOidArticleDefaultTemplate);
        }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        string fCode;
        [Indexed(Unique = true)]
        [Size(25)]
        public string Code
        {
            get { return fCode; }
            set { SetPropertyValue<string>("Code", ref fCode, value); }
        }

        string fCodeDealer;
        [Size(25)]
        public string CodeDealer
        {
            get { return fCodeDealer; }
            set { SetPropertyValue<string>("CodeDealer", ref fCodeDealer, value); }
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

        decimal fPrice1;
        public decimal Price1
        {
            get { return fPrice1; }
            set { SetPropertyValue<decimal>("Price1", ref fPrice1, value); }
        }

        decimal fPrice2;
        public decimal Price2
        {
            get { return fPrice2; }
            set { SetPropertyValue<decimal>("Price2", ref fPrice2, value); }
        }

        decimal fPrice3;
        public decimal Price3
        {
            get { return fPrice3; }
            set { SetPropertyValue<decimal>("Price3", ref fPrice3, value); }
        }

        decimal fPrice4;
        public decimal Price4
        {
            get { return fPrice4; }
            set { SetPropertyValue<decimal>("Price4", ref fPrice4, value); }
        }

        decimal fPrice5;
        public decimal Price5
        {
            get { return fPrice5; }
            set { SetPropertyValue<decimal>("Price5", ref fPrice5, value); }
        }

        decimal fPrice1Promotion;
        public decimal Price1Promotion
        {
            get { return fPrice1Promotion; }
            set { SetPropertyValue<decimal>("Price1Promotion", ref fPrice1Promotion, value); }
        }

        decimal fPrice2Promotion;
        public decimal Price2Promotion
        {
            get { return fPrice2Promotion; }
            set { SetPropertyValue<decimal>("Price2Promotion", ref fPrice2Promotion, value); }
        }

        decimal fPrice3Promotion;
        public decimal Price3Promotion
        {
            get { return fPrice3Promotion; }
            set { SetPropertyValue<decimal>("Price3Promotion", ref fPrice3Promotion, value); }
        }

        decimal fPrice4Promotion;
        public decimal Price4Promotion
        {
            get { return fPrice4Promotion; }
            set { SetPropertyValue<decimal>("Price4Promotion", ref fPrice4Promotion, value); }
        }

        decimal fPrice5Promotion;
        public decimal Price5Promotion
        {
            get { return fPrice5Promotion; }
            set { SetPropertyValue<decimal>("Price5Promotion", ref fPrice5Promotion, value); }
        }

        bool fPrice1UsePromotionPrice;
        public bool Price1UsePromotionPrice
        {
            get { return fPrice1UsePromotionPrice; }
            set { SetPropertyValue<bool>("Price1UsePromotionPrice", ref fPrice1UsePromotionPrice, value); }
        }

        bool fPrice2UsePromotionPrice;
        public bool Price2UsePromotionPrice
        {
            get { return fPrice2UsePromotionPrice; }
            set { SetPropertyValue<bool>("Price2UsePromotionPrice", ref fPrice2UsePromotionPrice, value); }
        }

        bool fPrice3UsePromotionPrice;
        public bool Price3UsePromotionPrice
        {
            get { return fPrice3UsePromotionPrice; }
            set { SetPropertyValue<bool>("Price3UsePromotionPrice", ref fPrice3UsePromotionPrice, value); }
        }

        bool fPrice4UsePromotionPrice;
        public bool Price4UsePromotionPrice
        {
            get { return fPrice4UsePromotionPrice; }
            set { SetPropertyValue<bool>("Price4UsePromotionPrice", ref fPrice4UsePromotionPrice, value); }
        }

        bool fPrice5UsePromotionPrice;
        public bool Price5UsePromotionPrice
        {
            get { return fPrice5UsePromotionPrice; }
            set { SetPropertyValue<bool>("Price5UsePromotionPrice", ref fPrice5UsePromotionPrice, value); }
        }

        bool fPriceWithVat;
        public bool PriceWithVat
        {
            get { return fPriceWithVat; }
            set { SetPropertyValue<bool>("PriceWithVat", ref fPriceWithVat, value); }
        }

        decimal fDiscount;
        public decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<decimal>("Discount", ref fDiscount, value); }
        }

        decimal fDefaultQuantity;
        public decimal DefaultQuantity
        {
            get { return fDefaultQuantity; }
            set { SetPropertyValue<decimal>("DefaultQuantity", ref fDefaultQuantity, value); }
        }

        decimal fAccounting;
        public decimal Accounting
        {
            get { return fAccounting; }
            set { SetPropertyValue<decimal>("Accounting", ref fAccounting, value); }
        }

        decimal fTare;
        public decimal Tare
        {
            get { return fTare; }
            set { SetPropertyValue<decimal>("Tare", ref fTare, value); }
        }

        decimal fWeight;
        public decimal Weight
        {
            get { return fWeight; }
            set { SetPropertyValue<decimal>("Weight", ref fWeight, value); }
        }

        string fBarCode;
        [Indexed(Unique = true)]
        public string BarCode
        {
            get { return fBarCode; }
            set { SetPropertyValue<string>("BarCode", ref fBarCode, value); }
        }

        bool fPVPVariable;
        public bool PVPVariable
        {
            get { return fPVPVariable; }
            set { SetPropertyValue<bool>("PVPVariable", ref fPVPVariable, value); }
        }

        bool fFavorite;
        public bool Favorite
        {
            get { return fFavorite; }
            set { SetPropertyValue<bool>("Favorite", ref fFavorite, value); }
        }

        bool fUseWeighingBalance;
        public bool UseWeighingBalance
        {
            get { return fUseWeighingBalance; }
            set { SetPropertyValue<bool>("UseWeighingBalance", ref fUseWeighingBalance, value); }
        }

        string fToken1;
        [Size(255)]
        public string Token1
        {
            get { return fToken1; }
            set { SetPropertyValue<string>("Token1", ref fToken1, value); }
        }

        string fToken2;
        [Size(255)]
        public string Token2
        {
            get { return fToken2; }
            set { SetPropertyValue<string>("Token2", ref fToken2, value); }
        }

//string fToken3;
//[Size(255)]
//public string Token3
//{
//    get { return fToken3; }
//    set { SetPropertyValue<string>("Token3", ref fToken3, value); }
//}

        //Article One <> Many DocumentOrderDetail
        [Association(@"ArticleReferencesDocumentOrderDetail", typeof(fin_documentorderdetail))]
        public XPCollection<fin_documentorderdetail> DocumentOrderDetail
        {
            get { return GetCollection<fin_documentorderdetail>("DocumentOrderDetail"); }
        }

        //Article One <> Many DocumentFinanceDetail
        [Association(@"ArticleReferencesDocumentDocumentFinanceDetail", typeof(fin_documentfinancedetail))]
        public XPCollection<fin_documentfinancedetail> DocumentFinanceDetail
        {
            get { return GetCollection<fin_documentfinancedetail>("DocumentFinanceDetail"); }
        }

        //Family One <> Many Article
        fin_articlefamily fFamily;
        [Association(@"FamilyReferencesArticle")]
        public fin_articlefamily Family
        {
            get { return fFamily; }
            set { SetPropertyValue<fin_articlefamily>("Family", ref fFamily, value); }
        }

        //SubFamily One <> Many Article
        fin_articlesubfamily fSubFamily;
        [Association(@"SubFamilyReferencesArticle")]
        public fin_articlesubfamily SubFamily
        {
            get { return fSubFamily; }
            set { SetPropertyValue<fin_articlesubfamily>("SubFamily", ref fSubFamily, value); }
        }

        //ArticleType One <> Many Article
        fin_articletype fType;
        [Association(@"ArticleTypeReferencesArticle")]
        public fin_articletype Type
        {
            get { return fType; }
            set { SetPropertyValue<fin_articletype>("Type", ref fType, value); }
        }

        //ArticleClass One <> Many Article
        fin_articleclass fClass;
        [Association(@"ArticleClassReferencesArticle")]
        public fin_articleclass Class
        {
            get { return fClass; }
            set { SetPropertyValue<fin_articleclass>("Class", ref fClass, value); }
        }

        //configurationUnitMeasure One <> Many Article
        cfg_configurationunitmeasure fUnitMeasure;
        [Association(@"ConfigurationUnitMeasureReferencesArticle")]
        public cfg_configurationunitmeasure UnitMeasure
        {
            get { return fUnitMeasure; }
            set { SetPropertyValue<cfg_configurationunitmeasure>("UnitMeasure", ref fUnitMeasure, value); }
        }

        //ConfigurationUnitSize One <> Many Article
        cfg_configurationunitsize fUnitSize;
        [Association(@"ConfigurationUnitSizeReferencesArticle")]
        public cfg_configurationunitsize UnitSize
        {
            get { return fUnitSize; }
            set { SetPropertyValue<cfg_configurationunitsize>("UnitSize", ref fUnitSize, value); }
        }

        //UserCommissionGroup One <> Many Article
        pos_usercommissiongroup fCommissionGroup;
        [Association(@"UserCommissionGroupReferencesArticle")]
        public pos_usercommissiongroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue<pos_usercommissiongroup>("CommissionGroup", ref fCommissionGroup, value); }
        }

        //CustomerDiscountGroup One <> Many Article
        erp_customerdiscountgroup fDiscountGroup;
        [Association(@"CustomerDiscountGroupReferencesArticle")]
        public erp_customerdiscountgroup DiscountGroup
        {
            get { return fDiscountGroup; }
            set { SetPropertyValue<erp_customerdiscountgroup>("DiscountGroup", ref fDiscountGroup, value); }
        }

        //ConfigurationVatRate One <> Many Article
        fin_configurationvatrate fVatOnTable;
        [Association(@"ConfigurationVatRateReferencesArticle_ForVatOnTable")]
        public fin_configurationvatrate VatOnTable
        {
            get { return fVatOnTable; }
            set { SetPropertyValue<fin_configurationvatrate>("VatTOnTable", ref fVatOnTable, value); }
        }

        //ConfigurationVatRate One <> Many Article
        fin_configurationvatrate fVatDirectSelling;
        [Association(@"ConfigurationVatRateReferencesArticle_ForVatDirectSelling")]
        public fin_configurationvatrate VatDirectSelling
        {
            get { return fVatDirectSelling; }
            set { SetPropertyValue<fin_configurationvatrate>("VatDirectSelling", ref fVatDirectSelling, value); }
        }

        //ConfigurationVatExemptionReason One <> Many Article
        fin_configurationvatexemptionreason fVatExemptionReason;
        [Association(@"ConfigurationVatExemptionReasonReferencesArticle")]
        public fin_configurationvatexemptionreason VatExemptionReason
        {
            get { return fVatExemptionReason; }
            set { SetPropertyValue<fin_configurationvatexemptionreason>("VatExemptionReason", ref fVatExemptionReason, value); }
        }

        //ConfigurationDevice One <> Many Article
        sys_configurationprinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesArticle")]
        public sys_configurationprinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<sys_configurationprinters>("Printer", ref fPrinter, value); }
        }

        //DocumentFinanceType One <> Many ConfigurationPrintersTemplates
        sys_configurationprinterstemplates fTemplate;
        [Association(@"ConfigurationPrintersTemplatesReferencesArticle")]
        public sys_configurationprinterstemplates Template
        {
            get { return fTemplate; }
            set { SetPropertyValue<sys_configurationprinterstemplates>("Template", ref fTemplate, value); }
        }
    }
}