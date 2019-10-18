using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_systemnotification : XPGuidObject
    {
        public sys_systemnotification() : base() { }
        public sys_systemnotification(Session session) : base(session) { }

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

        sys_userdetail fUserLastRead;
        public sys_userdetail UserLastRead
        {
            get { return fUserLastRead; }
            set { SetPropertyValue<sys_userdetail>("UserLastRead", ref fUserLastRead, value); }
        }

        pos_configurationplaceterminal fTerminalLastRead;
        public pos_configurationplaceterminal TerminalLastRead
        {
            get { return fTerminalLastRead; }
            set { SetPropertyValue<pos_configurationplaceterminal>("TerminalLastRead", ref fTerminalLastRead, value); }
        }

        //SystemNotificationType One <> Many SystemNotification
        sys_systemnotificationtype fNotificationType;
        [Association(@"SystemNotificationTypeReferencesSystemNotification")]
        public sys_systemnotificationtype NotificationType
        {
            get { return fNotificationType; }
            set { SetPropertyValue<sys_systemnotificationtype>("NotificationType", ref fNotificationType, value); }
        }

////SystemNotification Many <> Many DocumentFinanceMaster
//[Association(@"SystemNotificationReferencesDocumentFinanceMaster", typeof(fin_documentfinancemaster))]
//public XPCollection<fin_documentfinancemaster> DocumentMaster
//{
//    get { return GetCollection<fin_documentfinancemaster>("DocumentMaster"); }
//}

////SystemNotification One <> Many DocumentFinanceMaster
//sys_systemnotification fNotification;
//[Association(@"SystemNotificationReferencesDocumentFinanceMaster")]
//public sys_systemnotification Notification
//{
//    get { return fNotification; }
//    set { SetPropertyValue<sys_systemnotification>("Notification", ref fNotification, value); }
//}

        //SystemNotification One <> Many DocumentFinanceMaster
        [Association(@"SystemNotificationReferenceDocumentFinanceMaster", typeof(sys_systemnotificationdocumentmaster))]
        public XPCollection<sys_systemnotificationdocumentmaster> Notifications
        {
            get { return GetCollection<sys_systemnotificationdocumentmaster>("Notifications"); }
        }
    }
}