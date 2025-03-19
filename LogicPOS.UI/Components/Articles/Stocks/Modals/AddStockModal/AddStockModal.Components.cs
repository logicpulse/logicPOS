using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Enums;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.InputFields;
using System.Collections.Generic;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddStockModal
    {
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private TextBox TxtSupplier { get; set; }
        private TextBox TxtDate { get; set; }
        private TextBox TxtDocumnetNumber { get; set; }
        private TextBox TxtNotes { get; set; }
        private ArticleFieldsContainer ArticlesContainer { get; } = new ArticleFieldsContainer(ArticlesBoxMode.StockManagement);
        public HashSet<IValidatableField> ValidatableFields { get; private set; } = new HashSet<IValidatableField>();

        private void InitializeComponents()
        {
            InitializeTxtSupplier();
            InitializeTxtDate();
            InitializeTxtDocumnetNumber();
            InitializeTxtNotes();
            ValidatableFields.Add(ArticlesContainer);
            AddEventHandlers();
        }

        private void InitializeTxtSupplier()
        {
            TxtSupplier = new TextBox(WindowSettings.Source,
                                          GeneralUtils.GetResourceByName("global_supplier"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false,
                                          style: TextBoxStyle.Lite);

            TxtSupplier.Entry.IsEditable = false;

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
                                       includeKeyBoardButton: false,
                                       style: TextBoxStyle.Lite);
        }

        private void InitializeTxtDocumnetNumber()
        {
            TxtDocumnetNumber = new TextBox(WindowSettings.Source,
                                            GeneralUtils.GetResourceByName("global_document_number"),
                                            isRequired: false,
                                            isValidatable: false,
                                            includeSelectButton: true,
                                            includeKeyBoardButton: false,
                                            style: TextBoxStyle.Lite);

            TxtDocumnetNumber.SelectEntityClicked += BtnSelectDocumentNumber_Clicked;
        }

        private void InitializeTxtDate()
        {
            TxtDate = new TextBox(this,
                                      GeneralUtils.GetResourceByName("global_date"),
                                      isRequired: true,
                                      isValidatable: false,
                                      includeSelectButton: true,
                                      includeKeyBoardButton: false,
                                      style: TextBoxStyle.Lite);

            TxtDate.Entry.IsEditable = false;
            TxtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            TxtDate.SelectEntityClicked += TxtDate_SelectEntityClicked;
        }
    }
}
