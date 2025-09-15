using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Extensions;
using LogicPOS.Api.Features.Articles.StockManagement.AddStockMovement;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Pickers;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddFullStockMovementModal : Modal
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;

        public AddFullStockMovementModal(Window parent) : base(parent,
                                                   LocalizedString.Instance["window_title_dialog_article_stock"],
                                                   new Size(500, 660),
                                                   AppSettings.Paths.Images + @"Icons\Windows\icon_window_stocks.png",
                                                   windowMode: true)
        {
        }

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {

            if (AllFieldsAreValid() == false)
            {
                Validate();
                return;
            }

            var result = _mediator.Send(CreateCommand()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                Run();
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

        private void TxtSupplier_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtSupplier.Text))
            {
                TxtSupplier.Clear();
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

        protected void Validate()
        {
            if (AllFieldsAreValid())
            {
                return;
            }

            ValidationUtilities.ShowValidationErrors(ValidatableFields);

            Run();
        }

        protected bool AllFieldsAreValid()
        {
            return ValidatableFields.All(txt => txt.IsValid());
        }

        private AddStockMovementCommand CreateCommand()
        {
            var command = new AddStockMovementCommand
            {
                Date = TxtDate.Text.FromISO8601DateOnly(),
                DocumentNumber = TxtDocumnetNumber.Text,
                Notes = TxtNotes.Text,
                SupplierId = (TxtSupplier.SelectedEntity as Customer).Id,
                Items = ArticlesContainer.GetStockMovementItems(),
                ExternalDocument = TxtDocumnetNumber.SelectedEntity as string
            };

            return command;
        }
    }
}
