using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_systemaudit : XPGuidObject
    {
        public sys_systemaudit() : base() { }
        public sys_systemaudit(Session session) : base(session) { }

        DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { SetPropertyValue<DateTime>("Date", ref fDate, value); }
        }

        string fDescription;
        [Size(255)]
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue<string>("Description", ref fDescription, value); }
        }

        sys_userdetail fUserDetail;
        public sys_userdetail UserDetail
        {
            get { return fUserDetail; }
            set { SetPropertyValue<sys_userdetail>("UserDetail", ref fUserDetail, value); }
        }

        pos_configurationplaceterminal fTerminal;
        public pos_configurationplaceterminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue<pos_configurationplaceterminal>("Terminal", ref fTerminal, value); }
        }

        //SystemAuditType One <> Many SystemAudit
        sys_systemaudittype fAuditType;
        [Association(@"SystemAuditTypeReferencesSystemAudit")]
        public sys_systemaudittype AuditType
        {
            get { return fAuditType; }
            set { SetPropertyValue<sys_systemaudittype>("AuditType", ref fAuditType, value); }
        }
    }
}