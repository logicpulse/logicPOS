using System;

namespace LogicPOS.UI.Components.Articles.Stocks.Modals.Filters
{
    public struct ArticleHistoryFilterModalData
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? ArticleId { get; set; }
        public string SerialNumber { get; set; }
    }
}
