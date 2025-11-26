using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.Finance.VatRates;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsFilterModal
    {
        private void InitializeTextBoxes()
        {
            InitializeTxtStartDate();
            InitializeTxtEndDate();
            InitializeTxtDocumentType();
            InitializeTxtCustomer();
            InitializeTxtWarehouse();
            InitializeTxtVatRate();
            InitializeTxtArticle();
            InitializeTxtSerialNumber();
            InitializeTxtDocumentNumber();
        }
        private void InitializeTxtCustomer()
        {
            TxtCustomer = new TextBox(this,
                                          GeneralUtils.GetResourceByName("global_customer"),
                                          isRequired: false,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtCustomer.Entry.IsEditable = true;
            TxtCustomer.WithAutoCompletion(CustomersService.AutocompleteLines, id => CustomersService.GetById(id));
            TxtCustomer.OnCompletionSelected += c => SelectCustomer(c as Customer);
            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;
        }

        private void InitializeTxtDocumentType()
        {
            TxtDocumentType = new TextBox(this,
                                              GeneralUtils.GetResourceByName("global_documentfinanceseries_documenttype"),
                                              isRequired: false,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtDocumentType.Entry.IsEditable = true;
            TxtDocumentType.WithAutoCompletion(DocumentTypesService.AutocompleteLines, id => DocumentTypesService.GetById(id));
            TxtDocumentType.OnCompletionSelected += c => SelectDocumentType(c as DocumentType);
            TxtDocumentType.SelectEntityClicked += BtnSelectDocumentType_Clicked;
        }

        private void InitializeTxtVatRate()
        {
            TxtVatRate = new TextBox(this,
                                         GeneralUtils.GetResourceByName("global_vat_rates"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: true,
                                         includeKeyBoardButton: false);

            TxtVatRate.Entry.IsEditable = true;
            TxtVatRate.WithAutoCompletion(VatRatesService.AutocompleteLines, id => VatRatesService.GetById(id));
            TxtVatRate.OnCompletionSelected += c => SelectVatRate(c as VatRate);
            TxtVatRate.SelectEntityClicked += BtnSelectVatRate_Clicked;
        }

        private void InitializeTxtWarehouse()
        {
            TxtWarehouse = new TextBox(this,
                                           GeneralUtils.GetResourceByName("global_warehouse"),
                                           isRequired: false,
                                           isValidatable: false,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: false);

            TxtWarehouse.Entry.IsEditable = true;
            TxtWarehouse.SelectEntityClicked += BtnSelectWarehouse_Clicked;
        }

        private void InitializeTxtStartDate()
        {
            TxtStartDate = new TextBox(this,
                                           GeneralUtils.GetResourceByName("global_date_start"),
                                           isRequired: false,
                                           isValidatable: true,
                                           regex: RegularExpressions.Date,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: true);

            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            TxtStartDate.Text = firstDayOfMonth.ToString("yyyy-MM-dd");

            TxtStartDate.Entry.Changed += TxtStartDate_Entry_Changed;
            TxtStartDate.SelectEntityClicked += TxtStartDate_SelectEntityClicked;
        }

        private void InitializeTxtEndDate()
        {
            TxtEndDate = new TextBox(this,
                                           GeneralUtils.GetResourceByName("global_date_end"),
                                           isRequired: false,
                                           isValidatable: true,
                                           regex: RegularExpressions.Date,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: true);

            TxtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            TxtEndDate.Entry.Changed += TxtEndDate_Entry_Changed;
            TxtEndDate.SelectEntityClicked += TxtEndDate_SelectEntityClicked;
        }

        private void InitializeTxtArticle()
        {
            TxtArticle = new TextBox(this,
                                    GeneralUtils.GetResourceByName("global_articles"),
                                    isRequired: false,
                                    isValidatable: false,
                                    includeSelectButton: true,
                                    includeKeyBoardButton: false);

            TxtArticle.Entry.IsEditable = true;
            TxtArticle.WithAutoCompletion(ArticlesService.AutocompleteLines,id => ArticlesService.GetArticleViewModel(id));
            TxtArticle.OnCompletionSelected += c => SelectArticle(c as ArticleViewModel);
            TxtArticle.SelectEntityClicked += BtnSelectArticle_Clicked;
        }

        private void InitializeTxtSerialNumber()
        {
            TxtSerialNumber = new TextBox(this,
                                              GeneralUtils.GetResourceByName("global_serialnumber"),
                                              isRequired: false,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtSerialNumber.Entry.IsEditable = true;
            TxtSerialNumber.SelectEntityClicked += BtnSelectSerialNumber_Clicked;
        }

        private void InitializeTxtDocumentNumber()
        {
            TxtDocumentNumber = new TextBox(this,
                                              GeneralUtils.GetResourceByName("global_document_number"),
                                              isRequired: false,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtDocumentNumber.Entry.IsEditable = true;
            TxtDocumentNumber.OnCompletionSelected += c => SelectDocument(c as Document);
            TxtDocumentNumber.SelectEntityClicked += BtnSelectDocumentNumber_Clicked;
        }
    }
}
