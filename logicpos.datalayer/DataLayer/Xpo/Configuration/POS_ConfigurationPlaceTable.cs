using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.Enums;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class pos_configurationplacetable : XPGuidObject
    {
        public pos_configurationplacetable() : base() { }
        public pos_configurationplacetable(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(pos_configurationplacetable), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(pos_configurationplacetable), "Code");
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
        pos_configurationplace fPlace;
        [Association(@"ConfigurationPlaceReferencesConfigurationPlaceTable")]
        public pos_configurationplace Place
        {
            get { return fPlace; }
            set { SetPropertyValue<pos_configurationplace>("Place", ref fPlace, value); }
        }

        //ConfigurationPlaceTable One <> Many DocumentOrderMain
        [Association(@"ConfigurationPlaceTableReferencesDocumentOrderMain", typeof(fin_documentordermain))]
        public XPCollection<fin_documentordermain> OrderMain
        {
            get { return GetCollection<fin_documentordermain>("OrderMain"); }
        }

        //ConfigurationPlaceTable One <> Many DocumentOrderTicket
        [Association(@"ConfigurationPlaceTableReferencesDocumentOrderTicket", typeof(fin_documentorderticket))]
        public XPCollection<fin_documentorderticket> OrderTicket
        {
            get { return GetCollection<fin_documentorderticket>("OrderTicket"); }
        }
    }
}