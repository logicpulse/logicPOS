using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Menus;
using LogicPOS.UI.Components.POS;

namespace LogicPOS.UI.Components.Windows
{
    public partial class POSWindow : POSBaseWindow
    {

        #region Components
        private dynamic Theme { get; set; }
        public Fixed FixedWindow { get; set; } = new Fixed();
        public Label LabelClock { get; set; }
        public TextView TextViewLog { get; set; }
        public IconButtonWithText BtnQuit { get; set; }
        public IconButtonWithText BtnBackOffice { get; set; }
        public IconButtonWithText BtnReports { get; set; }
        public IconButtonWithText BtnShowSystemDialog { get; set; }
        public IconButtonWithText BtnLogOut { get; set; }
        public IconButtonWithText BtnChangeUser { get; set; }
        public IconButtonWithText BtnSessionOpening { get; set; }
        public IconButtonWithText BtnDocuments { get; set; }
        public IconButtonWithText BtnNewDocument { get; set; }
        public SaleOptionsPanel SaleOptionsPanel { get; set; }
        public ArticleFamiliesMenu MenuFamilies { get; set; }
        public ArticleSubfamiliesMenu MenuSubfamilies { get; set; }
        public ArticlesMenu MenuArticles { get; set; }
        public TextBuffer BufferTextView { get; set; }
        public Label LabelTerminalInfo { get; set; }
        public Label LabelCurrentTable { get; set; } = new Label();
        public Label LabelTotalTable { get; set; }
        #endregion
    }
}