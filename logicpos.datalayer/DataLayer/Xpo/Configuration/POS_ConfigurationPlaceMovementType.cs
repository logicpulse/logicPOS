using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer;
using logicpos.datalayer.Enums;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class POS_ConfigurationPlaceMovementType : XPGuidObject
    {
        public POS_ConfigurationPlaceMovementType() : base() { }
        public POS_ConfigurationPlaceMovementType(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("POS_ConfigurationPlaceMovementType", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("POS_ConfigurationPlaceMovementType", "Code");
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
        [Association(@"ConfigurationPlaceMovementTypeReferencesConfigurationPlace", typeof(POS_ConfigurationPlace))]
        public XPCollection<POS_ConfigurationPlace> Place
        {
            get { return GetCollection<POS_ConfigurationPlace>("Place"); }
        }
    }
}