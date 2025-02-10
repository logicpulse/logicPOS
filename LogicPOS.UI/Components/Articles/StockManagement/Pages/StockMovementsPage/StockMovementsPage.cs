using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.StockManagement.GetStockMovements;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Errors;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class StockMovementsPage : Page<StockMovement>
    {
        public GetStockMovementsQuery Query { get; private set; } = GetDefaultQuery();
        public PaginatedResult<StockMovement> Movements { get; private set; }

        public StockMovementsPage(Window parent,
                                  Dictionary<string, string> options = null) : base(parent, options)
        {
            Navigator.RightButtons.Remove(Navigator.BtnView);
            Navigator.RightButtons.Remove(Navigator.BtnDelete);
            Navigator.RightButtons.Remove(Navigator.BtnUpdate);
        }

        protected override void LoadEntities()
        {
            var getMovements = _mediator.Send(Query).Result;

            if (getMovements.IsError)
            {
                ErrorHandlingService.HandleApiError(getMovements,
                                                    source: SourceWindow);
                return;
            }

            Movements = getMovements.Value;

            _entities.Clear();
            _entities.AddRange(Movements.Items);
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new AddStockModal(SourceWindow);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateMovementTypeColumn());
            GridView.AppendColumn(CreateDateColumn());
            GridView.AppendColumn(CreateEntityColumn());
            GridView.AppendColumn(CreateDocumentNumberColumn());
            GridView.AppendColumn(CreateArticleColumn());
            GridView.AppendColumn(CreateQuantityColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(7));
        }

        protected override DeleteCommand GetDeleteCommand() => null;

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddMovementTypeSorting();
            AddDateSorting();
            AddEntitySorting();
            AddDocumentNumberSorting();
            AddArticleSorting();
            AddQuantitySorting();
            AddUpdatedAtSorting(7);
        }

        private static GetStockMovementsQuery GetDefaultQuery()
        {
            var query = new GetStockMovementsQuery
            {
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                EndDate = DateTime.Now,
            };

            return query;
        }
    }
}
