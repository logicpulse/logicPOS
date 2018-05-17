using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.Enums;
using System;
namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_Article : XPGuidObject
    {
        public FIN_Article() : base() { }
        public FIN_Article(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(FIN_Article), "Ord");
            //Disable now using AlfaNumeric Code, not Integer
            //Code = FrameworkUtils.GetNextTableFieldID("FIN_Article", "Code").ToString();
            Type = this.Session.GetObjectByKey<FIN_ArticleType>(SettingsApp.XpoOidArticleDefaultType);
            Class = this.Session.GetObjectByKey<FIN_ArticleClass>(SettingsApp.XpoOidArticleDefaultClass);
            //VatOnTable = this.Session.GetObjectByKey<FIN_ConfigurationVatRate>(SettingsApp.XpoOidArticleDefaultVatOnTable);
            //VatDirectSelling = this.Session.GetObjectByKey<FIN_ConfigurationVatRate>(SettingsApp.XpoOidArticleDefaultVatDirectSelling);
            if (SettingsApp.AppMode == AppOperationMode.Default)
            {
                //Force users to choose Tax for both modes Normal and TakeAway
                VatOnTable = null;
                VatDirectSelling = null;
            }
            else
            {
                VatOnTable = null;
                VatDirectSelling = this.Session.GetObjectByKey<FIN_ConfigurationVatRate>(SettingsApp.XpoOidArticleDefaultVatDirectSelling);
            }

            UnitMeasure = this.Session.GetObjectByKey<CFG_ConfigurationUnitMeasure>(SettingsApp.XpoOidArticleDefaultUnitMeasure);
            UnitSize = this.Session.GetObjectByKey<CFG_ConfigurationUnitSize>(SettingsApp.XpoOidArticleDefaultUnitSize);
            Template = this.Session.GetObjectByKey<SYS_ConfigurationPrintersTemplates>(SettingsApp.XpoOidArticleDefaultTemplate);
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

        //Article One <> Many DocumentOrderDetail
        [Association(@"ArticleReferencesDocumentOrderDetail", typeof(FIN_DocumentOrderDetail))]
        public XPCollection<FIN_DocumentOrderDetail> DocumentOrderDetail
        {
            get { return GetCollection<FIN_DocumentOrderDetail>("DocumentOrderDetail"); }
        }

        //Article One <> Many DocumentFinanceDetail
        [Association(@"ArticleReferencesDocumentDocumentFinanceDetail", typeof(FIN_DocumentFinanceDetail))]
        public XPCollection<FIN_DocumentFinanceDetail> DocumentFinanceDetail
        {
            get { return GetCollection<FIN_DocumentFinanceDetail>("DocumentFinanceDetail"); }
        }

        //Family One <> Many Article
        FIN_ArticleFamily fFamily;
        [Association(@"FamilyReferencesArticle")]
        public FIN_ArticleFamily Family
        {
            get { return fFamily; }
            set { SetPropertyValue<FIN_ArticleFamily>("Family", ref fFamily, value); }
        }

        //SubFamily One <> Many Article
        FIN_ArticleSubFamily fSubFamily;
        [Association(@"SubFamilyReferencesArticle")]
        public FIN_ArticleSubFamily SubFamily
        {
            get { return fSubFamily; }
            set { SetPropertyValue<FIN_ArticleSubFamily>("SubFamily", ref fSubFamily, value); }
        }

        //ArticleType One <> Many Article
        FIN_ArticleType fType;
        [Association(@"ArticleTypeReferencesArticle")]
        public FIN_ArticleType Type
        {
            get { return fType; }
            set { SetPropertyValue<FIN_ArticleType>("Type", ref fType, value); }
        }

        //ArticleClass One <> Many Article
        FIN_ArticleClass fClass;
        [Association(@"ArticleClassReferencesArticle")]
        public FIN_ArticleClass Class
        {
            get { return fClass; }
            set { SetPropertyValue<FIN_ArticleClass>("Class", ref fClass, value); }
        }

        //configurationUnitMeasure One <> Many Article
        CFG_ConfigurationUnitMeasure fUnitMeasure;
        [Association(@"ConfigurationUnitMeasureReferencesArticle")]
        public CFG_ConfigurationUnitMeasure UnitMeasure
        {
            get { return fUnitMeasure; }
            set { SetPropertyValue<CFG_ConfigurationUnitMeasure>("UnitMeasure", ref fUnitMeasure, value); }
        }

        //ConfigurationUnitSize One <> Many Article
        CFG_ConfigurationUnitSize fUnitSize;
        [Association(@"ConfigurationUnitSizeReferencesArticle")]
        public CFG_ConfigurationUnitSize UnitSize
        {
            get { return fUnitSize; }
            set { SetPropertyValue<CFG_ConfigurationUnitSize>("UnitSize", ref fUnitSize, value); }
        }

        //UserCommissionGroup One <> Many Article
        POS_UserCommissionGroup fCommissionGroup;
        [Association(@"UserCommissionGroupReferencesArticle")]
        public POS_UserCommissionGroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue<POS_UserCommissionGroup>("CommissionGroup", ref fCommissionGroup, value); }
        }

        //CustomerDiscountGroup One <> Many Article
        ERP_CustomerDiscountGroup fDiscountGroup;
        [Association(@"CustomerDiscountGroupReferencesArticle")]
        public ERP_CustomerDiscountGroup DiscountGroup
        {
            get { return fDiscountGroup; }
            set { SetPropertyValue<ERP_CustomerDiscountGroup>("DiscountGroup", ref fDiscountGroup, value); }
        }

        //ConfigurationVatRate One <> Many Article
        FIN_ConfigurationVatRate fVatOnTable;
        [Association(@"ConfigurationVatRateReferencesArticle_ForVatOnTable")]
        public FIN_ConfigurationVatRate VatOnTable
        {
            get { return fVatOnTable; }
            set { SetPropertyValue<FIN_ConfigurationVatRate>("VatTOnTable", ref fVatOnTable, value); }
        }

        //ConfigurationVatRate One <> Many Article
        FIN_ConfigurationVatRate fVatDirectSelling;
        [Association(@"ConfigurationVatRateReferencesArticle_ForVatDirectSelling")]
        public FIN_ConfigurationVatRate VatDirectSelling
        {
            get { return fVatDirectSelling; }
            set { SetPropertyValue<FIN_ConfigurationVatRate>("VatDirectSelling", ref fVatDirectSelling, value); }
        }

        //ConfigurationVatExemptionReason One <> Many Article
        FIN_ConfigurationVatExemptionReason fVatExemptionReason;
        [Association(@"ConfigurationVatExemptionReasonReferencesArticle")]
        public FIN_ConfigurationVatExemptionReason VatExemptionReason
        {
            get { return fVatExemptionReason; }
            set { SetPropertyValue<FIN_ConfigurationVatExemptionReason>("VatExemptionReason", ref fVatExemptionReason, value); }
        }

        //ConfigurationDevice One <> Many Article
        SYS_ConfigurationPrinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesArticle")]
        public SYS_ConfigurationPrinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<SYS_ConfigurationPrinters>("Printer", ref fPrinter, value); }
        }

        //DocumentFinanceType One <> Many ConfigurationPrintersTemplates
        SYS_ConfigurationPrintersTemplates fTemplate;
        [Association(@"ConfigurationPrintersTemplatesReferencesArticle")]
        public SYS_ConfigurationPrintersTemplates Template
        {
            get { return fTemplate; }
            set { SetPropertyValue<SYS_ConfigurationPrintersTemplates>("Template", ref fTemplate, value); }
        }
    }
}