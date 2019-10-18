using DevExpress.Xpo;
using logicpos.datalayer.Enums;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_documentordermain : XPGuidObject
    {
        public fin_documentordermain() : base() { }
        public fin_documentordermain(Session session) : base(session) { }

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
        [Association(@"DocumentOrderMainReferencesDocumentOrderTicket", typeof(fin_documentorderticket))]
        public XPCollection<fin_documentorderticket> OrderTicket
        {
            get { return GetCollection<fin_documentorderticket>("OrderTicket"); }
        }

        //ConfigurationPlaceTable One <> Many DocumentOrderMain
        pos_configurationplacetable fPlaceTable;
        [Association(@"ConfigurationPlaceTableReferencesDocumentOrderMain")]
        public pos_configurationplacetable PlaceTable
        {
            get { return fPlaceTable; }
            set { SetPropertyValue<pos_configurationplacetable>("PlaceTable", ref fPlaceTable, value); }
        }
    }
}