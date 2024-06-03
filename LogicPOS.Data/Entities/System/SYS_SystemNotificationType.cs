using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class sys_systemnotificationtype : XPGuidObject
    {
        public sys_systemnotificationtype() : base() { }
        public sys_systemnotificationtype(Session session) : base(session) { }

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
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fMessage;
        [Indexed(Unique = true), Size(255)]
        public string Message
        {
            get { return fMessage; }
            set { SetPropertyValue<string>("Message", ref fMessage, value); }
        }

        //Days to Show Notification
        private int fWarnDaysBefore;
        public int WarnDaysBefore
        {
            get { return fWarnDaysBefore; }
            set { SetPropertyValue<int>("WarnDaysBefore", ref fWarnDaysBefore, value); }
        }

        private sys_userdetail fUserTarget;
        public sys_userdetail UserTarget
        {
            get { return fUserTarget; }
            set { SetPropertyValue("UserTarget", ref fUserTarget, value); }
        }

        private pos_configurationplaceterminal fTerminalTarget;
        public pos_configurationplaceterminal TerminalTarget
        {
            get { return fTerminalTarget; }
            set { SetPropertyValue("TerminalTarget", ref fTerminalTarget, value); }
        }

        //SystemNotificationType One <> Many SystemNotification
        [Association(@"SystemNotificationTypeReferencesSystemNotification", typeof(sys_systemnotification))]
        public XPCollection<sys_systemnotification> Notification
        {
            get { return GetCollection<sys_systemnotification>("Notification"); }
        }
    }
}