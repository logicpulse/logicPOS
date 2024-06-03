using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Domain.Entities
{

    [DeferredDeletion(false)]
    public class pos_worksessionperiod : XPGuidObject
    {
        public pos_worksessionperiod() : base() { }
        public pos_worksessionperiod(Session session) : base(session) { }

        private WorkSessionPeriodType fPeriodType;
        public WorkSessionPeriodType PeriodType
        {
            get { return fPeriodType; }
            set { SetPropertyValue("PeriodType", ref fPeriodType, value); }
        }

        private WorkSessionPeriodStatus fSessionStatus;
        public WorkSessionPeriodStatus SessionStatus
        {
            get { return fSessionStatus; }
            set { SetPropertyValue("SessionStatus", ref fSessionStatus, value); }
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
            set { SetPropertyValue("Terminal", ref fTerminal, value); }
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
            set { SetPropertyValue("Parent", ref fParent, value); }
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
