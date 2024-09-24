using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Documents.CreateDocumentModal
{
    public class CreateDocumentArticlesTab : ModalTab
    {
        public CreateDocumentItemsPage ItemsPage { get; set; }

        public CreateDocumentArticlesTab(Window parent) : base(parent: parent,
                                                               name: GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page3"),
                                                               icon: PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_3_article.png")
        {
            Initialize();
            Design();
        }

        private void Initialize()
        {
            ItemsPage = new CreateDocumentItemsPage(SourceWindow);
            ItemsPage.Navigator.RightButtons.Remove(ItemsPage.Navigator.BtnRefresh);
            ItemsPage.Navigator.SearchBox.Bar.Remove(ItemsPage.Navigator.SearchBox.BtnFilter);
            ItemsPage.Navigator.SearchBox.Bar.Remove(ItemsPage.Navigator.SearchBox.BtnMore);
            ItemsPage.Navigator.ExtraButtonSpace.Remove(ItemsPage.Navigator.BtnApply);
        }

        private void Design()
        {
            PackStart(ItemsPage);
        }
    }
}
