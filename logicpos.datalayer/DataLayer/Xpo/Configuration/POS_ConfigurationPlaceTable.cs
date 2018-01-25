using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.Enums;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class POS_ConfigurationPlaceTable : XPGuidObject
    {
        public POS_ConfigurationPlaceTable() : base() { }
        public POS_ConfigurationPlaceTable(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("POS_ConfigurationPlaceTable", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("POS_ConfigurationPlaceTable", "Code");
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

        string fButtonImage;
        [Size(255)]
        public string ButtonImage
        {
            get { return fButtonImage; }
            set { SetPropertyValue<string>("ButtonImage", ref fButtonImage, value); }
        }

        decimal fDiscount;
        public decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<decimal>("Discount", ref fDiscount, value); }
        }

        TableStatus fTableStatus;
        public TableStatus TableStatus
        {
            get { return fTableStatus; }
            set { SetPropertyValue<TableStatus>("TableStatus", ref fTableStatus, value); }
        }

        Decimal fTotalOpen;
        public Decimal TotalOpen
        {
            get { return fTotalOpen; }
            set { SetPropertyValue<Decimal>("TotalOpen", ref fTotalOpen, value); }
        }

        DateTime fDateTableOpen;
        public DateTime DateTableOpen
        {
            get { return fDateTableOpen; }
            set { SetPropertyValue<DateTime>("DateTableOpen", ref fDateTableOpen, value); }
        }

        DateTime fDateTableClosed;
        public DateTime DateTableClosed
        {
            get { return fDateTableClosed; }
            set { SetPropertyValue<DateTime>("DateTableClosed", ref fDateTableClosed, value); }
        }

        //ConfigurationPlace One <> Many ConfigurationPlaceTable
        POS_ConfigurationPlace fPlace;
        [Association(@"ConfigurationPlaceReferencesConfigurationPlaceTable")]
        public POS_ConfigurationPlace Place
        {
            get { return fPlace; }
            set { SetPropertyValue<POS_ConfigurationPlace>("Place", ref fPlace, value); }
        }

        //ConfigurationPlaceTable One <> Many DocumentOrderMain
        [Association(@"ConfigurationPlaceTableReferencesDocumentOrderMain", typeof(FIN_DocumentOrderMain))]
        public XPCollection<FIN_DocumentOrderMain> OrderMain
        {
            get { return GetCollection<FIN_DocumentOrderMain>("OrderMain"); }
        }

        //ConfigurationPlaceTable One <> Many DocumentOrderTicket
        [Association(@"ConfigurationPlaceTableReferencesDocumentOrderTicket", typeof(FIN_DocumentOrderTicket))]
        public XPCollection<FIN_DocumentOrderTicket> OrderTicket
        {
            get { return GetCollection<FIN_DocumentOrderTicket>("OrderTicket"); }
        }
    }
}