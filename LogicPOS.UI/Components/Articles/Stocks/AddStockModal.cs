using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LogicPOS.Api.Extensions;

namespace LogicPOS.UI.Components.Modals
{
    public class AddStockModal : Modal
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        #region Components
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private PageTextBox TxtSupplier { get; set; }
        private PageTextBox TxtDate { get; set; }
        private PageTextBox TxtDocumnetNumber { get; set; }
        private PageTextBox TxtNotes { get; set; }
        private AddArticlesBox AddArticlesBox { get; } = new AddArticlesBox();
        public HashSet<IValidatableField> ValidatableFields { get; private set; } = new HashSet<IValidatableField>();
        #endregion

        public AddStockModal(Window parent) : base(parent,
                                                   GeneralUtils.GetResourceByName("window_title_dialog_article_stock"),
                                                   new Size(500, 660),
                                                   PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_stocks.png")
        {
        }

        private void Initialize()
        {
            InitializeTxtSupplier();
            InitializeTxtDate();
            InitializeTxtDocumnetNumber();
            InitializeTxtNotes();
            ValidatableFields.Add(AddArticlesBox);
            AddEventsHandlers();
        }

        private void AddEventsHandlers()
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
                Articles = AddArticlesBox.GetArticlesStocks()
            };

            return command;
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new PageTextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtDocumnetNumber()
        {
            TxtDocumnetNumber = new PageTextBox(WindowSettings.Source,
                                                GeneralUtils.GetResourceByName("global_document_number"),
                                                isRequired: false,
                                                isValidatable: false,
                                                includeSelectButton: false,
                                                includeKeyBoardButton: true);
        }

        private void InitializeTxtDate()
        {
            TxtDate = new PageTextBox(this,
                                      GeneralUtils.GetResourceByName("global_date"),
                                      isRequired: true,
                                      isValidatable: false,
                                      includeSelectButton: true,
                                      includeKeyBoardButton: false);

            TxtDate.Entry.IsEditable = false;
            TxtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            TxtDate.SelectEntityClicked += TxtDate_SelectEntityClicked;
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

        private void InitializeTxtSupplier()
        {
            TxtSupplier = new PageTextBox(WindowSettings.Source,
                                          GeneralUtils.GetResourceByName("global_supplier"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtSupplier.Entry.IsEditable = false;

            TxtSupplier.SelectEntityClicked += BtnSelectSupplier_Clicked;

            ValidatableFields.Add(TxtSupplier);
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

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        protected override Widget CreateBody()
        {
            Initialize();

            var vbox = new VBox(false, 2);
            vbox.PackStart(TxtSupplier.Component, false, false, 0);
            vbox.PackStart(TxtDate.Component, false, false, 0);
            vbox.PackStart(TxtDocumnetNumber.Component, false, false, 0);
            vbox.PackStart(TxtNotes.Component, false, false, 0);
            vbox.PackStart(AddArticlesBox.Component, true, true, 0);
            return vbox;
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
