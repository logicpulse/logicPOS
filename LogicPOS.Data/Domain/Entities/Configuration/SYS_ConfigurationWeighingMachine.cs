using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class sys_configurationweighingmachine : Entity
    {
        public sys_configurationweighingmachine() : base() { }
        public sys_configurationweighingmachine(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(sys_configurationweighingmachine), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(sys_configurationweighingmachine), "Code");
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
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fPortName;
        [Size(4)]
        public string PortName
        {
            get { return fPortName; }
            set { SetPropertyValue<string>("PortName", ref fPortName, value); }
        }

        private uint fBaudRate;
        public uint BaudRate
        {
            get { return fBaudRate; }
            set { SetPropertyValue("BaudRate", ref fBaudRate, value); }
        }

        private string fParity;
        [Size(5)]
        public string Parity
        {
            get { return fParity; }
            set { SetPropertyValue<string>("Parity", ref fParity, value); }
        }

        private string fStopBits;
        [Size(12)]
        public string StopBits
        {
            get { return fStopBits; }
            set { SetPropertyValue<string>("StopBits", ref fStopBits, value); }
        }

        private uint fDataBits;
        public uint DataBits
        {
            get { return fDataBits; }
            set { SetPropertyValue("DataBits", ref fDataBits, value); }
        }

        //ConfigurationHardwareWeighingMachine One <> Many CConfigurationPlaceTerminal
        [Association(@"ConfigurationHardwareWeighingMachineReferencesConfigurationPlaceTerminal", typeof(pos_configurationplaceterminal))]
        public XPCollection<pos_configurationplaceterminal> Terminals
        {
            get { return GetCollection<pos_configurationplaceterminal>("Terminals"); }
        }
    }
}
