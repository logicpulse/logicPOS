using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Extensions;
using LogicPOS.Api.Features.Articles.StockManagement.AddStockMovement;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddStockMovementModal : Modal
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        public AddStockMovementModal(Window parent) : base(parent,
                                                   GeneralUtils.GetResourceByName("window_title_dialog_article_stock"),
                                                   new Size(500, 660),
                                                   PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_stocks.png")
        {
        }

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            Validate();

            if (AllFieldsAreValid() == false)
            {
                return;
            }

            var result = _mediator.Send(CreateCommand()).Result;

            if (result.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(this, result.FirstError);
                Run();
            }
        }

        private AddArticlesStockCommand CreateCommand()
        {
            var command = new AddArticlesStockCommand
            {
                Date = TxtDate.Text.FromISO8601DateOnly(),
                DocumentNumber = TxtDocumnetNumber.Text,
                Notes = TxtNotes.Text,
                SupplierId = (TxtSupplier.SelectedEntity as Customer).Id,
                Items = AddArticlesBox.GetArticlesStocks()
            };

            return command;
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
    }
}
