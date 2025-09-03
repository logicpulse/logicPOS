using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CreateDocumentShipToTab
    {
        private TextBox TxtAddress { get; set; }
        private TextBox TxtRegion { get; set; }
        private TextBox TxtZipCode { get; set; }
        private TextBox TxtCity { get; set; }
        private TextBox TxtCountry { get; set; }
        private TextBox TxtDeliveryDate { get; set; }
        private TextBox TxtDeliveryId { get; set; }
        private TextBox TxtWarehouseId { get; set; }
        private TextBox TxtLocationId { get; set; }
    }
}
