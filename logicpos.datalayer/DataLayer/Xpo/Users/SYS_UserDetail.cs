using DevExpress.Xpo;
using logicpos.datalayer;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_UserDetail : XPGuidObject
    {
        public SYS_UserDetail() : base() { }
        public SYS_UserDetail(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("SYS_UserDetail", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("SYS_UserDetail", "Code");
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
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        string fResidence;
        public string Residence
        {
            get { return fResidence; }
            set { SetPropertyValue<string>("Residence", ref fResidence, value); }
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

        string fDateOfContract;
        public string DateOfContract
        {
            get { return fDateOfContract; }
            set { SetPropertyValue<string>("DateOfContract", ref fDateOfContract, value); }
        }

        string fPhone;
        public string Phone
        {
            get { return fPhone; }
            set { SetPropertyValue<string>("Phone", ref fPhone, value); }
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

        string fFiscalNumber;
        //[Indexed(Unique = true)]
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
        SYS_UserProfile fProfile;
        [Association(@"UserProfileReferencesUserDetail")]
        public SYS_UserProfile Profile
        {
            get { return fProfile; }
            set { SetPropertyValue<SYS_UserProfile>("Profile", ref fProfile, value); }
        }

        //CommissionGroup One <> Many User
        POS_UserCommissionGroup fCommissionGroup;
        [Association(@"UserCommissionGroupReferencesUserDetail")]
        public POS_UserCommissionGroup CommissionGroup
        {
            get { return fCommissionGroup; }
            set { SetPropertyValue<POS_UserCommissionGroup>("CommissionGroup", ref fCommissionGroup, value); }
        }
    }
}