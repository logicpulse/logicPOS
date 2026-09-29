using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;

namespace LogicPOS.UI.Components.Articles.Stocks.Modals.Filters
{
    public partial class ArticleHistoryFilterModal : Modal
    {
        private TextBox TxtStartDate { get; set; }
        private TextBox TxtEndDate { get; set; }
        private TextBox TxtArticle { get; set; }
        private TextBox TxtSerialNumber { get; set; }
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);
    }
}
