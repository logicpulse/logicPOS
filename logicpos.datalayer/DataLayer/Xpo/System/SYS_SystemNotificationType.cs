using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_SystemNotificationType : XPGuidObject
    {
        public SYS_SystemNotificationType() : base() { }
        public SYS_SystemNotificationType(Session session) : base(session) { }

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

        SYS_UserDetail fUserTarget;
        public SYS_UserDetail UserTarget
        {
            get { return fUserTarget; }
            set { SetPropertyValue<SYS_UserDetail>("UserTarget", ref fUserTarget, value); }
        }

        POS_ConfigurationPlaceTerminal fTerminalTarget;
        public POS_ConfigurationPlaceTerminal TerminalTarget
        {
            get { return fTerminalTarget; }
            set { SetPropertyValue<POS_ConfigurationPlaceTerminal>("TerminalTarget", ref fTerminalTarget, value); }
        }

        //SystemNotificationType One <> Many SystemNotification
        [Association(@"SystemNotificationTypeReferencesSystemNotification", typeof(SYS_SystemNotification))]
        public XPCollection<SYS_SystemNotification> Notification
        {
            get { return GetCollection<SYS_SystemNotification>("Notification"); }
        }
    }
}