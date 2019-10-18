using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class erp_customer : XPGuidObject
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public erp_customer() : base() { }
        public erp_customer(Session session) : base(session)
        {
            // Init EncryptedAttributes - Load Encrypted Attributes Fields if Exist
            InitEncryptedAttributes<erp_customer>();
        }

        protected override void OnAfterConstruction()
        {
            // Init EncryptedAttributes - Load Encrypted Attributes Fields if Exist - Required for New Records to have InitEncryptedAttributes else it Triggers Exception on Save
            InitEncryptedAttributes<erp_customer>();

            Ord = FrameworkUtils.GetNextTableFieldID(nameof(erp_customer), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(erp_customer), "Code");
            Country = this.Session.GetObjectByKey<cfg_configurationcountry>(SettingsApp.ConfigurationSystemCountry.Oid);
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
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        string fCodeInternal;
        [Indexed(Unique = true), Size(30)]
        public string CodeInternal
        {
            get { return fCodeInternal; }
            set { SetPropertyValue<string>("CodeInternal", ref fCodeInternal, value); }
        }

        string fName;
        [Size(512)]
        [XPGuidObject(Encrypted = true)]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        string fAddress;
        [Size(512)]
        [XPGuidObject(Encrypted = true)]
        public string Address
        {
            get { return fAddress; }
            set { SetPropertyValue<string>("Address", ref fAddress, value); }
        }

        string fLocality;
        [Size(255)]
        [XPGuidObject(Encrypted = true)]
        public string Locality
        {
            get { return fLocality; }
            set { SetPropertyValue<string>("Locality", ref fLocality, value); }
        }

        string fZipCode;
        [XPGuidObject(Encrypted = true)]
        public string ZipCode
        {
            get { return fZipCode; }
            set { SetPropertyValue<string>("ZipCode", ref fZipCode, value); }
        }

        string fCity;
        [Size(255)]
        [XPGuidObject(Encrypted = true)]
        public string City
        {
            get { return fCity; }
            set { SetPropertyValue<string>("City", ref fCity, value); }
        }

        string fDateOfBirth;
        [XPGuidObject(Encrypted = true)]
        public string DateOfBirth
        {
            get { return fDateOfBirth; }
            set { SetPropertyValue<string>("DateOfBirth", ref fDateOfBirth, value); }
        }

        string fPhone;
        [Size(255)]
        [XPGuidObject(Encrypted = true)]
        public string Phone
        {
            get { return fPhone; }
            set { SetPropertyValue<string>("Phone", ref fPhone, value); }
        }

        string fFax;
        [Size(255)]
        [XPGuidObject(Encrypted = true)]
        public string Fax
        {
            get { return fFax; }
            set { SetPropertyValue<string>("Fax", ref fFax, value); }
        }

        string fMobilePhone;
        [Size(255)]
        [XPGuidObject(Encrypted = true)]
        public string MobilePhone
        {
            get { return fMobilePhone; }
            set { SetPropertyValue<string>("MobilePhone", ref fMobilePhone, value); }
        }

        string fEmail;
        [Size(255)]
        [XPGuidObject(Encrypted = true)]
        public string Email
        {
            get { return fEmail; }
            set { SetPropertyValue<string>("Email", ref fEmail, value); }
        }

        string fWebSite;
        [XPGuidObject(Encrypted = true)]
        [Size(255)]
        public string WebSite
        {
            get { return fWebSite; }
            set { SetPropertyValue<string>("WebSite", ref fWebSite, value); }
        }

        string fFiscalNumber;
        [XPGuidObject(Encrypted = true)]
        //Removed, now we can have diferent customers with blacnk or null FisaclNumber
        //[Indexed(Unique = true)]
        public string FiscalNumber
        {
            get { return fFiscalNumber; }
            set { SetPropertyValue<string>("FiscalNumber", ref fFiscalNumber, value); }
        }

        string fCardNumber;
        [XPGuidObject(Encrypted = true)]
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
        erp_customertype fCustomerType;
        [Association(@"CustomerTypeReferencesCustomer")]
        public erp_customertype CustomerType
        {
            get { return fCustomerType; }
            set { SetPropertyValue<erp_customertype>("CustomerType", ref fCustomerType, value); }
        }

        //CustomerDiscountGroup One <> Many Customer
        erp_customerdiscountgroup fDiscountGroup;
        [Association(@"CustomerDiscountGroupReferencesCustomer")]
        public erp_customerdiscountgroup DiscountGroup
        {
            get { return fDiscountGroup; }
            set { SetPropertyValue<erp_customerdiscountgroup>("DiscountGroup", ref fDiscountGroup, value); }
        }

        //ConfigurationPriceType One <> Many Customer
        fin_configurationpricetype fPriceType;
        [Association(@"ConfigurationPriceTypeReferencesCustomer")]
        public fin_configurationpricetype PriceType
        {
            get { return fPriceType; }
            set { SetPropertyValue<fin_configurationpricetype>("PriceType", ref fPriceType, value); }
        }

        //ConfigurationCountry One <> Many Customer
        cfg_configurationcountry fCountry;
        [Association(@"ConfigurationCountryReferencesCustomer")]
        public cfg_configurationcountry Country
        {
            get { return fCountry; }
            set { SetPropertyValue<cfg_configurationcountry>("Country", ref fCountry, value); }
        }
    }
}