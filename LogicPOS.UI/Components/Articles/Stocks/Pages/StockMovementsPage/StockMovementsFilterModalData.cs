using System;

namespace LogicPOS.UI.Components.Articles.Stocks.Pages.StockMovementsPage
{
    public struct StockMovementsFilterModalData
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? ArticleId { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
