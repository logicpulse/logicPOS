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
    public class pos_worksessionperiod : XPGuidObject
    {
        public pos_worksessionperiod() : base() { }
        public pos_worksessionperiod(Session session) : base(session) { }

        private WorkSessionPeriodType fPeriodType;
        public WorkSessionPeriodType PeriodType
        {
            get { return fPeriodType; }
            set { SetPropertyValue<WorkSessionPeriodType>("PeriodType", ref fPeriodType, value); }
        }

        private WorkSessionPeriodStatus fSessionStatus;
        public WorkSessionPeriodStatus SessionStatus
        {
            get { return fSessionStatus; }
            set { SetPropertyValue<WorkSessionPeriodStatus>("SessionStatus", ref fSessionStatus, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private DateTime fDateStart;
        public DateTime DateStart
        {
            get { return fDateStart; }
            set { SetPropertyValue<DateTime>("DateStart", ref fDateStart, value); }
        }

        private DateTime fDateEnd;
        public DateTime DateEnd
        {
            get { return fDateEnd; }
            set { SetPropertyValue<DateTime>("DateEnd", ref fDateEnd, value); }
        }

        private pos_configurationplaceterminal fTerminal;
        public pos_configurationplaceterminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue<pos_configurationplaceterminal>("Terminal", ref fTerminal, value); }
        }

        //RECURSIVE RelationShip : WorkSessionPeriod One (Type Day) <> Many WorkSessionPeriod (Type Terminal)
        [Association(@"WorkSessionPeriodReferencesWorkSessionPeriod", typeof(pos_worksessionperiod))]
        public XPCollection<pos_worksessionperiod> Child
        {
            get { return GetCollection<pos_worksessionperiod>("Child"); }
        }

        //RECURSIVE RelationShip : WorkSessionPeriod One (Type Day) <> Many WorkSessionPeriod (Type Terminal)
        private pos_worksessionperiod fParent;
        [Association(@"WorkSessionPeriodReferencesWorkSessionPeriod")]
        public pos_worksessionperiod Parent
        {
            get { return fParent; }
            set { SetPropertyValue<pos_worksessionperiod>("Parent", ref fParent, value); }
        }

        //WorkSessionPeriod One <> Many WorkSessionMovement
        [Association(@"WorkSessionPeriodReferencesWorkSessionMovement", typeof(pos_worksessionmovement))]
        public XPCollection<pos_worksessionmovement> Movement
        {
            get { return GetCollection<pos_worksessionmovement>("Movement"); }
        }

        //WorkSessionPeriod One <> Many WorkSessionPeriodTotal
        [Association(@"WorkSessionPeriodReferencesWorkSessionPeriodTotal", typeof(pos_worksessionperiodtotal))]
        public XPCollection<pos_worksessionperiodtotal> TotalPeriod
        {
            get { return GetCollection<pos_worksessionperiodtotal>("TotalPeriod"); }
        }
    }
}
