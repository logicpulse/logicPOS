using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.StockManagement.UpdateStockMovement;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Pickers;
using LogicPOS.Utility;
using System;
using System.IO;
using LogicPOS.Api.Extensions;
using LogicPOS.UI.Errors;
using LogicPOS.Api.Features.Articles.Stocks.Common;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UpdateStockMovementModal : EntityEditionModal<StockMovementViewModel>
    {
        public UpdateStockMovementModal(StockMovementViewModel entity) : base(EntityEditionModalMode.Update, entity)
        {
        }

        protected override void AddEntity() => throw new System.NotImplementedException();
        
        protected override void ShowEntityData()
        {
            TxtSupplier.Text = _entity.Customer;
            TxtDate.Text = _entity.Date.ToString("yyyy-MM-dd");
            TxtDocumnetNumber.Text = _entity.DocumentNumber;
            TxtQuantity.Text = _entity.Quantity.ToString();
            TxtPrice.Text = _entity.Price.ToString();
        }

        private Guid? GetSupplierId() => (TxtSupplier.SelectedEntity == null) ? (Guid?)null : (TxtSupplier.SelectedEntity as ApiEntity).Id;

        protected override void UpdateEntity()
        {
            var command = new UpdateStockMovementCommand
            {
                Id = _entity.Id,
                SupplierId = GetSupplierId(),
                Date = TxtDate.Text.FromISO8601DateOnly(),
                DocumentNumber = TxtDocumnetNumber.Text,
                Quantity = Convert.ToInt32(Convert.ToDecimal(TxtQuantity.Text)),
                Price = Convert.ToDecimal(TxtPrice.Text),
                ExternalDocument = TxtDocumnetNumber.SelectedEntity as string
            };

            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
            }
        }
        
        private void TxtDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(this);
            ResponseType response = (ResponseType)dateTimePicker.Run();
            dateTimePicker.Destroy();

            if (response == ResponseType.Ok)
            {
                TxtDate.Text = dateTimePicker.Calendar.Date.ToString("yyyy-MM-dd");
            }
        }

        private void BtnSelectSupplier_Clicked(object sender, EventArgs e)
        {
            var page = new CustomersPage(null, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<Customer>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtSupplier.Text = page.SelectedEntity.Name;
                TxtSupplier.SelectedEntity = page.SelectedEntity;
            }
        }

        private void BtnSelectDocumentNumber_Clicked(object sender, EventArgs e)
        {
            var path = FilePicker.GetOpenFilePath(this, "Selecionar Documento", FilePicker.GetFileFilterPDF());

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            TxtDocumnetNumber.Text = System.IO.Path.GetFileNameWithoutExtension(path);
            TxtDocumnetNumber.SelectedEntity = Convert.ToBase64String(File.ReadAllBytes(path));
        }
    }
}
