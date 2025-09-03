using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using System.Collections.Generic;
using Customer = LogicPOS.Api.Entities.Customer;

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
        private List<Warehouse> _warehousesForCompletion;
        public List<Warehouse> WarehousesForCompletion => _warehousesForCompletion ?? InitializeWarehousesForCompletion();
        private List<ArticleHistory> _articleHistoriesForCompletion;
        public List<ArticleHistory> ArticleHistoriesForCompletion => _articleHistoriesForCompletion ?? InitializeWarehouseArticlesForCompletion();


    }
}
