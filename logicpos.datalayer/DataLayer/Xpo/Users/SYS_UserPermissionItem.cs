using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_userpermissionitem : XPGuidObject
    {
        public sys_userpermissionitem() : base() { }
        public sys_userpermissionitem(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(sys_userpermissionitem), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(sys_userpermissionitem), "Code");
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

        string fToken;
        [Size(100)]
        [Indexed(Unique = true)]
        public String Token
        {
            get { return fToken; }
            set { SetPropertyValue<String>("Token", ref fToken, value); }
        }

        string fDesignation;
        [Size(200)]
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        //UserPermissionGroup One <> Many UserPermissionItems
        sys_userpermissiongroup fPermissionGroup;
        [Association(@"UserPermissionGroup-UserPermissionItem")]
        public sys_userpermissiongroup PermissionGroup
        {
            get { return fPermissionGroup; }
            set { SetPropertyValue<sys_userpermissiongroup>("PermissionGroup", ref fPermissionGroup, value); }
        }

        //UserProfilePermissions One <> Many UserPermissionItems
        [Association(@"UserPermissionProfile-UserPermissionItem", typeof(sys_userpermissionprofile))]
        public XPCollection<sys_userpermissionprofile> PermissionProfiles
        {
            get { return GetCollection<sys_userpermissionprofile>("PermissionProfiles"); }
        }
    }
}
