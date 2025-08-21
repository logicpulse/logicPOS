using Gtk;
using LogicPOS.Api.Features.Articles.GetArticles;
using LogicPOS.Api.Features.Articles.StockManagement.GetStockMovements;
using LogicPOS.Api.Features.Articles.Stocks.Common;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles.Stocks.Movements;
using LogicPOS.UI.Components.Articles.Stocks.Pages.StockMovementsPage;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class StockMovementsPage : Page<StockMovementViewModel>
    {
        public GetStockMovementsQuery CurrentQuery { get; private set; } = GetDefaultQuery();
        public PaginatedResult<StockMovementViewModel> Movements { get; private set; }
        private IconButtonWithText BtnOpenDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument);
        public event EventHandler PageChanged;
       
        public StockMovementsPage(Window parent) : base(parent)
        {
            RemoveForbiddenButtons();
            AddOpenDocumentButton();
            AddEventHandlers();
        }

        public StockMovementsPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
        }

        public override void Search(string searchText)
        {
            CurrentQuery = new GetStockMovementsQuery { Search = searchText };
            Refresh();
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

            if (Movements.Items != null)
            {
                _entities.AddRange(Movements.Items);
            }
           
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            if (mode == EntityEditionModalMode.Insert)
            {
                return RunInsertModal();
            }

            if (SelectedEntity.Quantity < 0)
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

        protected override DeleteCommand GetDeleteCommand() => null;

    }
}
