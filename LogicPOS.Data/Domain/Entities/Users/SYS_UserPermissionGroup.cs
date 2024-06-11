using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class sys_userpermissiongroup : Entity
    {
        public sys_userpermissiongroup() : base() { }
        public sys_userpermissiongroup(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(sys_userpermissiongroup), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(sys_userpermissiongroup), "Code");
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

        //UserPermissionGroup One <> Many UserPermissionItems
        [Association(@"UserPermissionGroup-UserPermissionItem", typeof(sys_userpermissionitem))]
        public XPCollection<sys_userpermissionitem> Groups
        {
            get { return GetCollection<sys_userpermissionitem>("Groups"); }
        }
    }
}

