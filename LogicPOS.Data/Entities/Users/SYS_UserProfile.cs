using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class sys_userprofile : XPGuidObject
    {
        public sys_userprofile() : base() { }
        public sys_userprofile(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(sys_userprofile), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(sys_userprofile), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fAccessPassword;
        [Size(50)]
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