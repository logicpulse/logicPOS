using Gtk;
using logicpos;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents.CreateDocumentModal
{
    public class CreateDocumentDocumentTab : ModalTab
    {
        private PageTextBox TxtDocumentType { get; set; }
        private PageTextBox TxtPaymnentCondition { get; set; }
        private PageTextBox TxtPaymentMethod { get; set; }
        private PageTextBox TxtCurrency { get; set; }
        private PageTextBox TxtOriginDocument { get; set; }
        private PageTextBox TxtCopyDocument { get; set; }
        private PageTextBox TxtNotes { get; set; }

        public CreateDocumentDocumentTab(Window parent) : base(parent: parent,
                                                  name: GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page1"),
                                                  icon: PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_1_new_document.png")
        {
            Initialize();
            Design();
        }

        private void Initialize()
        {
            InitializeTxtDocumentType();
            InitializeTxtPaymnentCondition();
            InitializeTxtPaymentMethod();
            InitializeTxtCurrency();
            InitializeTxtOriginDocument();
            InitializeTxtCopyDocument();
            InitializeTxtNotes();
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new PageTextBox(SourceWindow,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);

            TxtNotes.Entry.IsEditable = true;
        }

        private void InitializeTxtCopyDocument()
        {
            TxtCopyDocument = new PageTextBox(SourceWindow,
                                              GeneralUtils.GetResourceByName("global_copy_finance_document"),
                                              isRequired: false,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtCopyDocument.Entry.IsEditable = false;

            TxtCopyDocument.SelectEntityClicked += BtnSelectCopyDocument_Clicked;
        }

        private void BtnSelectCopyDocument_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InitializeTxtOriginDocument()
        {
            TxtOriginDocument = new PageTextBox(SourceWindow,
                                                GeneralUtils.GetResourceByName("global_source_finance_document"),
                                                isRequired: false,
                                                isValidatable: false,
                                                includeSelectButton: true,
                                                includeKeyBoardButton: false);

            TxtOriginDocument.Entry.IsEditable = false;

            TxtOriginDocument.SelectEntityClicked += BtnSelectOriginDocument_Clicked;
        }

        private void BtnSelectOriginDocument_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InitializeTxtCurrency()
        {
            TxtCurrency = new PageTextBox(SourceWindow,
                                          GeneralUtils.GetResourceByName("global_currency"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtCurrency.Entry.IsEditable = false;

            TxtCurrency.SelectEntityClicked += BtnSelectCurrency_Clicked;
        }

        private void BtnSelectCurrency_Clicked(object sender, EventArgs e)
        {
            var page = new CurrenciesPage(null, PageOptions.SelectionPageOptions);
            var selectCurrencyModal = new EntitySelectionModal<Currency>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectCurrencyModal.Run();
            selectCurrencyModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCurrency.Text = page.SelectedEntity.Designation;
            }
        }

        private void InitializeTxtPaymentMethod()
        {
            TxtPaymentMethod = new PageTextBox(SourceWindow,
                                               GeneralUtils.GetResourceByName("global_payment_method"),
                                               isRequired: true,
                                               isValidatable: false,
                                               includeSelectButton: true,
                                               includeKeyBoardButton: false);

            TxtPaymentMethod.Entry.IsEditable = false;

            TxtPaymentMethod.SelectEntityClicked += BtnSelectPaymentMethod_Clicked;
        }

        private void BtnSelectPaymentMethod_Clicked(object sender, EventArgs e)
        {
            var page = new PaymentMethodsPage(null, PageOptions.SelectionPageOptions);
            var selectPaymentMethodModal = new EntitySelectionModal<PaymentMethod>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentMethodModal.Run();
            selectPaymentMethodModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtPaymentMethod.Text = page.SelectedEntity.Designation;
            }
        }

        private void InitializeTxtPaymnentCondition()
        {
            TxtPaymnentCondition = new PageTextBox(SourceWindow,
                                                   GeneralUtils.GetResourceByName("global_payment_condition"),
                                                   isRequired: true,
                                                   isValidatable: false,
                                                   includeSelectButton: true,
                                                   includeKeyBoardButton: false);

            TxtPaymnentCondition.Entry.IsEditable = false;

            TxtPaymnentCondition.SelectEntityClicked += BtnSelectPaymentCondition_Clicked;
        }

        private void BtnSelectPaymentCondition_Clicked(object sender, EventArgs e)
        {
            var page = new PaymentConditionsPage(null, PageOptions.SelectionPageOptions);
            var selectPaymentConditionModal = new EntitySelectionModal<PaymentCondition>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentConditionModal.Run();
            selectPaymentConditionModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtPaymnentCondition.Text = page.SelectedEntity.Designation;
            }
        }

        private void InitializeTxtDocumentType()
        {
            TxtDocumentType = new PageTextBox(SourceWindow,
                                              GeneralUtils.GetResourceByName("global_documentfinanceseries_documenttype"),
                                              isRequired: true,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtDocumentType.Entry.IsEditable = false;

            TxtDocumentType.SelectEntityClicked += BtnSelectDocumentType_Clicked;
        }

        private void BtnSelectDocumentType_Clicked(object sender, System.EventArgs e)
        {
            var page = new DocumentTypesPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<DocumentType>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtDocumentType.Text = page.SelectedEntity.Designation;
            }
        }

        private void Design()
        {
            var vbox = new VBox(false, 2);
            vbox.PackStart(TxtDocumentType.Component, false, false, 0);
            vbox.PackStart(TxtPaymnentCondition.Component, false, false, 0);
            vbox.PackStart(TxtPaymentMethod.Component, false, false, 0);
            vbox.PackStart(TxtCurrency.Component, false, false, 0);

            vbox.PackStart(PageTextBox.CreateHbox(TxtOriginDocument,TxtCopyDocument), false, false, 0);

            vbox.PackStart(TxtNotes.Component, false, false, 0);

            PackStart(vbox);
        }
    }
}
