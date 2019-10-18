using DevExpress.Xpo;
using logicpos.datalayer.App;
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
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(cfg_configurationholidays), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(cfg_configurationholidays), "Code");
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

        string fDescription;
        [Size(255)]
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue<string>("Description", ref fDescription, value); }
        }

        Int32 fYear;
        public Int32 Year
        {
            get { return fYear; }
            set { SetPropertyValue<Int32>("Year", ref fYear, value); }
        }

        Int32 fMonth;
        public Int32 Month
        {
            get { return fMonth; }
            set { SetPropertyValue<Int32>("Month", ref fMonth, value); }
        }
        
        Int32 fDay;
        public Int32 Day
        {
            get { return fDay; }
            set { SetPropertyValue<Int32>("Day", ref fDay, value); }
        }

        bool fFixed;
        public bool Fixed
        {
            get { return fFixed; }
            set { SetPropertyValue<bool>("Fixed", ref fFixed, value); }
        }
    }
}