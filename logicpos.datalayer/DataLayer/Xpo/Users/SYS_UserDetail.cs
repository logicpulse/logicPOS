using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_userdetail : XPGuidObject
    {
        public sys_userdetail() : base() { }
        public sys_userdetail(Session session) : base(session)
        {
            // Init EncryptedAttributes - Load Encrypted Attributes Fields if Exist
            InitEncryptedAttributes<sys_userdetail>();
        }

        protected override void OnAfterConstruction()
        {
            // Init EncryptedAttributes - Load Encrypted Attributes Fields if Exist - Required for New Records to have InitEncryptedAttributes else it Triggers Exception on Save
            InitEncryptedAttributes<sys_userdetail>();

            Ord = FrameworkUtils.GetNextTableFieldID(nameof(sys_userdetail), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(sys_userdetail), "Code");
            //Required for New Users
            AccessPin = CryptographyUtils.SaltedString.GenerateSaltedString(SettingsApp.DefaultValueUserDetailAccessPin);
            PasswordReset = true;
            ButtonImage = string.Format("{0}{1}", GlobalFramework.Path["assets"], SettingsApp.DefaultValueUserDetailButtonImage);
        }

        protected override void OnNewRecordSaving()
        {
            //Required for SAF-T
            CodeInternal = FrameworkUtils.GuidToStringId(Oid.ToString());
        }

        UInt32 _Ord;
        public UInt32 Ord
        {
            get { return _Ord; }
            set { SetPropertyValue<UInt32>("Ord", ref _Ord, value); }
        }

        UInt32 fCode;
        [Indexed(Unique = true)]
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        String fCodeInternal;
        [Indexed(Unique = true), Size(30)]
        public String CodeInternal
        {
            get { return fCodeInternal; }
            set { SetPropertyValue<String>("CodeInternal", ref fCodeInternal, value); }
        }

        string fName;
        [Size(512)]
        [XPGuidObject(Encrypted = true)]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        string fResidence;
        [Size(512)]
        [XPGuidObject(Encrypted = true)]
        public string Residence
        {
            get { return fResidence; }
            set { SetPropertyValue<string>("Residence", ref fResidence, value); }
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

        string fDateOfContract;
        [XPGuidObject(Encrypted = true)]
        public string DateOfContract
        {
            get { return fDateOfContract; }
            set { SetPropertyValue<string>("DateOfContract", ref fDateOfContract, value); }
        }

        string fPhone;
        [Size(255)]
        [XPGuidObject(Encrypted = true)]
        public string Phone
        {
            get { return fPhone; }
            set { SetPropertyValue<string>("Phone", ref fPhone, value); }
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

        string fFiscalNumber;
        //[Indexed(Unique = true)]
        [XPGuidObject(Encrypted = true)]
        public string FiscalNumber
        {
            get { return fFiscalNumber; }
            set { SetPropertyValue<string>("FiscalNumber", ref fFiscalNumber, value); }
        }

        string fLanguage;
        public string Language
        {
            get { return fLanguage; }
            set { SetPropertyValue<string>("Language", ref fLanguage, value); }
        }

        string fAssignedSeating;
        public string AssignedSeating
        {
            get { return fAssignedSeating; }
            set { SetPropertyValue<string>("AssignedSeating", ref fAssignedSeating, value); }
        }

        string fAccessPin;
        [Size(255)]
        [Indexed(Unique = true)]
        public string AccessPin
        {
            get { return fAccessPin; }
            set { SetPropertyValue<string>("AccessPin", ref fAccessPin, value); }
        }

        string fAccessCardNumber;
        public string AccessCardNumber
        {
            get { return fAccessCardNumber; }
            set { SetPropertyValue<string>("AccessCardNumber", ref fAccessCardNumber, value); }
        }

        string fLogin;
        public string Login
        {
            get { return fLogin; }
            set { SetPropertyValue<string>("Login", ref fLogin, value); }
        }

        string fPassword;
        [Size(255)]
        public string Password
        {
            get { return fPassword; }
            set { SetPropertyValue<string>("Password", ref fPassword, value); }
        }

        Boolean fPasswordReset;
        public Boolean PasswordReset
        {
            get { return fPasswordReset; }
            set { SetPropertyValue<Boolean>("PasswordReset", ref fPasswordReset, value); }
        }

        DateTime fPasswordResetDate;
        public DateTime PasswordResetDate
        {
            get { return fPasswordResetDate; }
            set { SetPropertyValue<DateTime>("PasswordResetDate", ref fPasswordResetDate, value); }
        }

        string fBaseConsumption;
        public string BaseConsumption
        {
            get { return fBaseConsumption; }
            set { SetPropertyValue<string>("BaseConsumption", ref fBaseConsumption, value); }
        }

        string fBaseOffers;
        public string BaseOffers
        {
            get { return fBaseOffers; }
            set { SetPropertyValue<string>("BaseOffers", ref fBaseOffers, value); }
        }

        string fPVPOffers;
        public string PVPOffers
        {
            get { return fPVPOffers; }
            set { SetPropertyValue<string>("PVPOffers", ref fPVPOffers, value); }
        }

        string fRemarks;
        public string Remarks
        {
            get { return fRemarks; }
            set { SetPropertyValue<string>("Remarks", ref fRemarks, value); }
        }

        string fButtonImage;
        [Size(255)]
        public string ButtonImage
        {
            get { return fButtonImage; }
            set { SetPropertyValue<string>("ButtonImage", ref fButtonImage, value); }
        }

        //UserProfile One <> Many User
        sys_userprofile fProfile;
        [Association(@"UserProfileReferencesUserDetail")]
        public sys_userprofile Profile
        {
            get { return fProfile; }
            set { SetPropertyValue<sys_userprofile>("Profile", ref fProfile, value); }
        }

        //CommissionGroup One <> Many User
        pos_usercommissiongroup fCommissionGroup;
        [Association(@"UserCommissionGroupReferencesUserDetail")]
        public pos_usercommissiongroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue<pos_usercommissiongroup>("CommissionGroup", ref fCommissionGroup, value); }
        }
    }
}