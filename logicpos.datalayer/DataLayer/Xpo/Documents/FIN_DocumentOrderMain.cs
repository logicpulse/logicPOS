using DevExpress.Xpo;
using logicpos.datalayer.Enums;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_DocumentOrderMain : XPGuidObject
    {
        public FIN_DocumentOrderMain() : base() { }
        public FIN_DocumentOrderMain(Session session) : base(session) { }

        DateTime fDateStart;
        public DateTime DateStart
        {
            get { return fDateStart; }
            set { SetPropertyValue<DateTime>("DateStart", ref fDateStart, value); }
        }

        OrderStatus fOrderStatus;
        public OrderStatus OrderStatus
        {
            get { return fOrderStatus; }
            set { SetPropertyValue<OrderStatus>("OrderStatus", ref fOrderStatus, value); }
        }

        //DocumentOrderMain One <> Many DocumentOrderTicket
        [Association(@"DocumentOrderMainReferencesDocumentOrderTicket", typeof(FIN_DocumentOrderTicket))]
        public XPCollection<FIN_DocumentOrderTicket> OrderTicket
        {
            get { return GetCollection<FIN_DocumentOrderTicket>("OrderTicket"); }
        }

        //ConfigurationPlaceTable One <> Many DocumentOrderMain
        POS_ConfigurationPlaceTable fPlaceTable;
        [Association(@"ConfigurationPlaceTableReferencesDocumentOrderMain")]
        public POS_ConfigurationPlaceTable PlaceTable
        {
            get { return fPlaceTable; }
            set { SetPropertyValue<POS_ConfigurationPlaceTable>("PlaceTable", ref fPlaceTable, value); }
        }
    }
}