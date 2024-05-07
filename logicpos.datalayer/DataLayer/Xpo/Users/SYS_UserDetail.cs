using DevExpress.Xpo;
using logicpos.datalayer.Xpo;
using LogicPOS.Settings;
using LogicPOS.Utility;
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

            Ord = XPOHelper.GetNextTableFieldID(nameof(sys_userdetail), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(sys_userdetail), "Code");
            //Required for New Users
            AccessPin = CryptographyUtils.GenerateSaltedString(XPOSettings.DefaultValueUserDetailAccessPin);
            PasswordReset = true;
            ButtonImage = string.Format("{0}{1}", GeneralSettings.Path["assets"], XPOSettings.DefaultValueUserDetailButtonImage);
        }

        protected override void OnNewRecordSaving()
        {
            //Required for SAF-T
            CodeInternal = XPOHelper.GuidToStringId(Oid.ToString());
        }

        private uint _Ord;
        public uint Ord
        {
            get { return _Ord; }
            set { SetPropertyValue<UInt32>("Ord", ref _Ord, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        private string fCodeInternal;
        [Indexed(Unique = true), Size(30)]
        public string CodeInternal
        {
            get { return fCodeInternal; }
            set { SetPropertyValue<string>("CodeInternal", ref fCodeInternal, value); }
        }

        private string fName;
        [Size(512)]
        [XPGuidObject(Encrypted = true)]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        private string fResidence;
        [Size(512)]
        [XPGuidObject(Encrypted = true)]
        public string Residence
        {
            get { return fResidence; }
            set { SetPropertyValue<string>("Residence", ref fResidence, value); }
        }

        private string fLocality;
        [Size(255)]
        [XPGuidObject(Encrypted = true)]
        public string Locality
        {
            get { return fLocality; }
            set { SetPropertyValue<string>("Locality", ref fLocality, value); }
        }

        private string fZipCode;
        [XPGuidObject(Encrypted = true)]
        public string ZipCode
        {
            get { return fZipCode; }
            set { SetPropertyValue<string>("ZipCode", ref fZipCode, value); }
        }

        private string fCity;
        [Size(255)]
        [XPGuidObject(Encrypted = true)]
        public string City
        {
            get { return fCity; }
            set { SetPropertyValue<string>("City", ref fCity, value); }
        }

        private string fDateOfContract;
        [XPGuidObject(Encrypted = true)]
        public string DateOfContract
        {
            get { return fDateOfContract; }
            set { SetPropertyValue<string>("DateOfContract", ref fDateOfContract, value); }
        }

        private string fPhone;
        [Size(255)]
        [XPGuidObject(Encrypted = true)]
        public string Phone
        {
            get { return fPhone; }
            set { SetPropertyValue<string>("Phone", ref fPhone, value); }
        }

        private string fMobilePhone;
        [Size(255)]
        [XPGuidObject(Encrypted = true)]
        public string MobilePhone
        {
            get { return fMobilePhone; }
            set { SetPropertyValue<string>("MobilePhone", ref fMobilePhone, value); }
        }

        private string fEmail;
        [Size(255)]
        [XPGuidObject(Encrypted = true)]
        public string Email
        {
            get { return fEmail; }
            set { SetPropertyValue<string>("Email", ref fEmail, value); }
        }

        private string fFiscalNumber;
        //[Indexed(Unique = true)]
        [XPGuidObject(Encrypted = true)]
        public string FiscalNumber
        {
            get { return fFiscalNumber; }
            set { SetPropertyValue<string>("FiscalNumber", ref fFiscalNumber, value); }
        }

        private string fLanguage;
        public string Language
        {
            get { return fLanguage; }
            set { SetPropertyValue<string>("Language", ref fLanguage, value); }
        }

        private string fAssignedSeating;
        public string AssignedSeating
        {
            get { return fAssignedSeating; }
            set { SetPropertyValue<string>("AssignedSeating", ref fAssignedSeating, value); }
        }

        private string fAccessPin;
        [Size(255)]
        [Indexed(Unique = true)]
        public string AccessPin
        {
            get { return fAccessPin; }
            set { SetPropertyValue<string>("AccessPin", ref fAccessPin, value); }
        }

        private string fAccessCardNumber;
        public string AccessCardNumber
        {
            get { return fAccessCardNumber; }
            set { SetPropertyValue<string>("AccessCardNumber", ref fAccessCardNumber, value); }
        }

        private string fLogin;
        public string Login
        {
            get { return fLogin; }
            set { SetPropertyValue<string>("Login", ref fLogin, value); }
        }

        private string fPassword;
        [Size(255)]
        public string Password
        {
            get { return fPassword; }
            set { SetPropertyValue<string>("Password", ref fPassword, value); }
        }

        private bool fPasswordReset;
        public bool PasswordReset
        {
            get { return fPasswordReset; }
            set { SetPropertyValue<bool>("PasswordReset", ref fPasswordReset, value); }
        }

        private DateTime fPasswordResetDate;
        public DateTime PasswordResetDate
        {
            get { return fPasswordResetDate; }
            set { SetPropertyValue<DateTime>("PasswordResetDate", ref fPasswordResetDate, value); }
        }

        private string fBaseConsumption;
        public string BaseConsumption
        {
            get { return fBaseConsumption; }
            set { SetPropertyValue<string>("BaseConsumption", ref fBaseConsumption, value); }
        }

        private string fBaseOffers;
        public string BaseOffers
        {
            get { return fBaseOffers; }
            set { SetPropertyValue<string>("BaseOffers", ref fBaseOffers, value); }
        }

        private string fPVPOffers;
        public string PVPOffers
        {
            get { return fPVPOffers; }
            set { SetPropertyValue<string>("PVPOffers", ref fPVPOffers, value); }
        }

        private string fRemarks;
        public string Remarks
        {
            get { return fRemarks; }
            set { SetPropertyValue<string>("Remarks", ref fRemarks, value); }
        }

        private string fButtonImage;
        [Size(255)]
        public string ButtonImage
        {
            get { return fButtonImage; }
            set { SetPropertyValue<string>("ButtonImage", ref fButtonImage, value); }
        }

        //UserProfile One <> Many User
        private sys_userprofile fProfile;
        [Association(@"UserProfileReferencesUserDetail")]
        public sys_userprofile Profile
        {
            get { return fProfile; }
            set { SetPropertyValue<sys_userprofile>("Profile", ref fProfile, value); }
        }

        //CommissionGroup One <> Many User
        private pos_usercommissiongroup fCommissionGroup;
        [Association(@"UserCommissionGroupReferencesUserDetail")]
        public pos_usercommissiongroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue<pos_usercommissiongroup>("CommissionGroup", ref fCommissionGroup, value); }
        }
    }
}