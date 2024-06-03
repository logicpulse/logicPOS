using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class sys_systemaudit : XPGuidObject
    {
        public sys_systemaudit() : base() { }
        public sys_systemaudit(Session session) : base(session) { }

        private DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { SetPropertyValue<DateTime>("Date", ref fDate, value); }
        }

        private string fDescription;
        [Size(255)]
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue<string>("Description", ref fDescription, value); }
        }

        private sys_userdetail fUserDetail;
        public sys_userdetail UserDetail
        {
            get { return fUserDetail; }
            set { SetPropertyValue("UserDetail", ref fUserDetail, value); }
        }

        private pos_configurationplaceterminal fTerminal;
        public pos_configurationplaceterminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue("Terminal", ref fTerminal, value); }
        }

        //SystemAuditType One <> Many SystemAudit
        private sys_systemaudittype fAuditType;
        [Association(@"SystemAuditTypeReferencesSystemAudit")]
        public sys_systemaudittype AuditType
        {
            get { return fAuditType; }
            set { SetPropertyValue("AuditType", ref fAuditType, value); }
        }
    }
}