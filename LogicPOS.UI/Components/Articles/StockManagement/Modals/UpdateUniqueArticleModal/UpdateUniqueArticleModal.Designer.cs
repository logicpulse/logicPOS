
using Gtk;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UpdateUniqueArticleModal
    {
        public override Size ModalSize => new Size(450, 520);
        public override string ModalTitleResourceName => "global_article";

        protected override void AddSensitiveFields()
        {

        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateSerialNumberTab(), "Editar");
        }

        private VBox CreateSerialNumberTab()
        {
            var tab = new VBox();
            tab.PackStart(new Label($"Artigo: {_entity.WarehouseArticle.Article.Designation}"), false, false, 5);
            tab.PackStart(SerialNumberField.Component, false, false, 0);
            return tab;
        }
    }
}
