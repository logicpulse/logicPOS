using System;
using DevExpress.Xpo;
using logicpos.datalayer.Enums;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_documentorderticket : XPGuidObject
    {
        public fin_documentorderticket() : base() { }
        public fin_documentorderticket(Session session) : base(session) { }

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
        [Association(@"DocumentOrderTicketReferencesDocumentOrderDetail", typeof(fin_documentorderdetail))]
        public XPCollection<fin_documentorderdetail> OrderDetail
        {
            get { return GetCollection<fin_documentorderdetail>("OrderDetail"); }
        }

        //DocumentOrderMain One <> Many DocumentOrderTicket
        fin_documentordermain fOrderMain;
        [Association(@"DocumentOrderMainReferencesDocumentOrderTicket")]
        public fin_documentordermain OrderMain
        {
            get { return fOrderMain; }
            set { SetPropertyValue<fin_documentordermain>("OrderMain", ref fOrderMain, value); }
        }

        //ConfigurationPlaceTable One <> Many DocumentOrderTicket
        pos_configurationplacetable fPlaceTable;
        [Association(@"ConfigurationPlaceTableReferencesDocumentOrderTicket")]
        public pos_configurationplacetable PlaceTable
        {
            get { return fPlaceTable; }
            set { SetPropertyValue<pos_configurationplacetable>("PlaceTable", ref fPlaceTable, value); }
        }
    }
}