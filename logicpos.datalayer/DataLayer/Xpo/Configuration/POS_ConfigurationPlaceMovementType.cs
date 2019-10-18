using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer;
using logicpos.datalayer.Enums;
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
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(pos_configurationplacemovementtype), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(pos_configurationplacemovementtype), "Code");
            //In Retail Mode VatDirectSelling is always true;
            if (SettingsApp.AppMode == AppOperationMode.Retail)
            {
                VatDirectSelling = true;
            }
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

        string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        //Use Vat Direct Sale or Normal on Table Vat
        Boolean fVatDirectSelling;
        public Boolean VatDirectSelling
        {
            get { return fVatDirectSelling; }
            set { SetPropertyValue<Boolean>("VatDirectSelling", ref fVatDirectSelling, value); }
        }

        //ConfigurationPlaceMovementType One <> Many ConfigurationPlace
        [Association(@"ConfigurationPlaceMovementTypeReferencesConfigurationPlace", typeof(pos_configurationplace))]
        public XPCollection<pos_configurationplace> Place
        {
            get { return GetCollection<pos_configurationplace>("Place"); }
        }
    }
}