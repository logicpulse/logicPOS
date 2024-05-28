using DevExpress.Xpo;
using logicpos.datalayer.Xpo;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_configurationpoledisplay : XPGuidObject
    {
        public sys_configurationpoledisplay() : base() { }
        public sys_configurationpoledisplay(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(sys_configurationpoledisplay), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(sys_configurationpoledisplay), "Code");
            VID = "0x0000";
            PID = "0x0000";
            EndPoint = "Ep01";
            CodeTable = "0x10";
            DisplayCharactersPerLine = 20;
            GoToStandByInSeconds = 60;
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

        private string fVID;
        public string VID
        {
            get { return fVID; }
            set { SetPropertyValue<string>("VID", ref fVID, value); }
        }

        private string fPID;
        public string PID
        {
            get { return fPID; }
            set { SetPropertyValue<string>("PID", ref fPID, value); }
        }

        private string fEndPoint;
        public string EndPoint
        {
            get { return fEndPoint; }
            set { SetPropertyValue<string>("EndPoint", ref fEndPoint, value); }
        }

        private string fComPort;
        public string COM
        {
            get { return fComPort; }
            set { SetPropertyValue<string>("COM", ref fComPort, value); }
        }

        private string fCodeTable;
        public string CodeTable
        {
            get { return fCodeTable; }
            set { SetPropertyValue<string>("CodeTable", ref fCodeTable, value); }
        }

        private uint fDisplayCharactersPerLine;
        public uint DisplayCharactersPerLine
        {
            get { return fDisplayCharactersPerLine; }
            set { SetPropertyValue<UInt32>("DisplayCharactersPerLine", ref fDisplayCharactersPerLine, value); }
        }

        private uint fGoToStandByInSeconds;
        public uint GoToStandByInSeconds
        {
            get { return fGoToStandByInSeconds; }
            set { SetPropertyValue<UInt32>("GoToStandByInSeconds", ref fGoToStandByInSeconds, value); }
        }

        private string fStandByLine1;
        public string StandByLine1
        {
            get { return fStandByLine1; }
            set { SetPropertyValue<string>("StandByLine1", ref fStandByLine1, value); }
        }

        private string fStandByLine2;
        public string StandByLine2
        {
            get { return fStandByLine2; }
            set { SetPropertyValue<string>("StandByLine2", ref fStandByLine2, value); }
        }

        //ConfigurationHardwarePoleDisplay One <> Many CConfigurationPlaceTerminal
        [Association(@"ConfigurationHardwarePoleDisplayReferencesConfigurationPlaceTerminal", typeof(pos_configurationplaceterminal))]
        public XPCollection<pos_configurationplaceterminal> Terminals
        {
            get { return GetCollection<pos_configurationplaceterminal>("Terminals"); }
        }
    }
}
