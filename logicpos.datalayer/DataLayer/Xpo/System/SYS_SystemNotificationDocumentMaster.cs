using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_systemnotificationdocumentmaster : XPGuidObject
    {
        public sys_systemnotificationdocumentmaster() : base() { }
        public sys_systemnotificationdocumentmaster(Session session) : base(session) { }

        //SystemNotification One <> Many DocumentFinanceMaster
        private sys_systemnotification fNotification;
        [Association(@"SystemNotificationReferenceDocumentFinanceMaster")]
        public sys_systemnotification Notification
        {
            get { return fNotification; }
            set { SetPropertyValue<sys_systemnotification>("Notification", ref fNotification, value); }
        }

        //DocumentFinanceMaster One <> Many SystemNotification
        private fin_documentfinancemaster fDocumentMaster;
        [Association(@"DocumentFinanceMasterReferenceSystemNotification")]
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentMaster", ref fDocumentMaster, value); }
        }
    }
}
