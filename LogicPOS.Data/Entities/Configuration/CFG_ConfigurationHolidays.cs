using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class cfg_configurationholidays : XPGuidObject
    {
        public cfg_configurationholidays() : base() { }
        public cfg_configurationholidays(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(cfg_configurationholidays), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(cfg_configurationholidays), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
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

        private string fDescription;
        [Size(255)]
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue<string>("Description", ref fDescription, value); }
        }

        private int fYear;
        public int Year
        {
            get { return fYear; }
            set { SetPropertyValue<int>("Year", ref fYear, value); }
        }

        private int fMonth;
        public int Month
        {
            get { return fMonth; }
            set { SetPropertyValue<int>("Month", ref fMonth, value); }
        }

        private int fDay;
        public int Day
        {
            get { return fDay; }
            set { SetPropertyValue<int>("Day", ref fDay, value); }
        }

        private bool fFixed;
        public bool Fixed
        {
            get { return fFixed; }
            set { SetPropertyValue<bool>("Fixed", ref fFixed, value); }
        }
    }
}