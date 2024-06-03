using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class sys_userpermissionitem : Entity
    {
        public sys_userpermissionitem() : base() { }
        public sys_userpermissionitem(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(sys_userpermissionitem), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(sys_userpermissionitem), "Code");
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

        private string fToken;
        [Size(100)]
        [Indexed(Unique = true)]
        public string Token
        {
            get { return fToken; }
            set { SetPropertyValue<string>("Token", ref fToken, value); }
        }

        private string fDesignation;
        [Size(200)]
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        //UserPermissionGroup One <> Many UserPermissionItems
        private sys_userpermissiongroup fPermissionGroup;
        [Association(@"UserPermissionGroup-UserPermissionItem")]
        public sys_userpermissiongroup PermissionGroup
        {
            get { return fPermissionGroup; }
            set { SetPropertyValue("PermissionGroup", ref fPermissionGroup, value); }
        }

        //UserProfilePermissions One <> Many UserPermissionItems
        [Association(@"UserPermissionProfile-UserPermissionItem", typeof(sys_userpermissionprofile))]
        public XPCollection<sys_userpermissionprofile> PermissionProfiles
        {
            get { return GetCollection<sys_userpermissionprofile>("PermissionProfiles"); }
        }
    }
}
