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

        private string fButtonImage;
        [Size(255)]
        public string ButtonImage
        {
            get { return fButtonImage; }
            set { SetPropertyValue<string>("ButtonImage", ref fButtonImage, value); }
        }

        private decimal fDiscount;
        public decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<decimal>("Discount", ref fDiscount, value); }
        }

        private TableStatus fTableStatus;
        public TableStatus TableStatus
        {
            get { return fTableStatus; }
            set { SetPropertyValue<TableStatus>("TableStatus", ref fTableStatus, value); }
        }

        private decimal fTotalOpen;
        public decimal TotalOpen
        {
            get { return fTotalOpen; }
            set { SetPropertyValue<decimal>("TotalOpen", ref fTotalOpen, value); }
        }

        private DateTime fDateTableOpen;
        public DateTime DateTableOpen
        {
            get { return fDateTableOpen; }
            set { SetPropertyValue<DateTime>("DateTableOpen", ref fDateTableOpen, value); }
        }

        private DateTime fDateTableClosed;
        public DateTime DateTableClosed
        {
            get { return fDateTableClosed; }
            set { SetPropertyValue<DateTime>("DateTableClosed", ref fDateTableClosed, value); }
        }

        //ConfigurationPlace One <> Many ConfigurationPlaceTable
        private pos_configurationplace fPlace;
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