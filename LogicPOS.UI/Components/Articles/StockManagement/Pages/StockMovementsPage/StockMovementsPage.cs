using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.StockManagement.GetStockMovements;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Errors;
using LogicPOS.UI.PDFViewer;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class StockMovementsPage : Page<StockMovement>
    {
        public GetStockMovementsQuery CurrentQuery { get; private set; } = GetDefaultQuery();
        public PaginatedResult<StockMovement> Movements { get; private set; }
        private IconButtonWithText BtnOpenDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument);

        public StockMovementsPage(Window parent) : base(parent)
        {
            RemoveForbiddenButtons();
            AddOpenDocumentButton();
            AddEventHandlers();
        }


        private void AddEventHandlers()
        {
            Navigator.SearchBox.BtnMore.Clicked += BtnMore_Clicked;
        }

        private void AddOpenDocumentButton()
        {
            BtnOpenDocument.ButtonLabel.Text = "Documento";
            BtnOpenDocument.Clicked += BtnOpenDocument_Clicked;
            Navigator.RightButtons.PackStart(BtnOpenDocument, false, false, 0);
        }

        private void RemoveForbiddenButtons()
        {
            Navigator.RightButtons.Remove(Navigator.BtnView);
            Navigator.RightButtons.Remove(Navigator.BtnDelete);
        }

        protected override void LoadEntities()
        {
            var getMovements = _mediator.Send(CurrentQuery).Result;

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
            if (mode == EntityEditionModalMode.Insert)
            {
                return RunInsertModal();
            }

            if(SelectedEntity.Quantity < 0)
            {
                return (int)ResponseType.Cancel;
            }

            return RunUpdateModal();
        }

        private int RunUpdateModal()
        {
            var modal = new UpdateStockMovementModal(SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        private int RunInsertModal()
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
