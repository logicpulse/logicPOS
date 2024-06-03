using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class sys_systemnotification : XPGuidObject
    {
        public sys_systemnotification() : base() { }
        public sys_systemnotification(Session session) : base(session) { }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private string fMessage;
        [Size(SizeAttribute.Unlimited)]
        public string Message
        {
            get { return fMessage; }
            set { SetPropertyValue<string>("Message", ref fMessage, value); }
        }

        private bool fReaded;
        public bool Readed
        {
            get { return fReaded; }
            set { SetPropertyValue<bool>("Readed", ref fReaded, value); }
        }

        private DateTime fDateRead;
        public DateTime DateRead
        {
            get { return fDateRead; }
            set { SetPropertyValue<DateTime>("DateRead", ref fDateRead, value); }
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

        private sys_userdetail fUserLastRead;
        public sys_userdetail UserLastRead
        {
            get { return fUserLastRead; }
            set { SetPropertyValue("UserLastRead", ref fUserLastRead, value); }
        }

        private pos_configurationplaceterminal fTerminalLastRead;
        public pos_configurationplaceterminal TerminalLastRead
        {
            get { return fTerminalLastRead; }
            set { SetPropertyValue("TerminalLastRead", ref fTerminalLastRead, value); }
        }

        //SystemNotificationType One <> Many SystemNotification
        private sys_systemnotificationtype fNotificationType;
        [Association(@"SystemNotificationTypeReferencesSystemNotification")]
        public sys_systemnotificationtype NotificationType
        {
            get { return fNotificationType; }
            set { SetPropertyValue("NotificationType", ref fNotificationType, value); }
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