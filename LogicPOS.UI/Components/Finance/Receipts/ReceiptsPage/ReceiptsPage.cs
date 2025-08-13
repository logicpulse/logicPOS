using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Documents.GetDocuments;
using LogicPOS.Api.Features.Receipts.GetReceipts;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

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

            if (_entities.Any() == false)
            {
                CustomAlerts.Warning(SourceWindow)
                    .WithMessage("Nenhum dado retornado.")
                    .ShowAlert();
                return;
            }
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            if (SelectedEntity != null)
            {
                DocumentPdfUtils.ViewReceiptPdf(this.SourceWindow, SelectedEntity.Id);
            }

            return (int)ResponseType.None;
        }

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

        public override void Search(string searchText)
        {
            Query = new GetReceiptsQuery { Search = searchText };
            Refresh();
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


    }
}
