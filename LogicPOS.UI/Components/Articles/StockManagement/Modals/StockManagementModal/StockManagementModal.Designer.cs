using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Pages;

namespace LogicPOS.UI.Components.Modals
{
    public partial class StockManagementModal
    {
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
            var verticalLayout = new VBox(false, 0);
            InitializeNotebook();
            verticalLayout.PackStart(Notebook, true, true, 0);
            return verticalLayout;
        }

        private void InitializeNotebook()
        {
            Notebook = new Notebook { BorderWidth = 3 };
            AddArticlesTab();
            AddStockMovementsTab();
            AddArticleHistoryTab();
            AddWarehouseManagementTab();

        }

        private void AddArticlesTab()
        {
            var articlesTab = new ArticlesPage(this);
            Notebook.AppendPage(articlesTab, new Label(LocalizedString.Instance["window_title_dialog_document_finance_page3"]));
        }

        private void AddStockMovementsTab()
        {
            var stockMovementsTab = new StockMovementsPage(this);
            Notebook.AppendPage(stockMovementsTab, new Label(LocalizedString.Instance["report_list_stock_movements"]));
        }

        private void AddArticleHistoryTab()
        {
            var tab = new VBox(false, 0);
            Notebook.AppendPage(tab, new Label(LocalizedString.Instance["global_article_history"]));
        }

        private void AddWarehouseManagementTab()
        {
            var tab = new VBox(false, 0);
            Notebook.AppendPage(tab, new Label(LocalizedString.Instance["global_warehose_management"]));
        }

    }
}
