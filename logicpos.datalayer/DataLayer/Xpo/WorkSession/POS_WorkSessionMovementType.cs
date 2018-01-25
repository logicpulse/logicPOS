using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class POS_WorkSessionMovementType : XPGuidObject
    {
        public POS_WorkSessionMovementType() : base() { }
        public POS_WorkSessionMovementType(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("POS_WorkSessionMovementType", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("POS_WorkSessionMovementType", "Code");
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
        [Association(@"WorkSessionMovementTypeReferencesWorkSessionMovement", typeof(POS_WorkSessionMovement))]
        public XPCollection<POS_WorkSessionMovement> Movement
        {
            get { return GetCollection<POS_WorkSessionMovement>("Movement"); }
        }
    }
}
