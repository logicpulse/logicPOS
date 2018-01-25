using System;
using DevExpress.Xpo;
using logicpos.datalayer.Enums;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_DocumentOrderTicket : XPGuidObject
    {
        public FIN_DocumentOrderTicket() : base() { }
        public FIN_DocumentOrderTicket(Session session) : base(session) { }

        Int32 fTicketId;
        public Int32 TicketId
        {
            get { return fTicketId; }
            set { SetPropertyValue<Int32>("TicketId", ref fTicketId, value); }
        }

        DateTime fDateStart;
        public DateTime DateStart
        {
            get { return fDateStart; }
            set { SetPropertyValue<DateTime>("DateStart", ref fDateStart, value); }
        }

        PriceType fPriceType;
        public PriceType PriceType
        {
            get { return fPriceType; }
            set { SetPropertyValue<PriceType>("PriceType", ref fPriceType, value); }
        }

        Decimal fDiscount;
        public Decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<Decimal>("Discount", ref fDiscount, value); }
        }

        //DocumentOrderTicket One <> Many DocumentOrderDetail
        [Association(@"DocumentOrderTicketReferencesDocumentOrderDetail", typeof(FIN_DocumentOrderDetail))]
        public XPCollection<FIN_DocumentOrderDetail> OrderDetail
        {
            get { return GetCollection<FIN_DocumentOrderDetail>("OrderDetail"); }
        }

        //DocumentOrderMain One <> Many DocumentOrderTicket
        FIN_DocumentOrderMain fOrderMain;
        [Association(@"DocumentOrderMainReferencesDocumentOrderTicket")]
        public FIN_DocumentOrderMain OrderMain
        {
            get { return fOrderMain; }
            set { SetPropertyValue<FIN_DocumentOrderMain>("OrderMain", ref fOrderMain, value); }
        }

        //ConfigurationPlaceTable One <> Many DocumentOrderTicket
        POS_ConfigurationPlaceTable fPlaceTable;
        [Association(@"ConfigurationPlaceTableReferencesDocumentOrderTicket")]
        public POS_ConfigurationPlaceTable PlaceTable
        {
            get { return fPlaceTable; }
            set { SetPropertyValue<POS_ConfigurationPlaceTable>("PlaceTable", ref fPlaceTable, value); }
        }
    }
}