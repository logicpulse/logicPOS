using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Enums;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddStockMovementModal: Modal
    {
        #region Components
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private TextBox TxtSupplier { get; set; }
        private TextBox TxtDate { get; set; }
        private TextBox TxtDocumentNumber { get; set; }
        private TextBox TxtNotes { get; set; }
        private ArticleFieldsContainer AddArticlesBox { get; } = new ArticleFieldsContainer(ArticlesBoxMode.StockMovement);
        public HashSet<IValidatableField> ValidatableFields { get; private set; } = new HashSet<IValidatableField>();
        #endregion

        private void InitializeComponents()
        {
            InitializeTxtSupplier();
            InitializeTxtDate();
            InitializeTxtDocumentNumber();
            InitializeTxtNotes();
            ValidatableFields.Add(AddArticlesBox);
            AddEventHandlers();
        }
       
        private void InitializeTxtSupplier()
        {
            TxtSupplier = new TextBox(WindowSettings.Source,
                                          GeneralUtils.GetResourceByName("global_supplier"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtSupplier.Entry.IsEditable = true;
            var suppliers = SuppliersForCompletion.Select(s => (s as object, s.Name)).ToList();
            TxtSupplier.WithAutoCompletion(suppliers);
            TxtSupplier.OnCompletionSelected += s => SelectSupplier(s as Customer);
            TxtSupplier.Entry.Changed += TxtSupplier_Changed;
            TxtSupplier.SelectEntityClicked += BtnSelectSupplier_Clicked;

            ValidatableFields.Add(TxtSupplier);
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new TextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtDocumentNumber()
        {
            TxtDocumentNumber = new TextBox(WindowSettings.Source,
                                            GeneralUtils.GetResourceByName("global_document_number"),
                                            isRequired: false,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: true);

            var documentNumbers = DocumentsForCompletion.Select(n => (n as object, n.Number)).ToList();
            TxtDocumentNumber.WithAutoCompletion(documentNumbers);
            TxtDocumentNumber.OnCompletionSelected += s => SelectDocument(s as Document);
            TxtDocumentNumber.Entry.Changed += TxtDocumentNumber_Changed;
            
        }

        private void InitializeTxtDate()
        {
            TxtDate = new TextBox(this,
                                      GeneralUtils.GetResourceByName("global_date"),
                                      isRequired: true,
                                      isValidatable: false,
                                      includeSelectButton: true,
                                      includeKeyBoardButton: false);

            TxtDate.Entry.IsEditable = false;
            TxtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            TxtDate.SelectEntityClicked += TxtDate_SelectEntityClicked;
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
            InitializeComponents();

            var vbox = new VBox(false, 2);
            vbox.PackStart(TxtSupplier.Component, false, false, 0);
            vbox.PackStart(TxtDate.Component, false, false, 0);
            vbox.PackStart(TxtDocumentNumber.Component, false, false, 0);
            vbox.PackStart(TxtNotes.Component, false, false, 0);
            vbox.PackStart(AddArticlesBox.Component, true, true, 0);
            return vbox;
        }

    }
}
