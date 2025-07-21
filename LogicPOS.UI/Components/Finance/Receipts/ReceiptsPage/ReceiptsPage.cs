using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Receipts.GetReceipts;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ReceiptsPage : Page<ReceiptViewModel>
    {
        public GetReceiptsQuery Query { get; private set; } = GetDefaultQuery();
        public PaginatedResult<ReceiptViewModel> Receipts { get; private set; }
        public List<ReceiptViewModel> SelectedReceipts { get; private set; } = new List<ReceiptViewModel>();
        public decimal SelectedReceiptsTotalAmount { get; private set; }
        public event EventHandler PageChanged;

        public ReceiptsPage(Window parent,
                            Dictionary<string, string> options = null) : base(parent, options)
        {
        }

        protected override void LoadEntities()
        {
            var getReceiptsResult = _mediator.Send(Query).Result;

            if (getReceiptsResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getReceiptsResult,
                                                    source: SourceWindow);
                return;
            }

            Receipts = getReceiptsResult.Value;

            _entities.Clear();
            _entities.AddRange(Receipts.Items);
        }

        public void RunFilter()
        {
            var filterModal = new DocumentsFilterModal(SourceWindow);
            var response = (ResponseType)filterModal.Run();
            var query = filterModal.GetReceiptsQuery();
            filterModal.Destroy();

            if (response != ResponseType.Ok)
            {
                return;
            }

            Query = query;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }
        public override int RunModal(EntityEditionModalMode mode) => (int)ResponseType.None;

        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateSelectColumn());
            GridView.AppendColumn(CreateDateColumn());
            GridView.AppendColumn(CreateNumberColumn());
            GridView.AppendColumn(CreateStatusColumn());
            GridView.AppendColumn(CreateEntityColumn());
            GridView.AppendColumn(CreateFiscalNumberColumn());
            GridView.AppendColumn(CreateTotalColumn());
            GridView.AppendColumn(CreateRelatedReceiptsColumn());
        }
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddDateSorting();
            AddNumberSorting();
            AddStatusSorting();
            AddEntitySorting();
            AddFiscalNumberSorting();
        }

        protected override void AddEntitiesToModel(IEnumerable<ReceiptViewModel> receipts)
        {
            var model = (ListStore)GridViewSettings.Model;
            foreach (ReceiptViewModel receipt in receipts)
            {
                model.AppendValues(receipt,false);
            }
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return null;
        }

        private static GetReceiptsQuery GetDefaultQuery()
        {
            var query = new GetReceiptsQuery
            {
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                EndDate = DateTime.Now,
            };

            return query;
        }
    }
}
