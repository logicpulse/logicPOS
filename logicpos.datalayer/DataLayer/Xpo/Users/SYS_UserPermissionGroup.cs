using System;
using DevExpress.Xpo;
using logicpos.datalayer.App;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_userpermissiongroup : XPGuidObject
    {
        public sys_userpermissiongroup() : base() { }
        public sys_userpermissiongroup(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(sys_userpermissiongroup), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(sys_userpermissiongroup), "Code");
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
        [Association(@"UserPermissionGroup-UserPermissionItem", typeof(sys_userpermissionitem))]
        public XPCollection<sys_userpermissionitem> Groups
        {
            get { return GetCollection<sys_userpermissionitem>("Groups"); }
        }
    }
}

