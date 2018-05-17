using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_UserProfile : XPGuidObject
    {
        public SYS_UserProfile() : base() { }
        public SYS_UserProfile(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(SYS_UserProfile), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(SYS_UserProfile), "Code");
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

        string fAccessPassword;
        [Size(50)]
        [ValueConverter(typeof(Encryption))]
        public string AccessPassword
        {
            get { return fAccessPassword; }
            set { SetPropertyValue<string>("AccessPassword", ref fAccessPassword, value); }
        }

        //UserProfile One <> Many User
        [Association(@"UserProfileReferencesUserDetail", typeof(SYS_UserDetail))]
        public XPCollection<SYS_UserDetail> Users
        {
            get { return GetCollection<SYS_UserDetail>("Users"); }
        }

        //UserProfile One <> Many UserPermissionProfile
        [Association(@"UserProfile-UserPermissionProfile", typeof(SYS_UserPermissionProfile))]
        public XPCollection<SYS_UserPermissionProfile> Permissions
        {
            get { return GetCollection<SYS_UserPermissionProfile>("Permissions"); }
        }
    }
}