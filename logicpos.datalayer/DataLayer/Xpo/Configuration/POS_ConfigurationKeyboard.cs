using DevExpress.Xpo;
using logicpos.datalayer.Xpo;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class pos_configurationkeyboard : XPGuidObject
    {
        public pos_configurationkeyboard() : base() { }
        public pos_configurationkeyboard(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(pos_configurationkeyboard), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(pos_configurationkeyboard), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fLanguage;
        public string Language
        {
            get { return fLanguage; }
            set { SetPropertyValue<string>("Language", ref fLanguage, value); }
        }

        private string fVirtualKeyboard;
        public string VirtualKeyboard
        {
            get { return fVirtualKeyboard; }
            set { SetPropertyValue<string>("VirtualKeyboard", ref fVirtualKeyboard, value); }
        }
    }
}