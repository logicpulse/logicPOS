using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class ERP_Customer : XPGuidObject
    {
        public ERP_Customer() : base() { }
        public ERP_Customer(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("ERP_Customer", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("ERP_Customer", "Code");
            Country = this.Session.GetObjectByKey<CFG_ConfigurationCountry>(SettingsApp.ConfigurationSystemCountry.Oid);
        }

        protected override void OnNewRecordSaving()
        {
            //Required for SAF-T
            CodeInternal = FrameworkUtils.GuidToStringId(Oid.ToString());
        }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        UInt32 fCode;
        //[Indexed(Unique = true)] : Can have Duplicated Customer Codes ex Hidden Customers
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32 >("Code", ref fCode, value); }
        }

        string fCodeInternal;
        [Indexed(Unique = true), Size(30)]
        public string CodeInternal
        {
            get { return fCodeInternal; }
            set { SetPropertyValue<string>("CodeInternal", ref fCodeInternal, value); }
        }

        string fName;
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        string fAddress;
        public string Address
        {
            get { return fAddress; }
            set { SetPropertyValue<string>("Address", ref fAddress, value); }
        }

        string fLocality;
        public string Locality
        {
            get { return fLocality; }
            set { SetPropertyValue<string>("Locality", ref fLocality, value); }
        }

        string fZipCode;
        public string ZipCode
        {
            get { return fZipCode; }
            set { SetPropertyValue<string>("ZipCode", ref fZipCode, value); }
        }

        string fCity;
        public string City
        {
            get { return fCity; }
            set { SetPropertyValue<string>("City", ref fCity, value); }
        }

        string fDateOfBirth;
        public string DateOfBirth
        {
            get { return fDateOfBirth; }
            set { SetPropertyValue<string>("DateOfBirth", ref fDateOfBirth, value); }
        }

        string fPhone;
        public string Phone
        {
            get { return fPhone; }
            set { SetPropertyValue<string>("Phone", ref fPhone, value); }
        }

        string fFax;
        public string Fax
        {
            get { return fFax; }
            set { SetPropertyValue<string>("Fax", ref fFax, value); }
        }

        string fMobilePhone;
        public string MobilePhone
        {
            get { return fMobilePhone; }
            set { SetPropertyValue<string>("MobilePhone", ref fMobilePhone, value); }
        }

        string fEmail;
        public string Email
        {
            get { return fEmail; }
            set { SetPropertyValue<string>("Email", ref fEmail, value); }
        }

        string fWebSite;
        [Size(255)]
        public string WebSite
        {
            get { return fWebSite; }
            set { SetPropertyValue<string>("WebSite", ref fWebSite, value); }
        }

        string fFiscalNumber;
        //Removed, now we can have diferent customers with blacnk or null FisaclNumber
        //[Indexed(Unique = true)]
        public string FiscalNumber
        {
            get { return fFiscalNumber; }
            set { SetPropertyValue<string>("FiscalNumber", ref fFiscalNumber, value); }
        }

        string fCardNumber;
        [Indexed(Unique = true)] //Give Problems in MsSqlServer, Must have a Card for Every Customer, Dont permit NULL 
        public string CardNumber
        {
            get { return fCardNumber; }
            set { SetPropertyValue<string>("CardNumber", ref fCardNumber, value); }
        }

        string fDiscountType;
        public string DiscountType
        {
            get { return fDiscountType; }
            set { SetPropertyValue<string>("DiscountType", ref fDiscountType, value); }
        }

        decimal fDiscount;
        public decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<decimal>("Discount", ref fDiscount, value); }
        }

        decimal fCardCredit;
        public decimal CardCredit
        {
            get { return fCardCredit; }
            set { SetPropertyValue<decimal>("CardCredit", ref fCardCredit, value); }
        }

        decimal fTotalDebt;
        public decimal TotalDebt
        {
            get { return fTotalDebt; }
            set { SetPropertyValue<decimal>("TotalDebt", ref fTotalDebt, value); }
        }

        decimal fTotalCredit;
        public decimal TotalCredit
        {
            get { return fTotalCredit; }
            set { SetPropertyValue<decimal>("TotalCredit", ref fTotalCredit, value); }
        }

        decimal fCurrentBalance;
        public decimal CurrentBalance
        {
            get { return fCurrentBalance; }
            set { SetPropertyValue<decimal>("CurrentBalance", ref fCurrentBalance, value); }
        }

        string fCreditLine;
        public string CreditLine
        {
            get { return fCreditLine; }
            set { SetPropertyValue<string>("CreditLine", ref fCreditLine, value); }
        }

        string fRemarks;
        public string Remarks
        {
            get { return fRemarks; }
            set { SetPropertyValue<string>("Remarks", ref fRemarks, value); }
        }

        Boolean fSupplier;
        public Boolean Supplier
        {
            get { return fSupplier; }
            set { SetPropertyValue<Boolean>("Supplier", ref fSupplier, value); }
        }        

        //Assign True to Temporary Customers, Customers that was created in Finance Documents without FiscalNumber
        Boolean fHidden;
        public Boolean Hidden
        {
            get { return fHidden; }
            set { SetPropertyValue<Boolean>("Hidden", ref fHidden, value); }
        }

        //CustomerType One <> Many Customer
        ERP_CustomerType fCustomerType;
        [Association(@"CustomerTypeReferencesCustomer")]
        public ERP_CustomerType CustomerType
        {
            get { return fCustomerType; }
            set { SetPropertyValue<ERP_CustomerType>("CustomerType", ref fCustomerType, value); }
        }

        //CustomerDiscountGroup One <> Many Customer
        ERP_CustomerDiscountGroup fDiscountGroup;
        [Association(@"CustomerDiscountGroupReferencesCustomer")]
        public ERP_CustomerDiscountGroup DiscountGroup
        {
            get { return fDiscountGroup; }
            set { SetPropertyValue<ERP_CustomerDiscountGroup>("DiscountGroup", ref fDiscountGroup, value); }
        }

        //ConfigurationPriceType One <> Many Customer
        FIN_ConfigurationPriceType fPriceType;
        [Association(@"ConfigurationPriceTypeReferencesCustomer")]
        public FIN_ConfigurationPriceType PriceType
        {
            get { return fPriceType; }
            set { SetPropertyValue<FIN_ConfigurationPriceType>("PriceType", ref fPriceType, value); }
        }

        //ConfigurationCountry One <> Many Customer
        CFG_ConfigurationCountry fCountry;
        [Association(@"ConfigurationCountryReferencesCustomer")]
        public CFG_ConfigurationCountry Country
        {
            get { return fCountry; }
            set { SetPropertyValue<CFG_ConfigurationCountry>("Country", ref fCountry, value); }
        }
    }
}