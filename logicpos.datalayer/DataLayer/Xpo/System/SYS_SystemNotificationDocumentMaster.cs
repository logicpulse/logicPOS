using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_SystemNotificationDocumentMaster : XPGuidObject
    {
        public SYS_SystemNotificationDocumentMaster() : base() { }
        public SYS_SystemNotificationDocumentMaster(Session session) : base(session) { }

        //SystemNotification One <> Many DocumentFinanceMaster
        SYS_SystemNotification fNotification;
        [Association(@"SystemNotificationReferenceDocumentFinanceMaster")]
        public SYS_SystemNotification Notification
        {
            get { return fNotification; }
            set { SetPropertyValue<SYS_SystemNotification>("Notification", ref fNotification, value); }
        }

        //DocumentFinanceMaster One <> Many SystemNotification
        FIN_DocumentFinanceMaster fDocumentMaster;
        [Association(@"DocumentFinanceMasterReferenceSystemNotification")]
        public FIN_DocumentFinanceMaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<FIN_DocumentFinanceMaster>("DocumentMaster", ref fDocumentMaster, value); }
        }
    }
}
