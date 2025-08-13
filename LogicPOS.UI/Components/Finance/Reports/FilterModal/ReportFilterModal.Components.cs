using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.Common;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsFilterModal
    {
        public TextBox TxtStartDate { get; set; }
        public TextBox TxtEndDate { get; set; }
        public TextBox TxtDocumentType { get; set; }
        public TextBox TxtCustomer { get; set; }
        public TextBox TxtWarehouse { get; set; }
        public TextBox TxtArticle { get; set; }
        public TextBox TxtVatRate { get; set; }
        public TextBox TxtSerialNumber { get; set; }
        public TextBox TxtDocumentNumber { get; set; }
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);

        private List<DocumentType> _documentTypesForCompletion;
        public List<DocumentType> DocumentTypesForCompletion => _documentTypesForCompletion ?? InitializeDocumentTypesForCompletion();

        private List<Customer> _customersForCompletion;
        private List<Customer> CustomersForCompletion => _customersForCompletion ?? InitializeCustomersForCompletion();
        private List<Warehouse> _warehousesForCompletion;
        public List<Warehouse> WarehousesForCompletion => _warehousesForCompletion ?? InitializeWarehousesForCompletion();

        private List<ArticleViewModel> _articlesForCompletion;
        private List<ArticleViewModel> ArticlesForCompletion => _articlesForCompletion ?? InitializeArticlesForCompletion();

        private List<VatRate> _vatRatesForCompletion;
        private List<VatRate> VatRatesForCompletion => _vatRatesForCompletion ?? InitializeVatRatesForCompletion();
        private List<ArticleHistory> _articleHistoriesForCompletion;
        public List<ArticleHistory> ArticleHistoriesForCompletion => _articleHistoriesForCompletion ?? InitializeWarehouseArticlesForCompletion();

        private List<Document> _documentsForCompletion;
        private List<Document> DocumentsForCompletion => _documentsForCompletion ?? InitializeDocumentsForCompletion();


    }
}
