using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class pos_worksessionmovementtype : Entity
    {
        public pos_worksessionmovementtype() : base() { }
        public pos_worksessionmovementtype(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(pos_worksessionmovementtype), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(pos_worksessionmovementtype), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue("Code", ref fCode, value); }
        }

        private string fToken;
        [Size(100)]
        [Indexed(Unique = true)]
        public string Token
        {
            get { return fToken; }
            set { SetPropertyValue<string>("Token", ref fToken, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fResourceString;
        [Indexed(Unique = true)]
        public string ResourceString
        {
            get { return fResourceString; }
            set { SetPropertyValue<string>("ResourceString", ref fResourceString, value); }
        }

        private string fButtonIcon;
        [Size(255)]
        public string ButtonIcon
        {
            get { return fButtonIcon; }
            set { SetPropertyValue<string>("ButtonIcon", ref fButtonIcon, value); }
        }

        private bool fCashDrawerMovement;
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
