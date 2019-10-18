using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class pos_worksessionmovementtype : XPGuidObject
    {
        public pos_worksessionmovementtype() : base() { }
        public pos_worksessionmovementtype(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(pos_worksessionmovementtype), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(pos_worksessionmovementtype), "Code");
        }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        UInt32 fCode;
        [Indexed(Unique = true)]
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        string fToken;
        [Size(100)]
        [Indexed(Unique = true)]
        public String Token
        {
            get { return fToken; }
            set { SetPropertyValue<String>("Token", ref fToken, value); }
        }

        string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        string fResourceString;
        [Indexed(Unique = true)]
        public string ResourceString
        {
            get { return fResourceString; }
            set { SetPropertyValue<string>("ResourceString", ref fResourceString, value); }
        }

        string fButtonIcon;
        [Size(255)]
        public string ButtonIcon
        {
            get { return fButtonIcon; }
            set { SetPropertyValue<string>("ButtonIcon", ref fButtonIcon, value); }
        }

        bool fCashDrawerMovement;
        public bool CashDrawerMovement
        {
            get { return fCashDrawerMovement; }
            set { SetPropertyValue<bool>("CashDrawerMovement", ref fCashDrawerMovement, value); }
        }

        //WorkSessionMovementType One <> Many WorkSessionMovement
        [Association(@"WorkSessionMovementTypeReferencesWorkSessionMovement", typeof(pos_worksessionmovement))]
        public XPCollection<pos_worksessionmovement> Movement
        {
            get { return GetCollection<pos_worksessionmovement>("Movement"); }
        }
    }
}
