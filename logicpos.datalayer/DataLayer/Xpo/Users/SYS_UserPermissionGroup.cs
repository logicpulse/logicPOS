using System;
using DevExpress.Xpo;
using logicpos.datalayer.App;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_UserPermissionGroup : XPGuidObject
    {
        public SYS_UserPermissionGroup() : base() { }
        public SYS_UserPermissionGroup(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(SYS_UserPermissionGroup), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(SYS_UserPermissionGroup), "Code");
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

        //UserPermissionGroup One <> Many UserPermissionItems
        [Association(@"UserPermissionGroup-UserPermissionItem", typeof(SYS_UserPermissionItem))]
        public XPCollection<SYS_UserPermissionItem> Groups
        {
            get { return GetCollection<SYS_UserPermissionItem>("Groups"); }
        }
    }
}

