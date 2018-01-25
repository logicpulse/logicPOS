using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    public enum WorkSessionPeriodType
    {
        Day, Terminal
    }

    public enum WorkSessionPeriodStatus
    {
        Open, Close
    }

    [DeferredDeletion(false)]
    public class POS_WorkSessionPeriod : XPGuidObject
    {
        public POS_WorkSessionPeriod() : base() { }
        public POS_WorkSessionPeriod(Session session) : base(session) { }

        WorkSessionPeriodType fPeriodType;
        public WorkSessionPeriodType PeriodType
        {
            get { return fPeriodType; }
            set { SetPropertyValue<WorkSessionPeriodType>("PeriodType", ref fPeriodType, value); }
        }

        WorkSessionPeriodStatus fSessionStatus;
        public WorkSessionPeriodStatus SessionStatus
        {
            get { return fSessionStatus; }
            set { SetPropertyValue<WorkSessionPeriodStatus>("SessionStatus", ref fSessionStatus, value); }
        }

        string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        DateTime fDateStart;
        public DateTime DateStart
        {
            get { return fDateStart; }
            set { SetPropertyValue<DateTime>("DateStart", ref fDateStart, value); }
        }

        DateTime fDateEnd;
        public DateTime DateEnd
        {
            get { return fDateEnd; }
            set { SetPropertyValue<DateTime>("DateEnd", ref fDateEnd, value); }
        }

        POS_ConfigurationPlaceTerminal fTerminal;
        public POS_ConfigurationPlaceTerminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue<POS_ConfigurationPlaceTerminal>("Terminal", ref fTerminal, value); }
        }

        //RECURSIVE RelationShip : WorkSessionPeriod One (Type Day) <> Many WorkSessionPeriod (Type Terminal)
        [Association(@"WorkSessionPeriodReferencesWorkSessionPeriod", typeof(POS_WorkSessionPeriod))]
        public XPCollection<POS_WorkSessionPeriod> Child
        {
            get { return GetCollection<POS_WorkSessionPeriod>("Child"); }
        }

        //RECURSIVE RelationShip : WorkSessionPeriod One (Type Day) <> Many WorkSessionPeriod (Type Terminal)
        POS_WorkSessionPeriod fParent;
        [Association(@"WorkSessionPeriodReferencesWorkSessionPeriod")]
        public POS_WorkSessionPeriod Parent
        {
            get { return fParent; }
            set { SetPropertyValue<POS_WorkSessionPeriod>("Parent", ref fParent, value); }
        }

        //WorkSessionPeriod One <> Many WorkSessionMovement
        [Association(@"WorkSessionPeriodReferencesWorkSessionMovement", typeof(POS_WorkSessionMovement))]
        public XPCollection<POS_WorkSessionMovement> Movement
        {
            get { return GetCollection<POS_WorkSessionMovement>("Movement"); }
        }

        //WorkSessionPeriod One <> Many WorkSessionPeriodTotal
        [Association(@"WorkSessionPeriodReferencesWorkSessionPeriodTotal", typeof(POS_WorkSessionPeriodTotal))]
        public XPCollection<POS_WorkSessionPeriodTotal> TotalPeriod
        {
            get { return GetCollection<POS_WorkSessionPeriodTotal>("TotalPeriod"); }
        }
    }
}
