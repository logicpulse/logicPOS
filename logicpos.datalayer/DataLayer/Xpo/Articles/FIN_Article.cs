using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo.Articles;
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

        private UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private string fCode;
        [Indexed(Unique = true)]
        [Size(25)]
        public string Code
        {
            get { return fCode; }
            set { SetPropertyValue<string>("Code", ref fCode, value); }
        }

        private string fCodeDealer;
        [Size(25)]
        public string CodeDealer
        {
            get { return fCodeDealer; }
            set { SetPropertyValue<string>("CodeDealer", ref fCodeDealer, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
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

        private decimal fPrice1;
        public decimal Price1
        {
            get { return fPrice1; }
            set { SetPropertyValue<decimal>("Price1", ref fPrice1, value); }
        }

        private decimal fPrice2;
        public decimal Price2
        {
            get { return fPrice2; }
            set { SetPropertyValue<decimal>("Price2", ref fPrice2, value); }
        }

        private decimal fPrice3;
        public decimal Price3
        {
            get { return fPrice3; }
            set { SetPropertyValue<decimal>("Price3", ref fPrice3, value); }
        }

        private decimal fPrice4;
        public decimal Price4
        {
            get { return fPrice4; }
            set { SetPropertyValue<decimal>("Price4", ref fPrice4, value); }
        }

        private decimal fPrice5;
        public decimal Price5
        {
            get { return fPrice5; }
            set { SetPropertyValue<decimal>("Price5", ref fPrice5, value); }
        }

        private decimal fPrice1Promotion;
        public decimal Price1Promotion
        {
            get { return fPrice1Promotion; }
            set { SetPropertyValue<decimal>("Price1Promotion", ref fPrice1Promotion, value); }
        }

        private decimal fPrice2Promotion;
        public decimal Price2Promotion
        {
            get { return fPrice2Promotion; }
            set { SetPropertyValue<decimal>("Price2Promotion", ref fPrice2Promotion, value); }
        }

        private decimal fPrice3Promotion;
        public decimal Price3Promotion
        {
            get { return fPrice3Promotion; }
            set { SetPropertyValue<decimal>("Price3Promotion", ref fPrice3Promotion, value); }
        }

        private decimal fPrice4Promotion;
        public decimal Price4Promotion
        {
            get { return fPrice4Promotion; }
            set { SetPropertyValue<decimal>("Price4Promotion", ref fPrice4Promotion, value); }
        }

        private decimal fPrice5Promotion;
        public decimal Price5Promotion
        {
            get { return fPrice5Promotion; }
            set { SetPropertyValue<decimal>("Price5Promotion", ref fPrice5Promotion, value); }
        }

        private bool fPrice1UsePromotionPrice;
        public bool Price1UsePromotionPrice
        {
            get { return fPrice1UsePromotionPrice; }
            set { SetPropertyValue<bool>("Price1UsePromotionPrice", ref fPrice1UsePromotionPrice, value); }
        }

        private bool fPrice2UsePromotionPrice;
        public bool Price2UsePromotionPrice
        {
            get { return fPrice2UsePromotionPrice; }
            set { SetPropertyValue<bool>("Price2UsePromotionPrice", ref fPrice2UsePromotionPrice, value); }
        }

        private bool fPrice3UsePromotionPrice;
        public bool Price3UsePromotionPrice
        {
            get { return fPrice3UsePromotionPrice; }
            set { SetPropertyValue<bool>("Price3UsePromotionPrice", ref fPrice3UsePromotionPrice, value); }
        }

        private bool fPrice4UsePromotionPrice;
        public bool Price4UsePromotionPrice
        {
            get { return fPrice4UsePromotionPrice; }
            set { SetPropertyValue<bool>("Price4UsePromotionPrice", ref fPrice4UsePromotionPrice, value); }
        }

        private bool fPrice5UsePromotionPrice;
        public bool Price5UsePromotionPrice
        {
            get { return fPrice5UsePromotionPrice; }
            set { SetPropertyValue<bool>("Price5UsePromotionPrice", ref fPrice5UsePromotionPrice, value); }
        }

        private bool fPriceWithVat;
        public bool PriceWithVat
        {
            get { return fPriceWithVat; }
            set { SetPropertyValue<bool>("PriceWithVat", ref fPriceWithVat, value); }
        }

        private decimal fDiscount;
        public decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<decimal>("Discount", ref fDiscount, value); }
        }

        private decimal fDefaultQuantity;
        public decimal DefaultQuantity
        {
            get { return fDefaultQuantity; }
            set { SetPropertyValue<decimal>("DefaultQuantity", ref fDefaultQuantity, value); }
        }

        private decimal fAccounting;
        public decimal Accounting
        {
            get { return fAccounting; }
            set { SetPropertyValue<decimal>("Accounting", ref fAccounting, value); }
        }

        //Gestão de Stocks : Janela de Gestão de Stocks [IN:016534]
        private decimal fMinimumStock;
        public decimal MinimumStock
        {
            get { return fMinimumStock; }
            set { SetPropertyValue<decimal>("MinimumStock", ref fMinimumStock, value); }
        }

        private decimal fTare;
        public decimal Tare
        {
            get { return fTare; }
            set { SetPropertyValue<decimal>("Tare", ref fTare, value); }
        }

        private decimal fWeight;
        public decimal Weight
        {
            get { return fWeight; }
            set { SetPropertyValue<decimal>("Weight", ref fWeight, value); }
        }

        private string fBarCode;
        [Indexed(Unique = true)]
        public string BarCode
        {
            get { return fBarCode; }
            set { SetPropertyValue<string>("BarCode", ref fBarCode, value); }
        }

        private bool fPVPVariable;
        public bool PVPVariable
        {
            get { return fPVPVariable; }
            set { SetPropertyValue<bool>("PVPVariable", ref fPVPVariable, value); }
        }

        //Artigos Compostos [IN:016522]
        private bool fIsComposed;
        public bool IsComposed
        {
            get { return fIsComposed; }
            set { SetPropertyValue<bool>("IsComposed", ref fIsComposed, value); }
        }

        private bool fUniqueArticles;
        public bool UniqueArticles
        {
            get { return fUniqueArticles; }
            set { SetPropertyValue<bool>("UniqueArticles", ref fUniqueArticles, value); }
        }

        private bool fFavorite;
        public bool Favorite
        {
            get { return fFavorite; }
            set { SetPropertyValue<bool>("Favorite", ref fFavorite, value); }
        }

        private bool fUseWeighingBalance;
        public bool UseWeighingBalance
        {
            get { return fUseWeighingBalance; }
            set { SetPropertyValue<bool>("UseWeighingBalance", ref fUseWeighingBalance, value); }
        }

        private string fToken1;
        [Size(255)]
        public string Token1
        {
            get { return fToken1; }
            set { SetPropertyValue<string>("Token1", ref fToken1, value); }
        }

        private string fToken2;
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

		//Artigos Compostos [IN:016522]
        //Article One <> Many ArticlesComposition
        [Association(@"ArticleReferencesArticleComposition", typeof(fin_articlecomposition))]
        public XPCollection<fin_articlecomposition> ArticleComposition
        {
            get { return GetCollection<fin_articlecomposition>("ArticleComposition"); }
        }

        [Association(@"ArticleReferencesArticleSerialNumber", typeof(fin_articleserialnumber))]
        public XPCollection<fin_articleserialnumber> ArticleSerialNumber
        {
            get { return GetCollection<fin_articleserialnumber>("ArticleSerialNumber"); }
        }

        [Association(@"ArticleReferencesArticleWareHouse", typeof(fin_articlewarehouse))]
        public XPCollection<fin_articlewarehouse> ArticleWarehouse
        {
            get { return GetCollection<fin_articlewarehouse>("ArticleWarehouse"); }
        }

        //Family One <> Many Article
        private fin_articlefamily fFamily;
        [Association(@"FamilyReferencesArticle")]
        public fin_articlefamily Family
        {
            get { return fFamily; }
            set { SetPropertyValue<fin_articlefamily>("Family", ref fFamily, value); }
        }

        //SubFamily One <> Many Article
        private fin_articlesubfamily fSubFamily;
        [Association(@"SubFamilyReferencesArticle")]
        public fin_articlesubfamily SubFamily
        {
            get { return fSubFamily; }
            set { SetPropertyValue<fin_articlesubfamily>("SubFamily", ref fSubFamily, value); }
        }

        //ArticleType One <> Many Article
        private fin_articletype fType;
        [Association(@"ArticleTypeReferencesArticle")]
        public fin_articletype Type
        {
            get { return fType; }
            set { SetPropertyValue<fin_articletype>("Type", ref fType, value); }
        }

        //ArticleClass One <> Many Article
        private fin_articleclass fClass;
        [Association(@"ArticleClassReferencesArticle")]
        public fin_articleclass Class
        {
            get { return fClass; }
            set { SetPropertyValue<fin_articleclass>("Class", ref fClass, value); }
        }

        //configurationUnitMeasure One <> Many Article
        private cfg_configurationunitmeasure fUnitMeasure;
        [Association(@"ConfigurationUnitMeasureReferencesArticle")]
        public cfg_configurationunitmeasure UnitMeasure
        {
            get { return fUnitMeasure; }
            set { SetPropertyValue<cfg_configurationunitmeasure>("UnitMeasure", ref fUnitMeasure, value); }
        }

        //ConfigurationUnitSize One <> Many Article
        private cfg_configurationunitsize fUnitSize;
        [Association(@"ConfigurationUnitSizeReferencesArticle")]
        public cfg_configurationunitsize UnitSize
        {
            get { return fUnitSize; }
            set { SetPropertyValue<cfg_configurationunitsize>("UnitSize", ref fUnitSize, value); }
        }

        //UserCommissionGroup One <> Many Article
        private pos_usercommissiongroup fCommissionGroup;
        [Association(@"UserCommissionGroupReferencesArticle")]
        public pos_usercommissiongroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue<pos_usercommissiongroup>("CommissionGroup", ref fCommissionGroup, value); }
        }

        //CustomerDiscountGroup One <> Many Article
        private erp_customerdiscountgroup fDiscountGroup;
        [Association(@"CustomerDiscountGroupReferencesArticle")]
        public erp_customerdiscountgroup DiscountGroup
        {
            get { return fDiscountGroup; }
            set { SetPropertyValue<erp_customerdiscountgroup>("DiscountGroup", ref fDiscountGroup, value); }
        }

        //ConfigurationVatRate One <> Many Article
        private fin_configurationvatrate fVatOnTable;
        [Association(@"ConfigurationVatRateReferencesArticle_ForVatOnTable")]
        public fin_configurationvatrate VatOnTable
        {
            get { return fVatOnTable; }
            set { SetPropertyValue<fin_configurationvatrate>("VatTOnTable", ref fVatOnTable, value); }
        }

        //ConfigurationVatRate One <> Many Article
        private fin_configurationvatrate fVatDirectSelling;
        [Association(@"ConfigurationVatRateReferencesArticle_ForVatDirectSelling")]
        public fin_configurationvatrate VatDirectSelling
        {
            get { return fVatDirectSelling; }
            set { SetPropertyValue<fin_configurationvatrate>("VatDirectSelling", ref fVatDirectSelling, value); }
        }

        //ConfigurationVatExemptionReason One <> Many Article
        private fin_configurationvatexemptionreason fVatExemptionReason;
        [Association(@"ConfigurationVatExemptionReasonReferencesArticle")]
        public fin_configurationvatexemptionreason VatExemptionReason
        {
            get { return fVatExemptionReason; }
            set { SetPropertyValue<fin_configurationvatexemptionreason>("VatExemptionReason", ref fVatExemptionReason, value); }
        }

        //ConfigurationDevice One <> Many Article
        private sys_configurationprinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesArticle")]
        public sys_configurationprinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<sys_configurationprinters>("Printer", ref fPrinter, value); }
        }

        //DocumentFinanceType One <> Many ConfigurationPrintersTemplates
        private sys_configurationprinterstemplates fTemplate;
        [Association(@"ConfigurationPrintersTemplatesReferencesArticle")]
        public sys_configurationprinterstemplates Template
        {
            get { return fTemplate; }
            set { SetPropertyValue<sys_configurationprinterstemplates>("Template", ref fTemplate, value); }
        }

        private sys_configurationprinterstemplates fTemplateBarCode;
        [Association(@"ConfigurationPrintersBarCodeTemplatesReferencesArticle")]
        public sys_configurationprinterstemplates TemplateBarCode
        {
            get { return fTemplateBarCode; }
            set { SetPropertyValue<sys_configurationprinterstemplates>("TemplateBarCode", ref fTemplateBarCode, value); }
        }
    }
}