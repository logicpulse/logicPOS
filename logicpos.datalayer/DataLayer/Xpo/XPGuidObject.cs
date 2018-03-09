using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [NonPersistent]
    [DeferredDeletion(false)]
    public abstract class XPGuidObject : XPCustomObject
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _isNewRecord = false;

        public XPGuidObject() : base() { }
        public XPGuidObject(Session pSession) : base(pSession) { }

        [Persistent("Oid"), Indexed(Unique = true), Key(true), MemberDesignTimeVisibility(false)]
        private Guid _Oid = Guid.Empty;
        [PersistentAlias("_Oid")]
        public Guid Oid { get { return _Oid; } }

        Boolean fDisabled;
        public Boolean Disabled
        {
            get { return fDisabled; }
            set { SetPropertyValue<Boolean>("Disabled", ref fDisabled, value); }
        }

        String fNotes;
        [Size(SizeAttribute.Unlimited)]
        public String Notes
        {
            get { return fNotes; }
            set { SetPropertyValue<String>("Notes", ref fNotes, value); }
        }

        DateTime fCreatedAt;
        public DateTime CreatedAt
        {
            get { return fCreatedAt; }
            set { SetPropertyValue<DateTime>("CreatedAt", ref fCreatedAt, value); }
        }

        SYS_UserDetail fCreatedBy;
        public SYS_UserDetail CreatedBy
        {
            get { return fCreatedBy; }
            set { SetPropertyValue<SYS_UserDetail>("CreatedBy", ref fCreatedBy, value); }
        }

        POS_ConfigurationPlaceTerminal fCreatedWhere;
        public POS_ConfigurationPlaceTerminal CreatedWhere
        {
            get { return fCreatedWhere; }
            set { SetPropertyValue<POS_ConfigurationPlaceTerminal>("CreatedWhere", ref fCreatedWhere, value); }
        }

        DateTime fUpdatedAt;
        public DateTime UpdatedAt
        {
            get { return fUpdatedAt; }
            set { SetPropertyValue<DateTime>("UpdatedAt", ref fUpdatedAt, value); }
        }

        SYS_UserDetail fUpdatedBy;
        public SYS_UserDetail UpdatedBy
        {
            get { return fUpdatedBy; }
            set { SetPropertyValue<SYS_UserDetail>("UpdatedBy", ref fUpdatedBy, value); }
        }

        POS_ConfigurationPlaceTerminal fUpdatedWhere;
        public POS_ConfigurationPlaceTerminal UpdatedWhere
        {
            get { return fUpdatedWhere; }
            set { SetPropertyValue<POS_ConfigurationPlaceTerminal>("UpdatedWhere", ref fUpdatedWhere, value); }
        }

        DateTime fDeletedAt;
        public DateTime DeletedAt
        {
            get { return fDeletedAt; }
            set { SetPropertyValue<DateTime>("DeletedAt", ref fDeletedAt, value); }
        }

        SYS_UserDetail fDeletedBy;
        public SYS_UserDetail DeletedBy
        {
            get { return fDeletedBy; }
            set { SetPropertyValue<SYS_UserDetail>("DeletedBy", ref fDeletedBy, value); }
        }

        POS_ConfigurationPlaceTerminal fDeletedWhere;
        public POS_ConfigurationPlaceTerminal DeletedWhere
        {
            get { return fDeletedWhere; }
            set { SetPropertyValue<POS_ConfigurationPlaceTerminal>("DeletedWhere", ref fDeletedWhere, value); }
        }

        protected bool IsNewRecord
        {
            get { return (_isNewRecord); }
            set { _isNewRecord = value; }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork) && Session.IsNewObject(this))
            {
                _Oid = XpoDefault.NewGuid();
            }
            //Global Updates
            UpdatedAt = FrameworkUtils.CurrentDateTimeAtomic();
            if (GlobalFramework.LoggedUser != null)
            {
                UpdatedBy = this.Session.GetObjectByKey<SYS_UserDetail>(GlobalFramework.LoggedUser.Oid);
            }
            if (GlobalFramework.LoggedTerminal != null)
            {
                UpdatedWhere = this.Session.GetObjectByKey<POS_ConfigurationPlaceTerminal>(GlobalFramework.LoggedTerminal.Oid);
            }
            if (_isNewRecord)
            {
                OnNewRecordSaving();
            }
            else
            {
                OnRecordSaving();
            }
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Disabled = false;
            DateTime dateTime = FrameworkUtils.CurrentDateTimeAtomic();
            CreatedAt = dateTime;
            UpdatedAt = dateTime;
            if (GlobalFramework.LoggedUser != null)
            {
                UpdatedBy = this.Session.GetObjectByKey<SYS_UserDetail>(GlobalFramework.LoggedUser.Oid);
            }
            if (GlobalFramework.LoggedTerminal != null)
            {
                UpdatedWhere = this.Session.GetObjectByKey<POS_ConfigurationPlaceTerminal>(GlobalFramework.LoggedTerminal.Oid);
            }

            _isNewRecord = true;

            OnAfterConstruction();
        }

        //To be Override by SubClasses
        protected virtual void OnAfterConstruction()
        {
        }

        protected virtual void OnNewRecordSaving()
        {
        }

        protected virtual void OnRecordSaving()
        {
        }
    }
}
