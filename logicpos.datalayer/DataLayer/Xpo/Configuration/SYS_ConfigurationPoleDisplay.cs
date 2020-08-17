using DevExpress.Xpo;
using logicpos.datalayer.App;
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
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(sys_configurationpoledisplay), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(sys_configurationpoledisplay), "Code");
            VID = "0x0000";
            PID = "0x0000";
            EndPoint = "Ep01";
            CodeTable = "0x10";
            DisplayCharactersPerLine = 20;
            GoToStandByInSeconds = 60;
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
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        string fVID;
        public string VID
        {
            get { return fVID; }
            set { SetPropertyValue<string>("VID", ref fVID, value); }
        }

        string fPID;
        public string PID
        {
            get { return fPID; }
            set { SetPropertyValue<string>("PID", ref fPID, value); }
        }

        string fEndPoint;
        public string EndPoint
        {
            get { return fEndPoint; }
            set { SetPropertyValue<string>("EndPoint", ref fEndPoint, value); }
        }

        string fComPort;
        public string COM
        {
            get { return fComPort; }
            set { SetPropertyValue<string>("COM", ref fComPort, value); }
        }

        string fCodeTable;
        public string CodeTable
        {
            get { return fCodeTable; }
            set { SetPropertyValue<string>("CodeTable", ref fCodeTable, value); }
        }

        UInt32 fDisplayCharactersPerLine;
        public UInt32 DisplayCharactersPerLine
        {
            get { return fDisplayCharactersPerLine; }
            set { SetPropertyValue<UInt32>("DisplayCharactersPerLine", ref fDisplayCharactersPerLine, value); }
        }

        UInt32 fGoToStandByInSeconds;
        public UInt32 GoToStandByInSeconds
        {
            get { return fGoToStandByInSeconds; }
            set { SetPropertyValue<UInt32>("GoToStandByInSeconds", ref fGoToStandByInSeconds, value); }
        }

        string fStandByLine1;
        public string StandByLine1
        {
            get { return fStandByLine1; }
            set { SetPropertyValue<string>("StandByLine1", ref fStandByLine1, value); }
        }

        string fStandByLine2;
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
