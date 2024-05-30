using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Settings;
using LogicPOS.Settings.Enums;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class pos_configurationplacemovementtype : XPGuidObject
    {
        public pos_configurationplacemovementtype() : base() { }
        public pos_configurationplacemovementtype(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(pos_configurationplacemovementtype), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(pos_configurationplacemovementtype), "Code");
    
            if (AppOperationModeSettings.AppMode == AppOperationMode.Retail)
            {
                VatDirectSelling = true;
            }
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

        //Use Vat Direct Sale or Normal on Table Vat
        private bool fVatDirectSelling;
        public bool VatDirectSelling
        {
            get { return fVatDirectSelling; }
            set { SetPropertyValue<bool>("VatDirectSelling", ref fVatDirectSelling, value); }
        }

        //ConfigurationPlaceMovementType One <> Many ConfigurationPlace
        [Association(@"ConfigurationPlaceMovementTypeReferencesConfigurationPlace", typeof(pos_configurationplace))]
        public XPCollection<pos_configurationplace> Place
        {
            get { return GetCollection<pos_configurationplace>("Place"); }
        }
    }
}