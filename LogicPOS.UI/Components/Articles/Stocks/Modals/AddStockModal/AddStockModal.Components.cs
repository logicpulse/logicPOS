using LogicPOS.Api.Entities;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Enums;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

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

            TxtSupplier.Entry.IsEditable = true;
            var suppliers= CustomersService.Customers.Select (p => (p as object, p.Name)).ToList();
            TxtSupplier.WithAutoCompletion(suppliers);
            TxtSupplier.OnCompletionSelected += s => SelectSupplier(s as Customer);
            TxtSupplier.SelectEntityClicked += BtnSelectSupplier_Clicked;
            TxtSupplier.Entry.Changed += TxtSupplier_Changed;
            ValidatableFields.Add(TxtSupplier);
        }

        private void SelectSupplier(Customer supplier)
        {
            TxtSupplier.SelectedEntity = supplier;
            TxtSupplier.Text = supplier.Name;
            
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

            TxtDate.Entry.IsEditable = true;
            TxtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            TxtDate.SelectEntityClicked += TxtDate_SelectEntityClicked;
        }
    }
}
