﻿using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;

namespace LogicPOS.Domain.Entities
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
            set { SetPropertyValue("Notification", ref fNotification, value); }
        }

        //DocumentFinanceMaster One <> Many SystemNotification
        private fin_documentfinancemaster fDocumentMaster;
        [Association(@"DocumentFinanceMasterReferenceSystemNotification")]
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue("DocumentMaster", ref fDocumentMaster, value); }
        }
    }
}
