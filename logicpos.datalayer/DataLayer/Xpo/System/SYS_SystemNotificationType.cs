using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_systemnotificationtype : XPGuidObject
    {
        public sys_systemnotificationtype() : base() { }
        public sys_systemnotificationtype(Session session) : base(session) { }

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

        string fMessage;
        [Indexed(Unique = true), Size(255)]
        public string Message
        {
            get { return fMessage; }
            set { SetPropertyValue<string>("Message", ref fMessage, value); }
        }

        //Days to Show Notification
        int fWarnDaysBefore;
        public int WarnDaysBefore
        {
            get { return fWarnDaysBefore; }
            set { SetPropertyValue<int>("WarnDaysBefore", ref fWarnDaysBefore, value); }
        }

        sys_userdetail fUserTarget;
        public sys_userdetail UserTarget
        {
            get { return fUserTarget; }
            set { SetPropertyValue<sys_userdetail>("UserTarget", ref fUserTarget, value); }
        }

        pos_configurationplaceterminal fTerminalTarget;
        public pos_configurationplaceterminal TerminalTarget
        {
            get { return fTerminalTarget; }
            set { SetPropertyValue<pos_configurationplaceterminal>("TerminalTarget", ref fTerminalTarget, value); }
        }

        //SystemNotificationType One <> Many SystemNotification
        [Association(@"SystemNotificationTypeReferencesSystemNotification", typeof(sys_systemnotification))]
        public XPCollection<sys_systemnotification> Notification
        {
            get { return GetCollection<sys_systemnotification>("Notification"); }
        }
    }
}