using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_SystemAudit : XPGuidObject
    {
        public SYS_SystemAudit() : base() { }
        public SYS_SystemAudit(Session session) : base(session) { }

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

        SYS_UserDetail fUserDetail;
        public SYS_UserDetail UserDetail
        {
            get { return fUserDetail; }
            set { SetPropertyValue<SYS_UserDetail>("UserDetail", ref fUserDetail, value); }
        }

        POS_ConfigurationPlaceTerminal fTerminal;
        public POS_ConfigurationPlaceTerminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue<POS_ConfigurationPlaceTerminal>("Terminal", ref fTerminal, value); }
        }

        //SystemAuditType One <> Many SystemAudit
        SYS_SystemAuditType fAuditType;
        [Association(@"SystemAuditTypeReferencesSystemAudit")]
        public SYS_SystemAuditType AuditType
        {
            get { return fAuditType; }
            set { SetPropertyValue<SYS_SystemAuditType>("AuditType", ref fAuditType, value); }
        }
    }
}