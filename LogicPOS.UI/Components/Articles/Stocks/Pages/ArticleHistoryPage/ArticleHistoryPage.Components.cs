using Gtk;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Buttons;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ArticleHistoryPage
    {
        private IconButtonWithText BtnPrintSerialNumber { get; set; } = IconButtonWithText.Create("buttonUserId",
                                                                                                 "Cod.Barras",
                                                                                                 @"Icons/Dialogs/icon_pos_dialog_action_print.png");
        private IconButtonWithText BtnOpenExternalDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument);
        private IconButtonWithText BtnOpenSaleDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument);
        public PaginatedResult<ArticleHistory> Histories { get; private set; }
        public List<ArticleHistory> SelectedHistories { get; private set; } = new List<ArticleHistory>();

        protected override void InitializeGridView()
        {
            GridViewSettings.Model = new ListStore(typeof(ArticleHistory),typeof(bool));

            InitializeGridViewModel();

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Sort;
            GridView.EnableSearch = true;
            GridView.SearchColumn = 1;

            GridView.RulesHint = true;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddColumns();
            AddGridViewEventHandlers();
        }

    }
}
