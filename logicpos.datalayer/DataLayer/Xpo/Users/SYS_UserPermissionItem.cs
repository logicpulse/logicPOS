using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_UserPermissionItem : XPGuidObject
    {
        public SYS_UserPermissionItem() : base() { }
        public SYS_UserPermissionItem(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(SYS_UserPermissionItem), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(SYS_UserPermissionItem), "Code");
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
        SYS_UserPermissionGroup fPermissionGroup;
        [Association(@"UserPermissionGroup-UserPermissionItem")]
        public SYS_UserPermissionGroup PermissionGroup
        {
            get { return fPermissionGroup; }
            set { SetPropertyValue<SYS_UserPermissionGroup>("PermissionGroup", ref fPermissionGroup, value); }
        }

        //UserProfilePermissions One <> Many UserPermissionItems
        [Association(@"UserPermissionProfile-UserPermissionItem", typeof(SYS_UserPermissionProfile))]
        public XPCollection<SYS_UserPermissionProfile> PermissionProfiles
        {
            get { return GetCollection<SYS_UserPermissionProfile>("PermissionProfiles"); }
        }
    }
}
