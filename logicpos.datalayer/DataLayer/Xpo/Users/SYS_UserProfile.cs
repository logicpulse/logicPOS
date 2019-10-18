using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_userprofile : XPGuidObject
    {
        public sys_userprofile() : base() { }
        public sys_userprofile(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(sys_userprofile), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(sys_userprofile), "Code");
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
        [Association(@"UserProfileReferencesUserDetail", typeof(sys_userdetail))]
        public XPCollection<sys_userdetail> Users
        {
            get { return GetCollection<sys_userdetail>("Users"); }
        }

        //UserProfile One <> Many UserPermissionProfile
        [Association(@"UserProfile-UserPermissionProfile", typeof(sys_userpermissionprofile))]
        public XPCollection<sys_userpermissionprofile> Permissions
        {
            get { return GetCollection<sys_userpermissionprofile>("Permissions"); }
        }
    }
}