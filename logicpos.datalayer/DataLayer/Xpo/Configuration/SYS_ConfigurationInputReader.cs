using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_configurationinputreader : XPGuidObject
    {
        public sys_configurationinputreader() : base() { }
        public sys_configurationinputreader(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(sys_configurationinputreader), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(sys_configurationinputreader), "Code");
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
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fReaderSizes;
        public string ReaderSizes
        {
            get { return fReaderSizes; }
            set { SetPropertyValue<string>("ReaderSizes", ref fReaderSizes, value); }
        }

        //ConfigurationHardwareInputReader One <> Many CConfigurationPlaceTerminal
        [Association(@"ConfigurationHardwareInputReader1ReferencesConfigurationPlaceTerminal", typeof(pos_configurationplaceterminal))]
        public XPCollection<pos_configurationplaceterminal> TerminalsInputReader1
        {
            get { return GetCollection<pos_configurationplaceterminal>("TerminalsInputReader1"); }
        }

        [Association(@"ConfigurationHardwareInputReader2ReferencesConfigurationPlaceTerminal", typeof(pos_configurationplaceterminal))]
        public XPCollection<pos_configurationplaceterminal> TerminalsInputReader2
        {
            get { return GetCollection<pos_configurationplaceterminal>("TerminalsInputReader2"); }
        }
    }
}
