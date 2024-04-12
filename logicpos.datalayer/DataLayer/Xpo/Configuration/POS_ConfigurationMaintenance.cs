using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class pos_configurationmaintenance : XPGuidObject
    {
        public pos_configurationmaintenance() : base() { }
        public pos_configurationmaintenance(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(pos_configurationmaintenance), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(pos_configurationmaintenance), "Code");
        }

        private UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private UInt32 fCode;
        [Indexed(Unique = true)]
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fDate;
        public string Date
        {
            get { return fDate; }
            set { SetPropertyValue<string>("Date", ref fDate, value); }
        }

        private string fTime;
        public string Time
        {
            get { return fTime; }
            set { SetPropertyValue<string>("Time", ref fTime, value); }
        }

        private string fPasswordAccess;
        public string PasswordAccess
        {
            get { return fPasswordAccess; }
            set { SetPropertyValue<string>("AccessPassword", ref fPasswordAccess, value); }
        }

        private string fRemarks;
        public string Remarks
        {
            get { return fRemarks; }
            set { SetPropertyValue<string>("Remarks", ref fRemarks, value); }
        }
    }
}