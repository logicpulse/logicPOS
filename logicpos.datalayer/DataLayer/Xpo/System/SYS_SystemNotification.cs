using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_SystemNotification : XPGuidObject
    {
        public SYS_SystemNotification() : base() { }
        public SYS_SystemNotification(Session session) : base(session) { }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        string fMessage;
        [Size(SizeAttribute.Unlimited)]
        public string Message
        {
            get { return fMessage; }
            set { SetPropertyValue<string>("Message", ref fMessage, value); }
        }

        Boolean fReaded;
        public Boolean Readed
        {
            get { return fReaded; }
            set { SetPropertyValue<Boolean>("Readed", ref fReaded, value); }
        }

        DateTime fDateRead;
        public DateTime DateRead
        {
            get { return fDateRead; }
            set { SetPropertyValue<DateTime>("DateRead", ref fDateRead, value); }
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

        SYS_UserDetail fUserLastRead;
        public SYS_UserDetail UserLastRead
        {
            get { return fUserLastRead; }
            set { SetPropertyValue<SYS_UserDetail>("UserLastRead", ref fUserLastRead, value); }
        }

        POS_ConfigurationPlaceTerminal fTerminalLastRead;
        public POS_ConfigurationPlaceTerminal TerminalLastRead
        {
            get { return fTerminalLastRead; }
            set { SetPropertyValue<POS_ConfigurationPlaceTerminal>("TerminalLastRead", ref fTerminalLastRead, value); }
        }

        //SystemNotificationType One <> Many SystemNotification
        SYS_SystemNotificationType fNotificationType;
        [Association(@"SystemNotificationTypeReferencesSystemNotification")]
        public SYS_SystemNotificationType NotificationType
        {
            get { return fNotificationType; }
            set { SetPropertyValue<SYS_SystemNotificationType>("NotificationType", ref fNotificationType, value); }
        }

////SystemNotification Many <> Many DocumentFinanceMaster
//[Association(@"SystemNotificationReferencesDocumentFinanceMaster", typeof(FIN_DocumentFinanceMaster))]
//public XPCollection<FIN_DocumentFinanceMaster> DocumentMaster
//{
//    get { return GetCollection<FIN_DocumentFinanceMaster>("DocumentMaster"); }
//}

////SystemNotification One <> Many DocumentFinanceMaster
//SYS_SystemNotification fNotification;
//[Association(@"SystemNotificationReferencesDocumentFinanceMaster")]
//public SYS_SystemNotification Notification
//{
//    get { return fNotification; }
//    set { SetPropertyValue<SYS_SystemNotification>("Notification", ref fNotification, value); }
//}

        //SystemNotification One <> Many DocumentFinanceMaster
        [Association(@"SystemNotificationReferenceDocumentFinanceMaster", typeof(SYS_SystemNotificationDocumentMaster))]
        public XPCollection<SYS_SystemNotificationDocumentMaster> Notifications
        {
            get { return GetCollection<SYS_SystemNotificationDocumentMaster>("Notifications"); }
        }
    }
}