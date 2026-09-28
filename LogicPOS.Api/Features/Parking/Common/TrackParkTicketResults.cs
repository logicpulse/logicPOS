using System;

namespace LogicPOS.Api.Features.Parking.Common
{
    public class TrackParkTicketQuoteResult
    {
        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public int NPedido { get; set; }
        public string ArticleCode { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
        public decimal DueAmount { get; set; }
        public bool AlreadyPaid { get; set; }
        public bool AlreadyExit { get; set; }
    }

    public class TrackParkTicketPaidResult
    {
        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public bool AlreadyPaid { get; set; }
        public int NPedido { get; set; }
    }
}
