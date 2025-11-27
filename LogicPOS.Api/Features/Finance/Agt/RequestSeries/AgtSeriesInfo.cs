using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Finance.Agt.RequestSeries
{
    public struct AgtSeriesInfo
    {
        public string Code { get; set; }
        public string Quantity { get; set; }
        public string FirstDocumentNo { get; set; }
        public string LastDocumentNo { get; set; }
    }
}
